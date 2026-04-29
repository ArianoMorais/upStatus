import { computed, ref } from 'vue';
import { defineStore } from 'pinia';
import * as authApi from '@/api/auth.api';
import type { AuthUserResponse } from '@/types/auth';

const TOKEN_KEY = 'upstatus.token';
const USER_KEY = 'upstatus.user';

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(null);
  const user = ref<AuthUserResponse | null>(null);

  const isAuthenticated = computed(() => !!token.value);

  function loadFromStorage(): void {
    const storedToken = localStorage.getItem(TOKEN_KEY);
    const storedUser = localStorage.getItem(USER_KEY);
    if (storedToken) {
      token.value = storedToken;
    }
    if (storedUser) {
      try {
        user.value = JSON.parse(storedUser) as AuthUserResponse;
      } catch {
        user.value = null;
      }
    }
  }

  async function login(email: string, password: string): Promise<void> {
    const res = await authApi.login({ email, password });
    token.value = res.accessToken;
    user.value = res.user;
    localStorage.setItem(TOKEN_KEY, res.accessToken);
    localStorage.setItem(USER_KEY, JSON.stringify(res.user));
  }

  async function fetchMe(): Promise<void> {
    if (!token.value) return;
    const me = await authApi.getMe();
    user.value = me;
    localStorage.setItem(USER_KEY, JSON.stringify(me));
  }

  function logout(): void {
    token.value = null;
    user.value = null;
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }

  return {
    token,
    user,
    isAuthenticated,
    loadFromStorage,
    login,
    fetchMe,
    logout,
  };
});
