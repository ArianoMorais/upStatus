import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';
import { useAuthStore } from '@/stores/auth.store';
import { useMonitorsStore } from '@/stores/monitors.store';
import { useIncidentsStore } from '@/stores/incidents.store';
import { useRealtimeStore } from '@/stores/realtime.store';
import { useAppToast } from '@/composables/useToast';
import type { MonitorResponse } from '@/types/monitors';
import type { CheckResponse } from '@/types/checks';
import type {
  IncidentResponse,
  IncidentCommentResponse,
} from '@/types/incidents';

const HUB_PATH = '/hubs/monitoring';

const EVENT_MONITOR_STATUS = 'monitor.status_changed';
const EVENT_CHECK_RECORDED = 'check.recorded';
const EVENT_INCIDENT_OPENED = 'incident.opened';
const EVENT_INCIDENT_RESOLVED = 'incident.resolved';
const EVENT_INCIDENT_ACKNOWLEDGED = 'incident.acknowledged';
const EVENT_INCIDENT_COMMENTED = 'incident.commented';

let connection: HubConnection | null = null;

function buildHubUrl(): string {
  const base = import.meta.env.VITE_API_URL || '';
  if (!base || base.startsWith('/')) {
    return `${base}${HUB_PATH}`.replace(/\/+/, '/');
  }
  return `${base.replace(/\/$/, '')}${HUB_PATH}`;
}

export function useRealtime() {
  const auth = useAuthStore();
  const monitors = useMonitorsStore();
  const incidents = useIncidentsStore();
  const realtime = useRealtimeStore();
  const toast = useAppToast();

  function bindHandlers(conn: HubConnection): void {
    conn.on(
      EVENT_MONITOR_STATUS,
      (payload: { monitor: MonitorResponse; previousStatus: string }) => {
        monitors.applyStatusChange(payload.monitor, payload.previousStatus);
      },
    );

    conn.on(EVENT_CHECK_RECORDED, (check: CheckResponse) => {
      monitors.applyCheckRecorded(check);
    });

    conn.on(EVENT_INCIDENT_OPENED, (incident: IncidentResponse) => {
      incidents.applyOpened(incident);
      const monitor = monitors.byId.get(incident.monitorId);
      toast.error(
        'Incidente aberto',
        `${monitor?.name ?? 'Monitor'}: ${incident.reason ?? 'Sem motivo informado'}`,
      );
    });

    conn.on(EVENT_INCIDENT_RESOLVED, (incident: IncidentResponse) => {
      incidents.applyResolved(incident);
      const monitor = monitors.byId.get(incident.monitorId);
      toast.success('Incidente resolvido', monitor?.name ?? 'Monitor');
    });

    conn.on(EVENT_INCIDENT_ACKNOWLEDGED, (incident: IncidentResponse) => {
      incidents.applyAcknowledged(incident);
    });

    conn.on(
      EVENT_INCIDENT_COMMENTED,
      (payload: { incident: IncidentResponse; comment: IncidentCommentResponse }) => {
        incidents.applyCommented(payload.incident, payload.comment);
      },
    );
  }

  async function start(): Promise<void> {
    if (connection && connection.state !== HubConnectionState.Disconnected) return;

    const token = auth.token;
    if (!token) return;

    realtime.setStatus('connecting');

    connection = new HubConnectionBuilder()
      .withUrl(buildHubUrl(), {
        accessTokenFactory: () => auth.token ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    bindHandlers(connection);

    connection.onreconnecting(() => realtime.setStatus('reconnecting'));
    connection.onreconnected(() => realtime.setStatus('connected'));
    connection.onclose(() => realtime.setStatus('disconnected'));

    try {
      await connection.start();
      realtime.setStatus('connected');
    } catch (err) {
      console.error('SignalR connection error', err);
      realtime.setStatus('disconnected');
    }
  }

  async function stop(): Promise<void> {
    if (!connection) return;
    try {
      await connection.stop();
    } finally {
      connection = null;
      realtime.setStatus('disconnected');
    }
  }

  return { start, stop };
}
