<script setup lang="ts">
import { onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import Button from 'primevue/button';
import { useAuthStore } from '@/stores/auth.store';
import { useIncidentsStore } from '@/stores/incidents.store';
import { useMonitorsStore } from '@/stores/monitors.store';
import RealtimeIndicator from '@/components/shared/RealtimeIndicator.vue';

const auth = useAuthStore();
const incidents = useIncidentsStore();
const monitors = useMonitorsStore();
const router = useRouter();
const sidebarOpen = ref(false);

onMounted(async () => {
  await Promise.allSettled([incidents.fetchOpen(), monitors.fetchAll()]);
});

function logout() {
  auth.logout();
  router.push({ name: 'login' });
}

function toggleSidebar() {
  sidebarOpen.value = !sidebarOpen.value;
}

function closeSidebar() {
  sidebarOpen.value = false;
}
</script>

<template>
  <div class="layout">
    <header class="header">
      <div class="header-left">
        <Button
          icon="pi pi-bars"
          text
          rounded
          aria-label="Menu"
          class="menu-toggle"
          @click="toggleSidebar"
        />
        <RouterLink to="/" class="brand">
          <i class="pi pi-bolt" />
          <span>UpStatus</span>
        </RouterLink>
      </div>
      <div class="header-right">
        <RealtimeIndicator />
        <span v-if="auth.user" class="user-name">{{ auth.user.name }}</span>
        <Button
          icon="pi pi-sign-out"
          severity="secondary"
          text
          rounded
          aria-label="Sair"
          @click="logout"
        />
      </div>
    </header>

    <aside class="sidebar" :class="{ open: sidebarOpen }">
      <nav>
        <RouterLink :to="{ name: 'dashboard' }" class="nav-item" @click="closeSidebar">
          <i class="pi pi-home" /> <span>Dashboard</span>
        </RouterLink>
        <RouterLink :to="{ name: 'monitors' }" class="nav-item" @click="closeSidebar">
          <i class="pi pi-globe" /> <span>Monitores</span>
        </RouterLink>
        <RouterLink :to="{ name: 'incidents' }" class="nav-item" @click="closeSidebar">
          <i class="pi pi-exclamation-triangle" />
          <span class="nav-label">Incidentes</span>
          <span v-if="incidents.openCount" class="badge">{{ incidents.openCount }}</span>
        </RouterLink>
      </nav>
    </aside>

    <div v-if="sidebarOpen" class="overlay" @click="closeSidebar"></div>

    <main class="content">
      <RouterView />
    </main>
  </div>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;
@use '@/styles/mixins' as *;

.layout {
  display: grid;
  grid-template-columns: 240px 1fr;
  grid-template-rows: 60px 1fr;
  grid-template-areas:
    'header header'
    'sidebar content';
  min-height: 100vh;
}

.header {
  grid-area: header;
  background: $color-surface;
  border-bottom: 1px solid $color-border;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 $space-lg;
  position: sticky;
  top: 0;
  z-index: 30;
}

.header-left,
.header-right {
  display: flex;
  align-items: center;
  gap: $space-md;
}

.brand {
  display: flex;
  align-items: center;
  gap: $space-sm;
  font-weight: 600;
  font-size: 1.1rem;
  color: $color-text;
  &:hover {
    text-decoration: none;
  }
  i {
    color: $color-primary;
  }
}

.user-name {
  color: $color-text-muted;
  font-size: 0.875rem;
}

.menu-toggle {
  display: none;
}

.sidebar {
  grid-area: sidebar;
  background: $color-surface;
  border-right: 1px solid $color-border;
  padding: $space-md $space-sm;
  position: sticky;
  top: 60px;
  align-self: start;
  height: calc(100vh - 60px);
  overflow-y: auto;
}

nav {
  display: flex;
  flex-direction: column;
  gap: $space-xs;
}

.nav-label {
  flex: 1;
}

.badge {
  background: $color-down;
  color: white;
  font-size: 0.7rem;
  font-weight: 700;
  border-radius: 999px;
  padding: 2px 7px;
  min-width: 18px;
  text-align: center;
}

.nav-item {
  display: flex;
  align-items: center;
  gap: $space-sm;
  padding: $space-sm $space-md;
  border-radius: $radius-md;
  color: $color-text;
  font-weight: 500;

  &:hover {
    background: $color-bg;
    text-decoration: none;
  }

  &.router-link-active {
    background: rgba(37, 99, 235, 0.08);
    color: $color-primary;
  }

  i {
    font-size: 1rem;
  }
}

.content {
  grid-area: content;
  padding: $space-lg;
  overflow-x: hidden;
}

.overlay {
  display: none;
}

@include md-down {
  .layout {
    grid-template-columns: 1fr;
    grid-template-areas:
      'header'
      'content';
  }
  .menu-toggle {
    display: inline-flex;
  }
  .sidebar {
    position: fixed;
    top: 60px;
    left: 0;
    bottom: 0;
    width: 240px;
    height: calc(100vh - 60px);
    transform: translateX(-100%);
    transition: transform 0.2s ease;
    z-index: 25;
    &.open {
      transform: translateX(0);
    }
  }
  .overlay {
    display: block;
    position: fixed;
    inset: 60px 0 0 0;
    background: rgba(0, 0, 0, 0.4);
    z-index: 20;
  }
}
</style>
