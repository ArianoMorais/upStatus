import { ref } from 'vue';
import { defineStore } from 'pinia';

export type RealtimeStatus = 'disconnected' | 'connecting' | 'connected' | 'reconnecting';

export const useRealtimeStore = defineStore('realtime', () => {
  const status = ref<RealtimeStatus>('disconnected');

  function setStatus(value: RealtimeStatus): void {
    status.value = value;
  }

  return { status, setStatus };
});
