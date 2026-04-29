<script setup lang="ts">
defineProps<{
  icon?: string;
  title: string;
  description?: string;
}>();
</script>

<template>
  <div class="empty">
    <i :class="['pi', icon ?? 'pi-inbox']" />
    <h3>{{ title }}</h3>
    <p v-if="description">{{ description }}</p>
    <slot />
  </div>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  padding: $space-2xl $space-md;
  color: $color-text-muted;
  gap: $space-sm;

  i {
    font-size: 2.5rem;
    color: $color-text-muted;
  }

  h3 {
    margin: 0;
    color: $color-text;
  }

  p {
    margin: 0;
    max-width: 360px;
  }
}
</style>
