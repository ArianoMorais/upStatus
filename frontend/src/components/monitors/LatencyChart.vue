<script setup lang="ts">
import { computed } from 'vue';
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Tooltip,
  Legend,
  Filler,
} from 'chart.js';
import { Line } from 'vue-chartjs';
import dayjs from 'dayjs';
import type { CheckResponse } from '@/types/checks';

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Tooltip,
  Legend,
  Filler,
);

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

const chartData = computed(() => ({
  labels: props.checks.map((c) => dayjs(c.timestamp).format('HH:mm:ss')),
  datasets: [
    {
      label: 'Latência (ms)',
      data: props.checks.map((c) => c.latencyMs),
      borderColor: '#2563eb',
      backgroundColor: 'rgba(37, 99, 235, 0.1)',
      fill: true,
      tension: 0.25,
      pointRadius: 3,
      pointBackgroundColor: props.checks.map((c) => colorFor(c.result)),
      pointBorderColor: props.checks.map((c) => colorFor(c.result)),
    },
  ],
}));

const options = {
  responsive: true,
  maintainAspectRatio: false,
  interaction: { mode: 'index' as const, intersect: false },
  plugins: {
    legend: { display: false },
    tooltip: {
      callbacks: {
        afterLabel: (ctx: { dataIndex: number }) => {
          const c = props.checks[ctx.dataIndex];
          if (!c) return '';
          const lines = [`Resultado: ${c.result}`];
          if (c.statusCode !== null && c.statusCode !== undefined) {
            lines.push(`HTTP: ${c.statusCode}`);
          }
          if (c.errorMessage) lines.push(`Erro: ${c.errorMessage}`);
          return lines;
        },
      },
    },
  },
  scales: {
    y: { beginAtZero: true, ticks: { callback: (v: number | string) => `${v}ms` } },
    x: { ticks: { maxRotation: 0, autoSkipPadding: 16 } },
  },
};
</script>

<template>
  <div class="chart">
    <Line v-if="checks.length" :data="chartData" :options="options" />
    <div v-else class="empty">Sem dados de latência ainda.</div>
  </div>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.chart {
  position: relative;
  height: 280px;
}

.empty {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: $color-text-muted;
}
</style>
