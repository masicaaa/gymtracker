import { api } from "./client";

export const progressApi = {
  getMonth: (year, month) => api.get(`/progress?year=${year}&month=${month}`),
};
