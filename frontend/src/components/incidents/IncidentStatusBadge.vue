<script setup lang="ts">
import { computed } from 'vue';
import Tag from 'primevue/tag';
import type { IncidentStatus } from '@/types/incidents';

const props = defineProps<{ status: IncidentStatus }>();

const meta = computed(() => {
  switch (props.status) {
    case 'Open':
      return { label: 'Aberto', icon: 'pi-exclamation-circle', cls: 'open' };
    case 'Acknowledged':
      return { label: 'Reconhecido', icon: 'pi-eye', cls: 'ack' };
    case 'Resolved':
      return { label: 'Resolvido', icon: 'pi-check-circle', cls: 'resolved' };
    default:
      return { label: props.status, icon: 'pi-info-circle', cls: 'open' };
  }
});
</script>

<template>
  <Tag :class="['incident-tag', meta.cls]">
    <i :class="['pi', meta.icon]" />
    <span>{{ meta.label }}</span>
  </Tag>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.incident-tag {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 10px;
  border-radius: 999px;
  font-weight: 600;
  font-size: 0.8rem;

  i { font-size: 0.85rem; }
}

.open {
  background: rgba(239, 68, 68, 0.12);
  color: $color-down;
}
.ack {
  background: rgba(245, 158, 11, 0.15);
  color: $color-degraded;
}
.resolved {
  background: rgba(16, 185, 129, 0.12);
  color: $color-up;
}
</style>
