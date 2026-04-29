import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';
import AuthLayout from '@/layouts/AuthLayout.vue';
import DefaultLayout from '@/layouts/DefaultLayout.vue';
import { registerGuards } from './guards';

const routes: RouteRecordRaw[] = [
  {
    path: '/login',
    component: AuthLayout,
    children: [
      {
        path: '',
        name: 'login',
        component: () => import('@/views/LoginView.vue'),
      },
    ],
  },
  {
    path: '/',
    component: DefaultLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'dashboard',
        component: () => import('@/views/DashboardView.vue'),
      },
      {
        path: 'monitors',
        name: 'monitors',
        component: () => import('@/views/monitors/MonitorsListView.vue'),
      },
      {
        path: 'monitors/new',
        name: 'monitor-new',
        component: () => import('@/views/monitors/MonitorFormView.vue'),
      },
      {
        path: 'monitors/:id/edit',
        name: 'monitor-edit',
        component: () => import('@/views/monitors/MonitorFormView.vue'),
        props: true,
      },
      {
        path: 'monitors/:id',
        name: 'monitor-detail',
        component: () => import('@/views/monitors/MonitorDetailView.vue'),
        props: true,
      },
      {
        path: 'incidents',
        name: 'incidents',
        component: () => import('@/views/incidents/IncidentsListView.vue'),
      },
      {
        path: 'incidents/:id',
        name: 'incident-detail',
        component: () => import('@/views/incidents/IncidentDetailView.vue'),
        props: true,
      },
    ],
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/',
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

registerGuards(router);

export default router;
