import { createApp } from 'vue';
import { createPinia } from 'pinia';
import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura';
import ToastService from 'primevue/toastservice';
import ConfirmationService from 'primevue/confirmationservice';

import 'primeicons/primeicons.css';
import './styles/main.scss';

import App from './App.vue';
import router from './router';
import { configureHttp } from './api/http';
import { useAuthStore } from './stores/auth.store';

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      darkModeSelector: '.dark',
    },
  },
});
app.use(ToastService);
app.use(ConfirmationService);

const auth = useAuthStore();
auth.loadFromStorage();

configureHttp({
  getToken: () => auth.token,
  onUnauthorized: () => {
    auth.logout();
    if (router.currentRoute.value.name !== 'login') {
      router.push({ name: 'login' });
    }
  },
});

app.use(router);

app.mount('#app');
