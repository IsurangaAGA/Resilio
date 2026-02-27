import axios from "axios";

const baseURL = import.meta.env.VITE_API_BASE_URL;

console.log("API BASE:", baseURL);

export const http = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
  timeout: 30000,
});

http.interceptors.request.use((config) => {
  const token = localStorage.getItem("accessToken");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});