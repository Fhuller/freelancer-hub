import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toast-notification'

const apiUrl = import.meta.env.VITE_API_URL

export async function apiFetch(
  endpoint: string,
  options: RequestInit = {}
) {
  const auth = useAuthStore()
  const toast = useToast()
  const token = auth.accessToken

  // Headers base - só adiciona Content-Type se NÃO for FormData
  const baseHeaders: HeadersInit = {
    'Accept': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {})
  }

  // Se o body NÃO for FormData, adiciona Content-Type JSON
  if (!(options.body instanceof FormData)) {
    baseHeaders['Content-Type'] = 'application/json'
  }

  try {
    const response = await fetch(apiUrl + endpoint, {
      ...options,
      headers: {
        ...baseHeaders,
        ...(options.headers || {})
      }
    })

    if (!response.ok) {
      if (response.status === 401) {
        toast.error('Sessão expirada, faça login novamente.')
        auth.logout()
      } else {
        let message = response.statusText || `Erro ${response.status}`
        try {
          const data = await response.json()
          if (data.message) {
            message = data.message
          }
        } catch {
          // se não for JSON, ignora
        }
        
        // Mensagens mais específicas para erros comuns
        if (response.status === 415) {
          message = 'Tipo de mídia não suportado. O servidor não aceitou o formato do arquivo.'
        }
        
        toast.error(message)
      }
      throw new Error(`HTTP error: ${response.status}`)
    }

    if (response.status === 204 || response.headers.get('Content-Length') === '0') {
      return
    }

    const text = await response.text()
    return text ? JSON.parse(text) : undefined
  } catch (err) {
    throw err
  }
}