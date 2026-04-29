<script setup lang="ts">
import { computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import dayjs from 'dayjs';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import { useIncidentsStore } from '@/stores/incidents.store';
import { useMonitorsStore } from '@/stores/monitors.store';
import { useApiError } from '@/composables/useApiError';
import { useAppToast } from '@/composables/useToast';
import PageHeader from '@/components/shared/PageHeader.vue';
import EmptyState from '@/components/shared/EmptyState.vue';
import LoadingPanel from '@/components/shared/LoadingPanel.vue';
import IncidentStatusBadge from '@/components/incidents/IncidentStatusBadge.vue';

const incidents = useIncidentsStore();
const monitors = useMonitorsStore();
const router = useRouter();
const { extract } = useApiError();
const toast = useAppToast();

onMounted(async () => {
  try {
    await Promise.all([incidents.fetchOpen(), monitors.fetchAll()]);
  } catch (err) {
    toast.error('Erro', extract(err).message);
  }
});

const open = computed(() => incidents.open);

function monitorName(id: string) {
  return monitors.byId.get(id)?.name ?? '-';
}

function format(value: string | null) {
  return value ? dayjs(value).format('DD/MM/YYYY HH:mm:ss') : '-';
}

function onRowClick(event: { data: { id: string } }) {
  router.push({ name: 'incident-detail', params: { id: event.data.id } });
}
</script>

<template>
  <PageHeader title="Incidentes abertos" subtitle="Atualizado em tempo real">
    <template #actions>
      <Button
        label="Atualizar"
        icon="pi pi-refresh"
        severity="secondary"
        text
        @click="incidents.fetchOpen(true)"
      />
    </template>
  </PageHeader>

  <LoadingPanel v-if="incidents.loading && !incidents.loaded" />

  <EmptyState
    v-else-if="!open.length"
    icon="pi-shield"
    title="Tudo tranquilo por aqui"
    description="Nenhum incidente em aberto no momento."
  />

  <DataTable
    v-else
    :value="open"
    data-key="id"
    :paginator="open.length > 10"
    :rows="10"
    striped-rows
    @row-click="onRowClick"
  >
    <Column header="Monitor">
      <template #body="{ data }">
        <strong>{{ monitorName(data.monitorId) }}</strong>
      </template>
    </Column>
    <Column field="reason" header="Motivo">
      <template #body="{ data }">
        {{ data.reason ?? '-' }}
      </template>
    </Column>
    <Column field="status" header="Status">
      <template #body="{ data }">
        <IncidentStatusBadge :status="data.status" />
      </template>
    </Column>
    <Column field="startedAt" header="Iniciado em">
      <template #body="{ data }">{{ format(data.startedAt) }}</template>
    </Column>
    <Column field="acknowledgedAt" header="Reconhecido em">
      <template #body="{ data }">{{ format(data.acknowledgedAt) }}</template>
    </Column>
    <Column header="" :style="{ width: '4rem' }">
      <template #body="{ data }">
        <Button
          icon="pi pi-arrow-right"
          text
          rounded
          severity="secondary"
          @click.stop="router.push({ name: 'incident-detail', params: { id: data.id } })"
        />
      </template>
    </Column>
  </DataTable>
</template>

<style scoped lang="scss">
:deep(.p-datatable-tbody > tr) {
  cursor: pointer;
}
</style>
