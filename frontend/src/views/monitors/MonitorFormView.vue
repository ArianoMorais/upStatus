<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue';
import { useRouter } from 'vue-router';
import Card from 'primevue/card';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import InputNumber from 'primevue/inputnumber';
import Select from 'primevue/select';
import Panel from 'primevue/panel';
import { useMonitorsStore } from '@/stores/monitors.store';
import { useApiError } from '@/composables/useApiError';
import { useAppToast } from '@/composables/useToast';
import { DEFAULT_MONITOR_CONFIG, type MonitorConfig } from '@/types/monitors';
import PageHeader from '@/components/shared/PageHeader.vue';
import LoadingPanel from '@/components/shared/LoadingPanel.vue';

const props = defineProps<{ id?: string }>();

const router = useRouter();
const monitors = useMonitorsStore();
const { extract } = useApiError();
const toast = useAppToast();

const isEdit = computed(() => !!props.id);
const loading = ref(false);
const submitting = ref(false);

const httpMethods = [
  { label: 'GET', value: 'GET' },
  { label: 'HEAD', value: 'HEAD' },
  { label: 'POST', value: 'POST' },
];

const form = reactive<{ name: string; url: string; config: MonitorConfig }>({
  name: '',
  url: '',
  config: { ...DEFAULT_MONITOR_CONFIG },
});

const errors = reactive<Record<string, string>>({});

onMounted(async () => {
  if (!isEdit.value) return;
  loading.value = true;
  try {
    const monitor = await monitors.fetchById(props.id!);
    form.name = monitor.name;
    form.url = monitor.url;
    form.config = { ...monitor.config };
  } catch (err) {
    toast.error('Erro ao carregar monitor', extract(err).message);
    router.push({ name: 'monitors' });
  } finally {
    loading.value = false;
  }
});

function validate(): boolean {
  for (const k of Object.keys(errors)) delete errors[k];
  if (!form.name.trim()) errors.name = 'Informe o nome.';
  if (!form.url.trim()) errors.url = 'Informe a URL.';
  else if (!/^https?:\/\/.+/i.test(form.url.trim()))
    errors.url = 'URL deve começar com http:// ou https://';
  if (form.config.intervalSeconds < 10)
    errors.intervalSeconds = 'Intervalo mínimo de 10 segundos.';
  if (form.config.timeoutMs < 100) errors.timeoutMs = 'Timeout mínimo de 100ms.';
  if (form.config.failuresToOpenIncident < 1) errors.failuresToOpenIncident = 'Mínimo 1.';
  if (form.config.successesToCloseIncident < 1) errors.successesToCloseIncident = 'Mínimo 1.';
  if (form.config.degradedLatencyMs < 1) errors.degradedLatencyMs = 'Mínimo 1ms.';
  return Object.keys(errors).length === 0;
}

async function onSubmit() {
  if (!validate()) return;
  submitting.value = true;
  try {
    const payload = {
      name: form.name.trim(),
      url: form.url.trim(),
      config: { ...form.config },
    };
    if (isEdit.value) {
      await monitors.update(props.id!, payload);
      toast.success('Monitor atualizado');
    } else {
      await monitors.create(payload);
      toast.success('Monitor criado');
    }
    router.push({ name: 'monitors' });
  } catch (err) {
    const apiErr = extract(err);
    toast.error('Erro ao salvar', apiErr.message);
    if (apiErr.fieldErrors) {
      for (const [field, msgs] of Object.entries(apiErr.fieldErrors)) {
        const key = field.charAt(0).toLowerCase() + field.slice(1);
        errors[key] = msgs[0] ?? 'Inválido';
      }
    }
  } finally {
    submitting.value = false;
  }
}
</script>

<template>
  <PageHeader
    :title="isEdit ? 'Editar monitor' : 'Novo monitor'"
    subtitle="Configure o que e como monitorar"
  >
    <template #actions>
      <Button
        label="Cancelar"
        severity="secondary"
        text
        @click="router.push({ name: 'monitors' })"
      />
    </template>
  </PageHeader>

  <LoadingPanel v-if="loading" message="Carregando monitor..." />

  <form v-else class="form" @submit.prevent="onSubmit">
    <Card>
      <template #title>Dados básicos</template>
      <template #content>
        <div class="grid">
          <div class="field">
            <label for="name">Nome *</label>
            <InputText id="name" v-model="form.name" :invalid="!!errors.name" fluid />
            <small v-if="errors.name" class="error">{{ errors.name }}</small>
          </div>
          <div class="field">
            <label for="url">URL *</label>
            <InputText
              id="url"
              v-model="form.url"
              placeholder="https://exemplo.com/health"
              :invalid="!!errors.url"
              fluid
            />
            <small v-if="errors.url" class="error">{{ errors.url }}</small>
          </div>
        </div>
      </template>
    </Card>

    <Panel header="Configurações avançadas" toggleable :collapsed="!isEdit">
      <div class="grid">
        <div class="field">
          <label>Método HTTP</label>
          <Select
            v-model="form.config.httpMethod"
            :options="httpMethods"
            option-label="label"
            option-value="value"
            fluid
          />
        </div>
        <div class="field">
          <label>Intervalo (segundos)</label>
          <InputNumber
            v-model="form.config.intervalSeconds"
            :min="10"
            :invalid="!!errors.intervalSeconds"
            fluid
          />
          <small v-if="errors.intervalSeconds" class="error">{{ errors.intervalSeconds }}</small>
        </div>
        <div class="field">
          <label>Timeout (ms)</label>
          <InputNumber
            v-model="form.config.timeoutMs"
            :min="100"
            :step="100"
            :invalid="!!errors.timeoutMs"
            fluid
          />
          <small v-if="errors.timeoutMs" class="error">{{ errors.timeoutMs }}</small>
        </div>
        <div class="field">
          <label>Latência para "degradado" (ms)</label>
          <InputNumber
            v-model="form.config.degradedLatencyMs"
            :min="1"
            :invalid="!!errors.degradedLatencyMs"
            fluid
          />
          <small v-if="errors.degradedLatencyMs" class="error">
            {{ errors.degradedLatencyMs }}
          </small>
        </div>
        <div class="field">
          <label>Falhas para abrir incidente</label>
          <InputNumber
            v-model="form.config.failuresToOpenIncident"
            :min="1"
            :invalid="!!errors.failuresToOpenIncident"
            fluid
          />
          <small v-if="errors.failuresToOpenIncident" class="error">
            {{ errors.failuresToOpenIncident }}
          </small>
        </div>
        <div class="field">
          <label>Sucessos para fechar incidente</label>
          <InputNumber
            v-model="form.config.successesToCloseIncident"
            :min="1"
            :invalid="!!errors.successesToCloseIncident"
            fluid
          />
          <small v-if="errors.successesToCloseIncident" class="error">
            {{ errors.successesToCloseIncident }}
          </small>
        </div>
        <div class="field">
          <label>Status code esperado (opcional)</label>
          <InputNumber
            v-model="form.config.expectedStatusCode"
            :use-grouping="false"
            placeholder="Ex.: 200"
            fluid
          />
        </div>
      </div>
    </Panel>

    <div class="form-actions">
      <Button
        type="button"
        label="Cancelar"
        severity="secondary"
        text
        @click="router.push({ name: 'monitors' })"
      />
      <Button
        type="submit"
        :label="isEdit ? 'Salvar alterações' : 'Criar monitor'"
        :loading="submitting"
      />
    </div>
  </form>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.form {
  display: flex;
  flex-direction: column;
  gap: $space-lg;
}

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: $space-md;
}

.field {
  display: flex;
  flex-direction: column;
  gap: $space-xs;

  label {
    font-weight: 500;
    font-size: 0.875rem;
  }
}

.error {
  color: $color-down;
  font-size: 0.75rem;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: $space-sm;
}
</style>
