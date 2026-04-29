export type MonitorStatus = 'Up' | 'Down' | 'Degraded' | 'Paused' | 'Unknown';

export interface MonitorConfig {
  intervalSeconds: number;
  timeoutMs: number;
  failuresToOpenIncident: number;
  successesToCloseIncident: number;
  degradedLatencyMs: number;
  expectedStatusCode?: number | null;
  httpMethod: string;
}

export interface MonitorResponse {
  id: string;
  name: string;
  url: string;
  status: MonitorStatus;
  isPaused: boolean;
  createdAt: string;
  lastCheckedAt: string | null;
  createdBy: string;
  config: MonitorConfig;
}

export interface CreateMonitorRequest {
  name: string;
  url: string;
  config: MonitorConfig;
}

export interface UpdateMonitorRequest {
  name: string;
  url: string;
  config: MonitorConfig;
}

export interface CreateMonitorResponse {
  id: string;
}

export const DEFAULT_MONITOR_CONFIG: MonitorConfig = {
  intervalSeconds: 60,
  timeoutMs: 5000,
  failuresToOpenIncident: 3,
  successesToCloseIncident: 2,
  degradedLatencyMs: 1500,
  expectedStatusCode: null,
  httpMethod: 'GET',
};
