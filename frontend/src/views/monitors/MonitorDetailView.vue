<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import dayjs from 'dayjs';
import Card from 'primevue/card';
import Button from 'primevue/button';
import SelectButton from 'primevue/selectbutton';
import Tabs from 'primevue/tabs';
import TabList from 'primevue/tablist';
import Tab from 'primevue/tab';
import TabPanels from 'primevue/tabpanels';
import TabPanel from 'primevue/tabpanel';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Tag from 'primevue/tag';
import { useConfirm } from 'primevue/useconfirm';
import { useMonitorsStore } from '@/stores/monitors.store';
import { useApiError } from '@/composables/useApiError';
import { useAppToast } from '@/composables/useToast';
import * as checksApi from '@/api/checks.api';
import type {
  CheckResponse,
  MonitorUptimeResponse,
  UptimeRange,
} from '@/types/checks';
import PageHeader from '@/components/shared/PageHeader.vue';
import LoadingPanel from '@/components/shared/LoadingPanel.vue';
import MonitorStatusBadge from '@/components/monitors/MonitorStatusBadge.vue';
import LatencyChart from '@/components/monitors/LatencyChart.vue';
import UptimeChart from '@/components/monitors/UptimeChart.vue';

const props = defineProps<{ id: string }>();

const router = useRouter();
const monitors = useMonitorsStore();
const { extract } = useApiError();
const toast = useAppToast();
const confirm = useConfirm();

const loading = ref(false);
const activeTab = ref('overview');

const range = ref<UptimeRange>('24h');
const rangeOptions = [
  { label: '1h', value: '1h' },
  { label: '24h', value: '24h' },
  { label: '7d', value: '7d' },
  { label: '30d', value: '30d' },
];

const uptime = ref<MonitorUptimeResponse | null>(null);
const uptimeLoading = ref(false);

const history = ref<CheckResponse[]>([]);
const historyTotal = ref(0);
const historyLoading = ref(false);

const monitor = computed(() => monitors.byId.get(props.id));
const recentChecks = computed(() => monitors.recentChecks[props.id] ?? []);

onMounted(async () => {
  loading.value = true;
  try {
    await monitors.fetchById(props.id);
    await loadUptime();
    await loadRecentChecks();
  } catch (err) {
    toast.error('Erro ao carregar monitor', extract(err).message);
    router.push({ name: 'monitors' });
  } finally {
    loading.value = false;
  }
});

watch(range, () => {
  loadUptime();
  loadRecentChecks();
});

watch(activeTab, (tab) => {
  if (tab === 'history' && !history.value.length) loadHistory();
});

async function loadUptime() {
  uptimeLoading.value = true;
  try {
    uptime.value = await checksApi.getMonitorUptime(props.id, range.value);
  } catch (err) {
    toast.error('Erro ao carregar uptime', extract(err).message);
  } finally {
    uptimeLoading.value = false;
  }
}

async function loadRecentChecks() {
  try {
    const res = await checksApi.listMonitorChecks(props.id, {
      limit: 50,
    });
    monitors.seedRecentChecks(props.id, res.items);
  } catch (err) {
    toast.error('Erro ao carregar checks', extract(err).message);
  }
}

async function loadHistory() {
  historyLoading.value = true;
  try {
    const res = await checksApi.listMonitorChecks(props.id, { limit: 200 });
    history.value = res.items;
    historyTotal.value = res.total;
  } catch (err) {
    toast.error('Erro ao carregar histórico', extract(err).message);
  } finally {
    historyLoading.value = false;
  }
}

function format(value: string | null | undefined): string {
  if (!value) return '-';
  return dayjs(value).format('DD/MM/YYYY HH:mm:ss');
}

function resultSeverity(result: CheckResponse['result']) {
  switch (result) {
    case 'Success': return 'success';
    case 'Degraded': return 'warn';
    case 'Failure':
    case 'Timeout': return 'danger';
    default: return 'secondary';
  }
}

async function togglePause() {
  if (!monitor.value) return;
  try {
    if (monitor.value.isPaused) await monitors.resume(monitor.value.id);
    else await monitors.pause(monitor.value.id);
    toast.success(monitor.value.isPaused ? 'Monitor pausado' : 'Monitor retomado');
  } catch (err) {
    toast.error('Erro', extract(err).message);
  }
}

function confirmRemove() {
  if (!monitor.value) return;
  const id = monitor.value.id;
  const name = monitor.value.name;
  confirm.require({
    message: `Excluir o monitor "${name}"?`,
    header: 'Confirmar exclusão',
    icon: 'pi pi-exclamation-triangle',
    acceptLabel: 'Excluir',
    rejectLabel: 'Cancelar',
    acceptClass: 'p-button-danger',
    accept: async () => {
      try {
        await monitors.remove(id);
        toast.success('Monitor excluído');
        router.push({ name: 'monitors' });
      } catch (err) {
        toast.error('Erro ao excluir', extract(err).message);
      }
    },
  });
}
</script>

<template>
  <LoadingPanel v-if="loading || !monitor" message="Carregando monitor..." />

  <template v-else>
    <PageHeader :title="monitor.name" :subtitle="monitor.url">
      <template #actions>
        <Button
          label="Voltar"
          icon="pi pi-arrow-left"
          severity="secondary"
          text
          @click="router.push({ name: 'monitors' })"
        />
        <Button
          label="Editar"
          icon="pi pi-pencil"
          severity="secondary"
          @click="router.push({ name: 'monitor-edit', params: { id: monitor.id } })"
        />
        <Button
          :label="monitor.isPaused ? 'Retomar' : 'Pausar'"
          :icon="monitor.isPaused ? 'pi pi-play' : 'pi pi-pause'"
          severity="warn"
          @click="togglePause"
        />
        <Button label="Excluir" icon="pi pi-trash" severity="danger" @click="confirmRemove" />
      </template>
    </PageHeader>

    <div class="status-row">
      <MonitorStatusBadge :status="monitor.status" />
      <span class="text-muted">
        Última verificação: {{ format(monitor.lastCheckedAt) }}
      </span>
    </div>

    <Tabs v-model:value="activeTab" class="tabs">
      <TabList>
        <Tab value="overview">Visão geral</Tab>
        <Tab value="history">Histórico</Tab>
        <Tab value="config">Configurações</Tab>
      </TabList>

      <TabPanels>
        <TabPanel value="overview">
          <div class="overview-toolbar">
            <SelectButton
              v-model="range"
              :options="rangeOptions"
              option-label="label"
              option-value="value"
              :allow-empty="false"
            />
            <Button
              label="Atualizar"
              icon="pi pi-refresh"
              text
              severity="secondary"
              @click="loadUptime"
            />
          </div>

          <section class="kpis">
            <Card class="kpi">
              <template #content>
                <div class="kpi-label">Uptime</div>
                <div class="kpi-value">
                  {{ uptime ? `${uptime.uptimePercent.toFixed(2)}%` : '-' }}
                </div>
              </template>
            </Card>
            <Card class="kpi">
              <template #content>
                <div class="kpi-label">P50</div>
                <div class="kpi-value">
                  {{ uptime?.p50LatencyMs != null ? `${uptime.p50LatencyMs} ms` : '-' }}
                </div>
              </template>
            </Card>
            <Card class="kpi">
              <template #content>
                <div class="kpi-label">P95</div>
                <div class="kpi-value">
                  {{ uptime?.p95LatencyMs != null ? `${uptime.p95LatencyMs} ms` : '-' }}
                </div>
              </template>
            </Card>
            <Card class="kpi">
              <template #content>
                <div class="kpi-label">P99</div>
                <div class="kpi-value">
                  {{ uptime?.p99LatencyMs != null ? `${uptime.p99LatencyMs} ms` : '-' }}
                </div>
              </template>
            </Card>
            <Card class="kpi">
              <template #content>
                <div class="kpi-label">Total de checks</div>
                <div class="kpi-value">{{ uptime?.total ?? 0 }}</div>
                <div class="kpi-sub">
                  <span class="ok">{{ uptime?.successful ?? 0 }} ok</span>
                  ·
                  <span class="warn">{{ uptime?.degraded ?? 0 }} deg</span>
                  ·
                  <span class="bad">{{ uptime?.failed ?? 0 }} falha</span>
                </div>
              </template>
            </Card>
          </section>

          <Card class="card">
            <template #title>Linha de uptime</template>
            <template #content>
              <UptimeChart :checks="recentChecks" />
            </template>
          </Card>

          <Card class="card">
            <template #title>Latência</template>
            <template #content>
              <LatencyChart :checks="recentChecks" />
            </template>
          </Card>
        </TabPanel>

        <TabPanel value="history">
          <LoadingPanel v-if="historyLoading" />
          <DataTable
            v-else
            :value="history"
            data-key="id"
            :paginator="history.length > 25"
            :rows="25"
            striped-rows
          >
            <Column field="timestamp" header="Data/hora">
              <template #body="{ data }">{{ format(data.timestamp) }}</template>
            </Column>
            <Column field="result" header="Resultado">
              <template #body="{ data }">
                <Tag :severity="resultSeverity(data.result)" :value="data.result" />
              </template>
            </Column>
            <Column field="statusCode" header="HTTP">
              <template #body="{ data }">{{ data.statusCode ?? '-' }}</template>
            </Column>
            <Column field="latencyMs" header="Latência">
              <template #body="{ data }">{{ data.latencyMs }} ms</template>
            </Column>
            <Column field="errorMessage" header="Erro">
              <template #body="{ data }">
                <span class="text-muted">{{ data.errorMessage ?? '-' }}</span>
              </template>
            </Column>
          </DataTable>
        </TabPanel>

        <TabPanel value="config">
          <Card>
            <template #content>
              <div class="config-grid">
                <div class="info">
                  <span class="label">Método HTTP</span>
                  <span>{{ monitor.config.httpMethod }}</span>
                </div>
                <div class="info">
                  <span class="label">Intervalo</span>
                  <span>{{ monitor.config.intervalSeconds }}s</span>
                </div>
                <div class="info">
                  <span class="label">Timeout</span>
                  <span>{{ monitor.config.timeoutMs }}ms</span>
                </div>
                <div class="info">
                  <span class="label">Latência degradado</span>
                  <span>{{ monitor.config.degradedLatencyMs }}ms</span>
                </div>
                <div class="info">
                  <span class="label">Falhas p/ abrir incidente</span>
                  <span>{{ monitor.config.failuresToOpenIncident }}</span>
                </div>
                <div class="info">
                  <span class="label">Sucessos p/ fechar</span>
                  <span>{{ monitor.config.successesToCloseIncident }}</span>
                </div>
                <div class="info">
                  <span class="label">Status code esperado</span>
                  <span>{{ monitor.config.expectedStatusCode ?? 'qualquer 2xx' }}</span>
                </div>
                <div class="info">
                  <span class="label">Criado em</span>
                  <span>{{ format(monitor.createdAt) }}</span>
                </div>
              </div>
            </template>
          </Card>
        </TabPanel>
      </TabPanels>
    </Tabs>
  </template>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.status-row {
  display: flex;
  align-items: center;
  gap: $space-md;
  margin-bottom: $space-lg;
  flex-wrap: wrap;
}

.text-muted {
  color: $color-text-muted;
  font-size: 0.875rem;
}

.tabs {
  margin-top: $space-md;
}

.overview-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: $space-md;
  margin: $space-md 0;
  flex-wrap: wrap;
}

.kpis {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
  gap: $space-md;
  margin-bottom: $space-md;
}

.kpi-label {
  color: $color-text-muted;
  font-size: 0.8rem;
}

.kpi-value {
  font-size: 1.5rem;
  font-weight: 700;
  margin-top: $space-xs;
}

.kpi-sub {
  margin-top: $space-xs;
  font-size: 0.75rem;

  .ok { color: $color-up; }
  .warn { color: $color-degraded; }
  .bad { color: $color-down; }
}

.card {
  margin-bottom: $space-md;
}

.config-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: $space-md;
}

.info {
  display: flex;
  flex-direction: column;
  gap: $space-xs;
  padding: $space-sm;
  border: 1px solid $color-border;
  border-radius: $radius-md;
  background: $color-bg;
}

.label {
  color: $color-text-muted;
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.04em;
}
</style>
