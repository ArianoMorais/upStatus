<script setup lang="ts">
import dayjs from 'dayjs';
import type { CheckResponse } from '@/types/checks';

const props = defineProps<{ checks: CheckResponse[] }>();

const colorFor = (result: CheckResponse['result']): string => {
  switch (result) {
    case 'Success': return '#10b981';
    case 'Degraded': return '#f59e0b';
    case 'Failure':
    case 'Timeout': return '#ef4444';
    default: return '#9ca3af';
  }
};

function tooltip(c: CheckResponse): string {
  return `${dayjs(c.timestamp).format('DD/MM HH:mm:ss')} · ${c.result} · ${c.latencyMs}ms`;
}

defineExpose({ props });
</script>

<template>
  <div class="timeline">
    <div
      v-for="c in checks"
      :key="c.id"
      class="bar"
      :style="{ background: colorFor(c.result) }"
      :title="tooltip(c)"
    />
    <div v-if="!checks.length" class="empty">Sem checks ainda.</div>
  </div>
  <div class="legend">
    <span class="dot up" /> Operacional
    <span class="dot degraded" /> Degradado
    <span class="dot down" /> Falha
  </div>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.timeline {
  display: flex;
  align-items: stretch;
  gap: 2px;
  height: 48px;
  padding: $space-sm 0;
}

.bar {
  flex: 1;
  min-width: 4px;
  border-radius: 2px;
  transition: transform 0.15s ease;

  &:hover {
    transform: scaleY(1.1);
  }
}

.empty {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: $color-text-muted;
}

.legend {
  display: flex;
  align-items: center;
  gap: $space-md;
  font-size: 0.75rem;
  color: $color-text-muted;
}

.dot {
  display: inline-block;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  margin-right: 4px;

  &.up { background: $color-up; }
  &.degraded { background: $color-degraded; }
  &.down { background: $color-down; }
}
</style>
