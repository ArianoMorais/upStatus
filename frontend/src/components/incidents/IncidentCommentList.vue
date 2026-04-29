<script setup lang="ts">
import dayjs from 'dayjs';
import type { IncidentCommentResponse } from '@/types/incidents';

defineProps<{ comments: IncidentCommentResponse[] }>();
</script>

<template>
  <ul v-if="comments.length" class="list">
    <li v-for="(c, i) in comments" :key="`${c.authorId}-${c.createdAt}-${i}`" class="item">
      <div class="head">
        <strong>{{ c.authorName || 'Usuário' }}</strong>
        <span class="when">{{ dayjs(c.createdAt).format('DD/MM/YYYY HH:mm') }}</span>
      </div>
      <p class="msg">{{ c.message }}</p>
    </li>
  </ul>
  <p v-else class="empty">Nenhum comentário ainda.</p>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: $space-sm;
}

.item {
  background: $color-bg;
  border: 1px solid $color-border;
  border-radius: $radius-md;
  padding: $space-sm $space-md;
}

.head {
  display: flex;
  justify-content: space-between;
  align-items: baseline;
  gap: $space-sm;
  margin-bottom: 2px;
}

.when {
  color: $color-text-muted;
  font-size: 0.75rem;
}

.msg {
  margin: 0;
  white-space: pre-wrap;
}

.empty {
  color: $color-text-muted;
  font-size: 0.875rem;
  margin: 0;
}
</style>
