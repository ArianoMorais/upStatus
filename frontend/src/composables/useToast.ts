import { useToast as usePrimeToast } from 'primevue/usetoast';

type Severity = 'success' | 'info' | 'warn' | 'error';

export function useAppToast() {
  const toast = usePrimeToast();

  function show(severity: Severity, summary: string, detail?: string, life = 4000) {
    toast.add({ severity, summary, detail, life });
  }

  return {
    success: (summary: string, detail?: string) => show('success', summary, detail),
    info: (summary: string, detail?: string) => show('info', summary, detail),
    warn: (summary: string, detail?: string) => show('warn', summary, detail),
    error: (summary: string, detail?: string) => show('error', summary, detail, 6000),
  };
}
