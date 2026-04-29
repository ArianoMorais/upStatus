<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import dayjs from 'dayjs';
import Card from 'primevue/card';
import Button from 'primevue/button';
import { useMonitorsStore } from '@/stores/monitors.store';
import { useIncidentsStore } from '@/stores/incidents.store';
import PageHeader from '@/components/shared/PageHeader.vue';
import EmptyState from '@/components/shared/EmptyState.vue';
import MonitorStatusBadge from '@/components/monitors/MonitorStatusBadge.vue';
import IncidentRow from '@/components/incidents/IncidentRow.vue';

const monitors = useMonitorsStore();
const incidents = useIncidentsStore();
const router = useRouter();

onMounted(async () => {
  await Promise.allSettled([monitors.fetchAll(), incidents.fetchOpen()]);
});

const topIncidents = computed(() => incidents.open.slice(0, 5));

const monitorList = computed(() =>
  [...monitors.items].sort((a, b) => {
    const order = { Down: 0, Degraded: 1, Unknown: 2, Up: 3, Paused: 4 } as const;
    return order[a.status] - order[b.status];
  }),
);

function isPulsing(monitorId: string): boolean {
  const ts = monitors.lastCheckPing[monitorId];
  if (!ts) return false;
  return Date.now() - ts < 4000;
}

function formatLastCheck(value: string | null): string {
  return value ? dayjs(value).format('DD/MM HH:mm:ss') : '-';
}
</script>

<template>
  <PageHeader title="Dashboard" subtitle="Visão geral em tempo real" />

  <section class="kpis">
    <Card class="kpi up">
      <template #content>
        <div class="kpi-label">Operacionais</div>
        <div class="kpi-value">{{ monitors.totals.up }}</div>
      </template>
    </Card>
    <Card class="kpi degraded">
      <template #content>
        <div class="kpi-label">Degradados</div>
        <div class="kpi-value">{{ monitors.totals.degraded }}</div>
      </template>
    </Card>
    <Card class="kpi down">
      <template #content>
        <div class="kpi-label">Fora do ar</div>
        <div class="kpi-value">{{ monitors.totals.down }}</div>
      </template>
    </Card>
    <Card class="kpi paused">
      <template #content>
        <div class="kpi-label">Pausados</div>
        <div class="kpi-value">{{ monitors.totals.paused }}</div>
      </template>
    </Card>
    <Card class="kpi total">
      <template #content>
        <div class="kpi-label">Total</div>
        <div class="kpi-value">{{ monitors.totals.total }}</div>
      </template>
    </Card>
  </section>

  <div class="grid">
    <Card class="block">
      <template #title>
        <div class="block-title">
          <span>Incidentes abertos</span>
          <Button
            label="Ver todos"
            text
            size="small"
            @click="router.push({ name: 'incidents' })"
          />
        </div>
      </template>
      <template #content>
        <EmptyState
          v-if="!topIncidents.length"
          icon="pi-shield"
          title="Sem incidentes"
          description="Nenhum monitor com problema agora."
        />
        <div v-else class="incidents">
          <IncidentRow v-for="i in topIncidents" :key="i.id" :incident="i" />
        </div>
      </template>
    </Card>

    <Card class="block">
      <template #title>
        <div class="block-title">
          <span>Monitores</span>
          <Button
            label="Gerenciar"
            text
            size="small"
            @click="router.push({ name: 'monitors' })"
          />
        </div>
      </template>
      <template #content>
        <EmptyState
          v-if="!monitorList.length"
          icon="pi-globe"
          title="Nenhum monitor"
          description="Crie seu primeiro monitor para começar."
        />
        <ul v-else class="monitors">
          <li
            v-for="m in monitorList"
            :key="m.id"
            class="monitor-row"
            :class="{ pulse: isPulsing(m.id) }"
            @click="router.push({ name: 'monitor-detail', params: { id: m.id } })"
          >
            <div class="left">
              <MonitorStatusBadge :status="m.status" size="sm" />
              <strong>{{ m.name }}</strong>
            </div>
            <span class="text-muted">{{ formatLastCheck(m.lastCheckedAt) }}</span>
          </li>
        </ul>
      </template>
    </Card>
  </div>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;
@use '@/styles/mixins' as *;

.kpis {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
  gap: $space-md;
  margin-bottom: $space-lg;
}

.kpi-label {
  color: $color-text-muted;
  font-size: 0.85rem;
  margin-bottom: $space-xs;
}

.kpi-value {
  font-size: 2rem;
  font-weight: 700;
}

.kpi.up .kpi-value { color: $color-up; }
.kpi.down .kpi-value { color: $color-down; }
.kpi.degraded .kpi-value { color: $color-degraded; }
.kpi.paused .kpi-value { color: $color-paused; }

.grid {
  display: grid;
  grid-template-columns: 1.2fr 1fr;
  gap: $space-md;

  @include md-down {
    grid-template-columns: 1fr;
  }
}

.block-title {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
}

.incidents {
  display: flex;
  flex-direction: column;
  gap: $space-sm;
}

.monitors {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
}

.monitor-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: $space-sm $space-md;
  border-radius: $radius-md;
  cursor: pointer;
  transition: background 0.15s ease, box-shadow 0.4s ease;
  border-bottom: 1px solid transparent;

  &:hover {
    background: $color-bg;
  }

  &.pulse {
    box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.3);
  }
}

.left {
  display: flex;
  align-items: center;
  gap: $space-sm;
}

.text-muted {
  color: $color-text-muted;
  font-size: 0.85rem;
}
</style>
