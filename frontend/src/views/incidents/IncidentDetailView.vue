<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import dayjs from 'dayjs';
import Card from 'primevue/card';
import Button from 'primevue/button';
import Textarea from 'primevue/textarea';
import Dialog from 'primevue/dialog';
import { useIncidentsStore } from '@/stores/incidents.store';
import { useMonitorsStore } from '@/stores/monitors.store';
import { useApiError } from '@/composables/useApiError';
import { useAppToast } from '@/composables/useToast';
import PageHeader from '@/components/shared/PageHeader.vue';
import LoadingPanel from '@/components/shared/LoadingPanel.vue';
import IncidentStatusBadge from '@/components/incidents/IncidentStatusBadge.vue';
import IncidentCommentList from '@/components/incidents/IncidentCommentList.vue';

const props = defineProps<{ id: string }>();

const router = useRouter();
const store = useIncidentsStore();
const monitors = useMonitorsStore();
const { extract } = useApiError();
const toast = useAppToast();

const loading = ref(false);
const comment = ref('');
const submitting = ref(false);
const showResolve = ref(false);
const resolveComment = ref('');
const resolving = ref(false);

const incident = computed(() => store.selected);
const monitor = computed(() =>
  incident.value ? monitors.byId.get(incident.value.monitorId) : null,
);

const isOpen = computed(() => incident.value?.status === 'Open');
const isResolved = computed(() => incident.value?.status === 'Resolved');

onMounted(async () => {
  loading.value = true;
  try {
    await Promise.all([store.fetchById(props.id), monitors.fetchAll()]);
  } catch (err) {
    toast.error('Erro', extract(err).message);
    router.push({ name: 'incidents' });
  } finally {
    loading.value = false;
  }
});

function format(value: string | null | undefined) {
  return value ? dayjs(value).format('DD/MM/YYYY HH:mm:ss') : '-';
}

async function acknowledge() {
  if (!incident.value) return;
  try {
    await store.acknowledge(incident.value.id);
    toast.success('Incidente reconhecido');
  } catch (err) {
    toast.error('Erro', extract(err).message);
  }
}

async function submitComment() {
  if (!incident.value || !comment.value.trim()) return;
  submitting.value = true;
  try {
    await store.addComment(incident.value.id, comment.value.trim());
    comment.value = '';
    toast.success('Comentário adicionado');
  } catch (err) {
    toast.error('Erro', extract(err).message);
  } finally {
    submitting.value = false;
  }
}

async function confirmResolve() {
  if (!incident.value) return;
  resolving.value = true;
  try {
    await store.resolve(
      incident.value.id,
      resolveComment.value.trim() || undefined,
    );
    toast.success('Incidente resolvido');
    showResolve.value = false;
    resolveComment.value = '';
  } catch (err) {
    toast.error('Erro', extract(err).message);
  } finally {
    resolving.value = false;
  }
}
</script>

<template>
  <LoadingPanel v-if="loading || !incident" />

  <template v-else>
    <PageHeader
      :title="`Incidente · ${monitor?.name ?? 'Monitor'}`"
      :subtitle="incident.reason ?? 'Sem motivo informado'"
    >
      <template #actions>
        <Button
          label="Voltar"
          icon="pi pi-arrow-left"
          severity="secondary"
          text
          @click="router.push({ name: 'incidents' })"
        />
        <Button
          v-if="isOpen"
          label="Reconhecer"
          icon="pi pi-eye"
          severity="warn"
          @click="acknowledge"
        />
        <Button
          v-if="!isResolved"
          label="Resolver"
          icon="pi pi-check"
          severity="success"
          @click="showResolve = true"
        />
      </template>
    </PageHeader>

    <div class="header-info">
      <IncidentStatusBadge :status="incident.status" />
      <div class="meta">
        <div><strong>Início:</strong> {{ format(incident.startedAt) }}</div>
        <div v-if="incident.acknowledgedAt">
          <strong>Reconhecido:</strong> {{ format(incident.acknowledgedAt) }}
        </div>
        <div v-if="incident.resolvedAt">
          <strong>Resolvido:</strong> {{ format(incident.resolvedAt) }}
        </div>
      </div>
    </div>

    <Card class="card">
      <template #title>Histórico</template>
      <template #content>
        <IncidentCommentList :comments="incident.comments" />
      </template>
    </Card>

    <Card v-if="!isResolved" class="card">
      <template #title>Adicionar comentário</template>
      <template #content>
        <Textarea
          v-model="comment"
          rows="3"
          placeholder="Descreva o que está sendo feito..."
          fluid
          :disabled="submitting"
        />
        <div class="row-end">
          <Button
            label="Enviar"
            icon="pi pi-send"
            :loading="submitting"
            :disabled="!comment.trim()"
            @click="submitComment"
          />
        </div>
      </template>
    </Card>

    <Dialog
      v-model:visible="showResolve"
      header="Resolver incidente"
      modal
      :style="{ width: 'min(480px, 95vw)' }"
    >
      <p>Deseja adicionar um comentário final ao resolver?</p>
      <Textarea
        v-model="resolveComment"
        rows="3"
        placeholder="Comentário (opcional)..."
        fluid
      />
      <template #footer>
        <Button
          label="Cancelar"
          severity="secondary"
          text
          @click="showResolve = false"
        />
        <Button
          label="Resolver"
          icon="pi pi-check"
          severity="success"
          :loading="resolving"
          @click="confirmResolve"
        />
      </template>
    </Dialog>
  </template>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.header-info {
  display: flex;
  align-items: center;
  gap: $space-lg;
  margin-bottom: $space-lg;
  flex-wrap: wrap;
}

.meta {
  display: flex;
  flex-direction: column;
  gap: $space-xs;
  color: $color-text-muted;
  font-size: 0.875rem;

  strong {
    color: $color-text;
  }
}

.card {
  margin-bottom: $space-md;
}

.row-end {
  display: flex;
  justify-content: flex-end;
  margin-top: $space-sm;
}
</style>
