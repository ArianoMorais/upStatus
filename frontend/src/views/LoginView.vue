<script setup lang="ts">
import { reactive, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import Card from 'primevue/card';
import InputText from 'primevue/inputtext';
import Password from 'primevue/password';
import Button from 'primevue/button';
import { useAuthStore } from '@/stores/auth.store';
import { useApiError } from '@/composables/useApiError';
import { useAppToast } from '@/composables/useToast';

const auth = useAuthStore();
const router = useRouter();
const route = useRoute();
const { extract } = useApiError();
const toast = useAppToast();

const form = reactive({ email: '', password: '' });
const errors = reactive({ email: '', password: '' });
const submitting = ref(false);

function validate(): boolean {
  errors.email = '';
  errors.password = '';
  let ok = true;
  if (!form.email.trim()) {
    errors.email = 'Informe o e-mail.';
    ok = false;
  }
  if (!form.password) {
    errors.password = 'Informe a senha.';
    ok = false;
  }
  return ok;
}

async function onSubmit() {
  if (!validate()) return;
  submitting.value = true;
  try {
    await auth.login(form.email.trim(), form.password);
    const redirect = (route.query.redirect as string) || '/';
    router.push(redirect);
  } catch (err) {
    const apiErr = extract(err);
    toast.error('Falha no login', apiErr.message);
  } finally {
    submitting.value = false;
  }
}
</script>

<template>
  <Card class="login-card">
    <template #title>
      <div class="title">
        <i class="pi pi-bolt" />
        <span>UpStatus</span>
      </div>
    </template>
    <template #subtitle>
      <span class="subtitle">Entre com sua conta para continuar</span>
    </template>
    <template #content>
      <form class="form" @submit.prevent="onSubmit">
        <div class="field">
          <label for="email">E-mail</label>
          <InputText
            id="email"
            v-model="form.email"
            type="email"
            autocomplete="email"
            :invalid="!!errors.email"
            :disabled="submitting"
          />
          <small v-if="errors.email" class="error">{{ errors.email }}</small>
        </div>
        <div class="field">
          <label for="password">Senha</label>
          <Password
            id="password"
            v-model="form.password"
            :feedback="false"
            toggle-mask
            :input-props="{ autocomplete: 'current-password' }"
            :invalid="!!errors.password"
            :disabled="submitting"
            fluid
          />
          <small v-if="errors.password" class="error">{{ errors.password }}</small>
        </div>
        <Button type="submit" label="Entrar" :loading="submitting" fluid />
      </form>
    </template>
  </Card>
</template>

<style scoped lang="scss">
@use '@/styles/variables' as *;

.login-card {
  width: 100%;
  max-width: 420px;
}

.title {
  display: flex;
  align-items: center;
  gap: $space-sm;
  i {
    color: $color-primary;
  }
}

.subtitle {
  color: $color-text-muted;
  font-size: 0.875rem;
}

.form {
  display: flex;
  flex-direction: column;
  gap: $space-md;
}

.field {
  display: flex;
  flex-direction: column;
  gap: $space-xs;

  label {
    font-weight: 500;
    font-size: 0.875rem;
  }

  :deep(.p-password),
  :deep(.p-password-input) {
    width: 100%;
  }

  :deep(.p-inputtext) {
    width: 100%;
  }
}

.error {
  color: $color-down;
  font-size: 0.75rem;
}
</style>
