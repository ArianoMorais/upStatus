<script setup lang="ts">
import { computed } from 'vue';
import { useRealtimeStore } from '@/stores/realtime.store';

const realtime = useRealtimeStore();

const meta = computed(() => {
  switch (realtime.status) {
    case 'connected':
      return { cls: 'on', label: 'Realtime ativo' };
    case 'connecting':
    case 'reconnecting':
      return { cls: 'pending', label: 'Conectando...' };
    case 'disconnected':
    default:
      return { cls: 'off', label: 'Sem conexão realtime' };
  }
});
</script>

<template>
  <div class="indicator" :title="meta.label">
    <span :class="['dot', meta.cls]" />
    <span class="label">{{ meta.label }}</span>
  </div>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;
@use '@/styles/mixins' as *;

.indicator {
  display: inline-flex;
  align-items: center;
  gap: $space-xs;
  font-size: 0.8rem;
  color: $color-text-muted;
}

.dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  display: inline-block;

  &.on {
    background: $color-up;
    box-shadow: 0 0 0 4px rgba(16, 185, 129, 0.18);
    animation: pulse 1.6s ease-in-out infinite;
  }
  &.pending {
    background: $color-degraded;
    animation: pulse 1.2s ease-in-out infinite;
  }
  &.off {
    background: $color-down;
  }
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.4; }
}

@include md-down {
  .label {
    display: none;
  }
}
</style>
