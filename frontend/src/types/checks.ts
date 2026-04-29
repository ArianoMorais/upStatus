export type CheckResult = 'Success' | 'Failure' | 'Timeout' | 'Degraded';

export interface CheckResponse {
  id: string;
  monitorId: string;
  timestamp: string;
  result: CheckResult;
  statusCode: number | null;
  latencyMs: number;
  errorMessage: string | null;
}

export interface MonitorChecksResponse {
  monitorId: string;
  from: string;
  to: string;
  total: number;
  items: CheckResponse[];
}

export interface MonitorUptimeResponse {
  monitorId: string;
  range: string;
  from: string;
  to: string;
  total: number;
  successful: number;
  degraded: number;
  failed: number;
  uptimePercent: number;
  p50LatencyMs: number | null;
  p95LatencyMs: number | null;
  p99LatencyMs: number | null;
}

export type UptimeRange = '1h' | '24h' | '7d' | '30d';
