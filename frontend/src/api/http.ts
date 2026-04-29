import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios';
import type { ProblemDetails } from '@/types/problem';

const baseURL = import.meta.env.VITE_API_URL || 'http://localhost:5137';

export const http = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
});

let tokenProvider: () => string | null = () => null;
let onUnauthorized: () => void = () => {};

export function configureHttp(opts: {
  getToken: () => string | null;
  onUnauthorized: () => void;
}): void {
  tokenProvider = opts.getToken;
  onUnauthorized = opts.onUnauthorized;
}

http.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = tokenProvider();
  if (token) {
    config.headers.set('Authorization', `Bearer ${token}`);
  }
  return config;
});

http.interceptors.response.use(
  (res) => res,
  (error: AxiosError<ProblemDetails>) => {
    if (error.response?.status === 401) {
      onUnauthorized();
    }
    return Promise.reject(error);
  },
);

export function isProblemDetails(value: unknown): value is ProblemDetails {
  return !!value && typeof value === 'object' && ('title' in value || 'status' in value);
}
