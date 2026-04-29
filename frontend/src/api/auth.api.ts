import { http } from './http';
import type { AuthUserResponse, LoginRequest, LoginResponse } from '@/types/auth';

export async function login(req: LoginRequest): Promise<LoginResponse> {
  const { data } = await http.post<LoginResponse>('/auth/login', req);
  return data;
}

export async function getMe(): Promise<AuthUserResponse> {
  const { data } = await http.get<AuthUserResponse>('/auth/me');
  return data;
}
