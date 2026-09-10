# GymTracker

Aplikacija za praćenje treninga u teretani — korisnik bilježi svoje treninge i prati
napredak po sedmicama.

Backend je .NET 8 Web API pisan po Clean arhitekturi, frontend React, baza MySQL.

---

## Šta aplikacija radi

- **Registracija i prijava** — JWT token, lozinke se čuvaju kao BCrypt heš
- **Unos treninga** — vrsta vježbe, datum i vrijeme, trajanje, potrošene kalorije,
  težina treninga (1–10), umor poslije treninga (1–10) i bilješka
- **Izmjena i brisanje** treninga
- **Praćenje napretka** — izbor mjeseca, pa za svaku sedmicu tog mjeseca: broj treninga,
  ukupno trajanje, prosječna težina i prosječan umor

Svaki korisnik vidi isključivo svoje treninge.

---

## Tehnologije

| Sloj | Tehnologije |
|---|---|
| Backend | .NET 8, ASP.NET Core Web API, EF Core 8 (Pomelo MySQL) |
| Sigurnost | JWT (Bearer), BCrypt |
| Validacija | FluentValidation |
| Baza | MySQL 8 |
| Frontend | React 19, Vite, React Router, `fetch` |

---

## Arhitektura

Backend sam podijelila na četiri projekta. Zavisnosti idu **samo ka unutra**, pa
`Application` sloj nema referencu na EF Core i ne može pozvati bazu — to pravilo čuva
kompajler, a ne dogovor.

```
GymTracker.Api            →  kontroleri, DI, JWT, Swagger, hvatanje grešaka
      ↓
GymTracker.Infrastructure →  EF Core i MySQL, BCrypt, izdavanje tokena
      ↓
GymTracker.Application    →  poslovna logika, DTO-ovi, validatori, interfejsi
      ↓
GymTracker.Domain         →  entiteti, bez ijedne zavisnosti
```

Interfejse (`IUserRepository`, `IWorkoutRepository`, `IPasswordHasher`, `ITokenGenerator`)
držim u `Application` sloju, a implementacije u `Infrastructure`. Spajaju se na jednom
mjestu, kroz `AddApplication()` i `AddInfrastructure()`.

---

## Pokretanje

**Preduslovi:** .NET 8 SDK, Node.js 20+, MySQL 8 na portu 3306.

### 1. Baza

```sql
CREATE USER 'gymtracker'@'localhost' IDENTIFIED BY 'VAŠA_LOZINKA';
GRANT ALL PRIVILEGES ON gymtracker.* TO 'gymtracker'@'localhost';
FLUSH PRIVILEGES;
```

U `backend/src/GymTracker.Api/` napraviti `appsettings.Development.json` po uzoru na
`appsettings.Development.json.example` i upisati tu lozinku. Šemu i tabele pravi migracija:

```bash
cd backend
dotnet ef database update -p src/GymTracker.Infrastructure -s src/GymTracker.Api
```

### 2. Backend

```bash
cd backend/src/GymTracker.Api
dotnet run --launch-profile http
```

API: `http://localhost:5067` · Swagger sa punom listom endpointa: `/swagger`

### 3. Frontend

```bash
cd frontend
npm install
npm run dev
```

Aplikacija: `http://localhost:5173`

**Backend se pokreće prvi** — frontend bez njega ne može dohvatiti podatke.

---

## Odluke koje se ne vide iz koda

**Korisnika ne uzimam iz zahtjeva, nego iz tokena.** Njegov `Id` ulazi u `WHERE` uslov
svakog upita, pa tuđi trening ne može ni da se dohvati — nema naknadne provjere koja se
može zaboraviti. Za tuđi ili nepostojeći zapis vraćam `404`, a ne `403`, da odgovor ne
potvrdi da zapis postoji.

**Vrijeme treninga namjerno ne pretvaram u UTC.** `PerformedAt` je lokalno vrijeme koje je
korisnik unio. Da ga pretvaram, trening u ponedjeljak u 00:30 pomjerio bi se u nedjelju i
upao u pogrešnu sedmicu — a sedmice su osnova cijele statistike. `CreatedAtUtc` je
tehnički podatak i njega čuvam u UTC-u.

**Sedmice počinju ponedjeljkom i odsijecam ih na mjesec.** Ako mjesec počne u utorak, prva
sedmica ide od prvog u mjesecu, a ne od ponedjeljka prethodnog — inače bi se u zbir uvukli
treninzi iz drugog mjeseca. Sedmice bez treninga prikazujem sa nulama, jer je propuštena
sedmica takođe informacija.

**Prosjek prazne sedmice je `null`, a ne nula.** Nula bi značila „vrlo lagani treninzi", a
treninga uopšte nije bilo — zato u prikazu stoji crtica. Time je riješeno i dijeljenje
nulom.

**Validaciju držim u `Application` sloju**, da važi bez obzira odakle poziv dolazi, a ista
pravila su osigurana i `CHECK` ograničenjima u bazi. Greške vraćam u standardnom
`ProblemDetails` formatu (RFC 7807), grupisane po polju, pa ih forma prikaže uz tačno
polje.

**Aplikacija se ne kači na bazu kao `root`,** nego nalogom koji ima prava isključivo nad
šemom `gymtracker`.

---

## Konfiguracija

`appsettings.Development.json` je u `.gitignore` — u repozitorijumu je samo `.example`.
`frontend/.env` sadrži isključivo adresu API-ja pa je komitovan; sve povjerljivo išlo bi u
`.env.local`, koji nije. Ključ za potpisivanje tokena u `appsettings.json` je razvojni; u
produkciji bi dolazio iz varijable okruženja.
