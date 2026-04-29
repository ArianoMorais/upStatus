<script setup lang="ts">
import { watch } from 'vue';
import Toast from 'primevue/toast';
import ConfirmDialog from 'primevue/confirmdialog';
import { useAuthStore } from '@/stores/auth.store';
import { useRealtime } from '@/composables/useRealtime';
import { useMonitorsStore } from '@/stores/monitors.store';
import { useIncidentsStore } from '@/stores/incidents.store';

const auth = useAuthStore();
const realtime = useRealtime();
const monitors = useMonitorsStore();
const incidents = useIncidentsStore();

watch(
  () => auth.isAuthenticated,
  async (isAuth) => {
    if (isAuth) {
      await realtime.start();
    } else {
      await realtime.stop();
      monitors.reset();
      incidents.reset();
    }
  },
  { immediate: true },
);
</script>

<template>
  <RouterView />
  <Toast position="top-right" />
  <ConfirmDialog />
</template>
