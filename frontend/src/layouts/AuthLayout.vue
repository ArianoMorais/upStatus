<script setup lang="ts"></script>

<template>
  <main class="auth-layout">
    <RouterView />
  </main>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.auth-layout {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: $space-md;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
}
</style>
