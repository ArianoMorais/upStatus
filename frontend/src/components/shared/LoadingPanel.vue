<script setup lang="ts">
import ProgressSpinner from 'primevue/progressspinner';

defineProps<{ message?: string }>();
</script>

<template>
  <div class="loading">
    <ProgressSpinner style="width: 40px; height: 40px" stroke-width="4" />
    <span v-if="message">{{ message }}</span>
  </div>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: $space-2xl $space-md;
  gap: $space-sm;
  color: $color-text-muted;
}
</style>
