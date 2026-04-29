<script setup lang="ts">
import { computed } from 'vue';
import { useRouter } from 'vue-router';
import dayjs from 'dayjs';
import IncidentStatusBadge from './IncidentStatusBadge.vue';
import { useMonitorsStore } from '@/stores/monitors.store';
import type { IncidentResponse } from '@/types/incidents';

const props = defineProps<{ incident: IncidentResponse }>();

const router = useRouter();
const monitors = useMonitorsStore();

const monitor = computed(() => monitors.byId.get(props.incident.monitorId));

function open() {
  router.push({ name: 'incident-detail', params: { id: props.incident.id } });
}
</script>

<template>
  <div class="row" @click="open">
    <div class="info">
      <div class="title">
        <strong>{{ monitor?.name ?? 'Monitor desconhecido' }}</strong>
        <IncidentStatusBadge :status="incident.status" />
      </div>
      <div class="reason">{{ incident.reason ?? 'Sem motivo informado' }}</div>
      <div class="meta">
        Iniciado em {{ dayjs(incident.startedAt).format('DD/MM HH:mm:ss') }}
        <span v-if="incident.acknowledgedAt">
          · reconhecido às {{ dayjs(incident.acknowledgedAt).format('HH:mm') }}
        </span>
      </div>
    </div>
    <i class="pi pi-chevron-right" />
  </div>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: $space-md;
  padding: $space-md;
  border: 1px solid $color-border;
  border-radius: $radius-md;
  background: $color-surface;
  cursor: pointer;
  transition: background 0.15s ease;

  &:hover {
    background: $color-bg;
  }

  i.pi-chevron-right {
    color: $color-text-muted;
  }
}

.info {
  display: flex;
  flex-direction: column;
  gap: $space-xs;
  flex: 1;
}

.title {
  display: flex;
  align-items: center;
  gap: $space-sm;
  flex-wrap: wrap;
}

.reason {
  color: $color-text;
  font-size: 0.9rem;
}

.meta {
  color: $color-text-muted;
  font-size: 0.8rem;
}
</style>
