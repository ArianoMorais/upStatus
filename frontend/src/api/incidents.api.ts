import { http } from './http';
import type { IncidentResponse } from '@/types/incidents';

export async function listOpenIncidents(): Promise<IncidentResponse[]> {
  const { data } = await http.get<IncidentResponse[]>('/incidents');
  return data;
}

export async function getIncidentById(id: string): Promise<IncidentResponse> {
  const { data } = await http.get<IncidentResponse>(`/incidents/${id}`);
  return data;
}

export async function acknowledgeIncident(id: string): Promise<IncidentResponse> {
  const { data } = await http.post<IncidentResponse>(`/incidents/${id}/acknowledge`);
  return data;
}

export async function resolveIncident(
  id: string,
  comment?: string,
): Promise<IncidentResponse> {
  const { data } = await http.post<IncidentResponse>(`/incidents/${id}/resolve`, {
    comment,
  });
  return data;
}

export async function addIncidentComment(
  id: string,
  message: string,
): Promise<IncidentResponse> {
  const { data } = await http.post<IncidentResponse>(`/incidents/${id}/comments`, {
    message,
  });
  return data;
}
