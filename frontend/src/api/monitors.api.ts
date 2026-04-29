import { http } from './http';
import type {
  CreateMonitorRequest,
  CreateMonitorResponse,
  MonitorResponse,
  UpdateMonitorRequest,
} from '@/types/monitors';

export async function listMonitors(): Promise<MonitorResponse[]> {
  const { data } = await http.get<MonitorResponse[]>('/monitors');
  return data;
}

export async function getMonitorById(id: string): Promise<MonitorResponse> {
  const { data } = await http.get<MonitorResponse>(`/monitors/${id}`);
  return data;
}

export async function createMonitor(
  req: CreateMonitorRequest,
): Promise<CreateMonitorResponse> {
  const { data } = await http.post<CreateMonitorResponse>('/monitors', req);
  return data;
}

export async function updateMonitor(
  id: string,
  req: UpdateMonitorRequest,
): Promise<MonitorResponse> {
  const { data } = await http.put<MonitorResponse>(`/monitors/${id}`, req);
  return data;
}

export async function pauseMonitor(id: string): Promise<MonitorResponse> {
  const { data } = await http.post<MonitorResponse>(`/monitors/${id}/pause`);
  return data;
}

export async function resumeMonitor(id: string): Promise<MonitorResponse> {
  const { data } = await http.post<MonitorResponse>(`/monitors/${id}/resume`);
  return data;
}

export async function deleteMonitor(id: string): Promise<void> {
  await http.delete(`/monitors/${id}`);
}
