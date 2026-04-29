import axios from 'axios';
import type { ProblemDetails } from '@/types/problem';

const CODE_MESSAGES: Record<string, string> = {
  'auth.invalid_credentials': 'E-mail ou senha inválidos.',
  'auth.user_inactive': 'Usuário inativo.',
  'auth.unauthorized': 'Sessão expirada. Faça login novamente.',
  'monitor.not_found': 'Monitor não encontrado.',
  'monitor.duplicate_url': 'Já existe um monitor com essa URL.',
  'monitor.invalid_url': 'URL inválida.',
  'monitor.invalid_config': 'Configuração inválida.',
  'incident.not_found': 'Incidente não encontrado.',
  'incident.already_resolved': 'Incidente já está resolvido.',
  'incident.already_acknowledged': 'Incidente já foi reconhecido.',
  'validation.failed': 'Verifique os campos do formulário.',
};

export interface ApiError {
  code?: string;
  status?: number;
  message: string;
  fieldErrors?: Record<string, string[]>;
}

export function useApiError() {
  function extract(error: unknown): ApiError {
    if (axios.isAxiosError<ProblemDetails>(error)) {
      const data = error.response?.data;
      const code = (data?.code as string | undefined) ?? undefined;
      const status = error.response?.status;
      const fieldErrors = data?.errors as Record<string, string[]> | undefined;

      const friendly = code ? CODE_MESSAGES[code] : undefined;
      const message =
        friendly ??
        data?.detail ??
        data?.title ??
        (status === 401
          ? 'Não autorizado.'
          : status === 403
            ? 'Você não tem permissão para essa ação.'
            : status === 404
              ? 'Recurso não encontrado.'
              : 'Erro ao se comunicar com o servidor.');

      return { code, status, message, fieldErrors };
    }

    if (error instanceof Error) {
      return { message: error.message };
    }

    return { message: 'Erro inesperado.' };
  }

  return { extract };
}
