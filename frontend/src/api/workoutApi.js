import { api } from "./client";

export const workoutApi = {
  getAll: () => api.get("/workouts"),

  create: (workout) => api.post("/workouts", workout),

  update: (id, workout) => api.put(`/workouts/${id}`, workout),

  remove: (id) => api.del(`/workouts/${id}`),
};
