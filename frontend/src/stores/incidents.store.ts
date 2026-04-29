import { computed, ref } from 'vue';
import { defineStore } from 'pinia';
import * as incidentsApi from '@/api/incidents.api';
import type { IncidentResponse, IncidentCommentResponse } from '@/types/incidents';

export const useIncidentsStore = defineStore('incidents', () => {
  const items = ref<IncidentResponse[]>([]);
  const selected = ref<IncidentResponse | null>(null);
  const loading = ref(false);
  const loaded = ref(false);

  const open = computed(() => items.value.filter((i) => i.status !== 'Resolved'));
  const openCount = computed(() => open.value.length);

  async function fetchOpen(force = false): Promise<void> {
    if (loaded.value && !force) return;
    loading.value = true;
    try {
      items.value = await incidentsApi.listOpenIncidents();
      loaded.value = true;
    } finally {
      loading.value = false;
    }
  }

  async function fetchById(id: string): Promise<IncidentResponse> {
    const incident = await incidentsApi.getIncidentById(id);
    upsert(incident);
    selected.value = incident;
    return incident;
  }

  async function acknowledge(id: string): Promise<void> {
    const updated = await incidentsApi.acknowledgeIncident(id);
    upsert(updated);
    if (selected.value?.id === id) selected.value = updated;
  }

  async function resolve(id: string, comment?: string): Promise<void> {
    const updated = await incidentsApi.resolveIncident(id, comment);
    upsert(updated);
    if (selected.value?.id === id) selected.value = updated;
  }

  async function addComment(id: string, message: string): Promise<void> {
    const updated = await incidentsApi.addIncidentComment(id, message);
    upsert(updated);
    if (selected.value?.id === id) selected.value = updated;
  }

  function applyOpened(incident: IncidentResponse): void {
    upsert(incident);
  }

  function applyResolved(incident: IncidentResponse): void {
    upsert(incident);
    if (selected.value?.id === incident.id) selected.value = incident;
  }

  function applyAcknowledged(incident: IncidentResponse): void {
    upsert(incident);
    if (selected.value?.id === incident.id) selected.value = incident;
  }

  function applyCommented(
    incident: IncidentResponse,
    _comment: IncidentCommentResponse,
  ): void {
    upsert(incident);
    if (selected.value?.id === incident.id) selected.value = incident;
  }

  function upsert(incident: IncidentResponse): void {
    const idx = items.value.findIndex((i) => i.id === incident.id);
    if (idx >= 0) items.value.splice(idx, 1, incident);
    else items.value.unshift(incident);
  }

  function reset(): void {
    items.value = [];
    selected.value = null;
    loaded.value = false;
  }

  return {
    items,
    selected,
    loading,
    loaded,
    open,
    openCount,
    fetchOpen,
    fetchById,
    acknowledge,
    resolve,
    addComment,
    applyOpened,
    applyResolved,
    applyAcknowledged,
    applyCommented,
    reset,
  };
});
