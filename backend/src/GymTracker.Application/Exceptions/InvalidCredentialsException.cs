namespace GymTracker.Application.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Email ili lozinka nisu ispravni.")
    {
    }
}
