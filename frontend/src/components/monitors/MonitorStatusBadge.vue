<script setup lang="ts">
import { computed } from 'vue';
import Tag from 'primevue/tag';
import type { MonitorStatus } from '@/types/monitors';

const props = defineProps<{
  status: MonitorStatus;
  size?: 'sm' | 'md';
}>();

const meta = computed(() => {
  switch (props.status) {
    case 'Up':
      return { label: 'Operacional', icon: 'pi-check-circle', cls: 'status-up' };
    case 'Down':
      return { label: 'Fora do ar', icon: 'pi-times-circle', cls: 'status-down' };
    case 'Degraded':
      return { label: 'Degradado', icon: 'pi-exclamation-triangle', cls: 'status-degraded' };
    case 'Paused':
      return { label: 'Pausado', icon: 'pi-pause-circle', cls: 'status-paused' };
    case 'Unknown':
    default:
      return { label: 'Aguardando', icon: 'pi-question-circle', cls: 'status-unknown' };
  }
});
</script>

<template>
  <Tag :class="['status-tag', meta.cls, size === 'sm' ? 'sm' : '']">
    <i :class="['pi', meta.icon]" />
    <span>{{ meta.label }}</span>
  </Tag>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.status-tag {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 10px;
  border-radius: 999px;
  font-weight: 600;
  font-size: 0.8rem;

  i {
    font-size: 0.85rem;
  }

  &.sm {
    padding: 2px 8px;
    font-size: 0.7rem;
  }
}

.status-up {
  background: rgba(16, 185, 129, 0.12);
  color: $color-up;
}
.status-down {
  background: rgba(239, 68, 68, 0.12);
  color: $color-down;
}
.status-degraded {
  background: rgba(245, 158, 11, 0.15);
  color: $color-degraded;
}
.status-paused {
  background: rgba(107, 114, 128, 0.15);
  color: $color-paused;
}
.status-unknown {
  background: rgba(148, 163, 184, 0.18);
  color: $color-unknown;
}
</style>
