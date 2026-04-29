import { computed, ref } from 'vue';
import { defineStore } from 'pinia';
import * as monitorsApi from '@/api/monitors.api';
import type {
  CreateMonitorRequest,
  MonitorResponse,
  MonitorStatus,
  UpdateMonitorRequest,
} from '@/types/monitors';
import type { CheckResponse } from '@/types/checks';

const RECENT_CHECKS_BUFFER = 50;

export const useMonitorsStore = defineStore('monitors', () => {
  const items = ref<MonitorResponse[]>([]);
  const loading = ref(false);
  const loaded = ref(false);
  const recentChecks = ref<Record<string, CheckResponse[]>>({});
  const lastCheckPing = ref<Record<string, number>>({});

  const byId = computed(() => {
    const map = new Map<string, MonitorResponse>();
    for (const m of items.value) map.set(m.id, m);
    return map;
  });

  const totals = computed(() => {
    const counts: Record<MonitorStatus, number> = {
      Up: 0,
      Down: 0,
      Degraded: 0,
      Paused: 0,
      Unknown: 0,
    };
    for (const m of items.value) counts[m.status] += 1;
    return {
      total: items.value.length,
      up: counts.Up,
      down: counts.Down,
      degraded: counts.Degraded,
      paused: counts.Paused,
      unknown: counts.Unknown,
    };
  });

  async function fetchAll(force = false): Promise<void> {
    if (loaded.value && !force) return;
    loading.value = true;
    try {
      items.value = await monitorsApi.listMonitors();
      loaded.value = true;
    } finally {
      loading.value = false;
    }
  }

  async function fetchById(id: string): Promise<MonitorResponse> {
    const monitor = await monitorsApi.getMonitorById(id);
    upsert(monitor);
    return monitor;
  }

  async function create(req: CreateMonitorRequest): Promise<string> {
    const res = await monitorsApi.createMonitor(req);
    await fetchAll(true);
    return res.id;
  }

  async function update(id: string, req: UpdateMonitorRequest): Promise<MonitorResponse> {
    const monitor = await monitorsApi.updateMonitor(id, req);
    upsert(monitor);
    return monitor;
  }

  async function pause(id: string): Promise<void> {
    const monitor = await monitorsApi.pauseMonitor(id);
    upsert(monitor);
  }

  async function resume(id: string): Promise<void> {
    const monitor = await monitorsApi.resumeMonitor(id);
    upsert(monitor);
  }

  async function remove(id: string): Promise<void> {
    await monitorsApi.deleteMonitor(id);
    items.value = items.value.filter((m) => m.id !== id);
  }

  function upsert(monitor: MonitorResponse): void {
    const idx = items.value.findIndex((m) => m.id === monitor.id);
    if (idx >= 0) items.value.splice(idx, 1, monitor);
    else items.value.push(monitor);
  }

  function applyStatusChange(monitor: MonitorResponse, _previousStatus: string): void {
    upsert(monitor);
  }

  function applyCheckRecorded(check: CheckResponse): void {
    const buf = recentChecks.value[check.monitorId] ?? [];
    const next = [...buf, check].slice(-RECENT_CHECKS_BUFFER);
    recentChecks.value = { ...recentChecks.value, [check.monitorId]: next };
    lastCheckPing.value = { ...lastCheckPing.value, [check.monitorId]: Date.now() };

    const monitor = items.value.find((m) => m.id === check.monitorId);
    if (monitor) {
      upsert({ ...monitor, lastCheckedAt: check.timestamp });
    }
  }

  function seedRecentChecks(monitorId: string, checks: CheckResponse[]): void {
    const sorted = [...checks].sort((a, b) =>
      a.timestamp.localeCompare(b.timestamp),
    );
    recentChecks.value = {
      ...recentChecks.value,
      [monitorId]: sorted.slice(-RECENT_CHECKS_BUFFER),
    };
  }

  function reset(): void {
    items.value = [];
    loaded.value = false;
    recentChecks.value = {};
    lastCheckPing.value = {};
  }

  return {
    items,
    loading,
    loaded,
    recentChecks,
    lastCheckPing,
    byId,
    totals,
    fetchAll,
    fetchById,
    create,
    update,
    pause,
    resume,
    remove,
    upsert,
    applyStatusChange,
    applyCheckRecorded,
    seedRecentChecks,
    reset,
  };
});
