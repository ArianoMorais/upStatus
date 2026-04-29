import type { Router } from 'vue-router';
import { useAuthStore } from '@/stores/auth.store';

export function registerGuards(router: Router): void {
  router.beforeEach((to) => {
    const auth = useAuthStore();
    const requiresAuth = to.matched.some((r) => r.meta.requiresAuth);

    if (requiresAuth && !auth.isAuthenticated) {
      return { name: 'login', query: { redirect: to.fullPath } };
    }

    if (to.name === 'login' && auth.isAuthenticated) {
      return { name: 'dashboard' };
    }

    return true;
  });
}
