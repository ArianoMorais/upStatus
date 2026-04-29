<script setup lang="ts">
defineProps<{
  title: string;
  subtitle?: string;
}>();
</script>

<template>
  <header class="page-header">
    <div class="texts">
      <h1>{{ title }}</h1>
      <p v-if="subtitle">{{ subtitle }}</p>
    </div>
    <div class="actions">
      <slot name="actions" />
    </div>
  </header>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.page-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  gap: $space-md;
  margin-bottom: $space-lg;
  flex-wrap: wrap;
}

h1 {
  margin: 0;
  font-size: 1.5rem;
}

p {
  margin: $space-xs 0 0 0;
  color: $color-text-muted;
}

.actions {
  display: flex;
  gap: $space-sm;
}
</style>
