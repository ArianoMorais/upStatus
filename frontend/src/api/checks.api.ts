import { http } from './http';
import type {
  MonitorChecksResponse,
  MonitorUptimeResponse,
  UptimeRange,
} from '@/types/checks';

export interface ListChecksParams {
  from?: string;
  to?: string;
  limit?: number;
}

export async function listMonitorChecks(
  monitorId: string,
  params: ListChecksParams = {},
): Promise<MonitorChecksResponse> {
  const { data } = await http.get<MonitorChecksResponse>(
    `/monitors/${monitorId}/checks`,
    { params },
  );
  return data;
}

export async function getMonitorUptime(
  monitorId: string,
  range: UptimeRange = '24h',
): Promise<MonitorUptimeResponse> {
  const { data } = await http.get<MonitorUptimeResponse>(
    `/monitors/${monitorId}/uptime`,
    { params: { range } },
  );
  return data;
}
