<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import dayjs from 'dayjs';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';
import { useConfirm } from 'primevue/useconfirm';
import { useMonitorsStore } from '@/stores/monitors.store';
import { useApiError } from '@/composables/useApiError';
import { useAppToast } from '@/composables/useToast';
import PageHeader from '@/components/shared/PageHeader.vue';
import EmptyState from '@/components/shared/EmptyState.vue';
import LoadingPanel from '@/components/shared/LoadingPanel.vue';
import MonitorStatusBadge from '@/components/monitors/MonitorStatusBadge.vue';

const monitors = useMonitorsStore();
const router = useRouter();
const confirm = useConfirm();
const toast = useAppToast();
const { extract } = useApiError();

const filter = ref('');

onMounted(() => monitors.fetchAll());

const filtered = computed(() => {
  const q = filter.value.trim().toLowerCase();
  if (!q) return monitors.items;
  return monitors.items.filter(
    (m) => m.name.toLowerCase().includes(q) || m.url.toLowerCase().includes(q),
  );
});

function formatLastChecked(value: string | null): string {
  if (!value) return '-';
  return dayjs(value).format('DD/MM/YYYY HH:mm:ss');
}

function goToDetail(id: string) {
  router.push({ name: 'monitor-detail', params: { id } });
}

function goToEdit(id: string) {
  router.push({ name: 'monitor-edit', params: { id } });
}

async function togglePause(id: string, isPaused: boolean) {
  try {
    if (isPaused) await monitors.resume(id);
    else await monitors.pause(id);
    toast.success(isPaused ? 'Monitor retomado' : 'Monitor pausado');
  } catch (err) {
    toast.error('Erro', extract(err).message);
  }
}

function confirmRemove(id: string, name: string) {
  confirm.require({
    message: `Excluir o monitor "${name}"? Essa ação não pode ser desfeita.`,
    header: 'Confirmar exclusão',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Excluir',
    rejectLabel: 'Cancelar',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await monitors.remove(id);
        toast.success('Monitor excluído');
      } catch (err) {
        toast.error('Erro ao excluir', extract(err).message);
      }
    },
  });
}
</script>

<template>
  <PageHeader title="Monitores" subtitle="Gerencie os endpoints monitorados">
    <template #actions>
      <Button
        label="Novo monitor"
        icon="pi pi-plus"
        @click="router.push({ name: 'monitor-new' })"
      />
    </template>
  </PageHeader>

  <LoadingPanel v-if="monitors.loading && !monitors.loaded" message="Carregando monitores..." />

  <EmptyState
    v-else-if="!monitors.items.length"
    icon="pi-globe"
    title="Nenhum monitor cadastrado"
    description="Comece criando seu primeiro monitor para acompanhar a disponibilidade dos seus serviços."
  >
    <Button
      label="Criar monitor"
      icon="pi pi-plus"
      @click="router.push({ name: 'monitor-new' })"
    />
  </EmptyState>

  <div v-else class="list">
    <div class="filter">
      <IconField>
        <InputIcon class="pi pi-search" />
        <InputText v-model="filter" placeholder="Buscar por nome ou URL..." fluid />
      </IconField>
    </div>

    <DataTable
      :value="filtered"
      data-key="id"
      :paginator="filtered.length > 10"
      :rows="10"
      striped-rows
      removable-sort
    >
      <Column field="name" header="Nome" sortable>
        <template #body="{ data }">
          <a class="link" @click.prevent="goToDetail(data.id)" href="#">{{ data.name }}</a>
        </template>
      </Column>
      <Column field="url" header="URL">
        <template #body="{ data }">
          <span class="text-mono">{{ data.url }}</span>
        </template>
      </Column>
      <Column field="status" header="Status" sortable>
        <template #body="{ data }">
          <MonitorStatusBadge :status="data.status" />
        </template>
      </Column>
      <Column field="lastCheckedAt" header="Última verificação" sortable>
        <template #body="{ data }">
          {{ formatLastChecked(data.lastCheckedAt) }}
        </template>
      </Column>
      <Column header="Ações" :style="{ width: '14rem' }">
        <template #body="{ data }">
          <div class="actions">
            <Button
              icon="pi pi-eye"
              text
              rounded
              severity="secondary"
              aria-label="Ver detalhes"
              @click="goToDetail(data.id)"
            />
            <Button
              icon="pi pi-pencil"
              text
              rounded
              severity="secondary"
              aria-label="Editar"
              @click="goToEdit(data.id)"
            />
            <Button
              :icon="data.isPaused ? 'pi pi-play' : 'pi pi-pause'"
              text
              rounded
              severity="secondary"
              :aria-label="data.isPaused ? 'Retomar' : 'Pausar'"
              @click="togglePause(data.id, data.isPaused)"
            />
            <Button
              icon="pi pi-trash"
              text
              rounded
              severity="danger"
              aria-label="Excluir"
              @click="confirmRemove(data.id, data.name)"
            />
          </div>
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.list {
  display: flex;
  flex-direction: column;
  gap: $space-md;
}

.filter {
  max-width: 360px;
}

.link {
  color: $color-primary;
  font-weight: 500;
  cursor: pointer;
}

.actions {
  display: flex;
  gap: 2px;
}

.text-mono {
  font-family: ui-monospace, SFMono-Regular, Menlo, Consolas, monospace;
  font-size: 0.85rem;
  color: $color-text-muted;
}
</style>
