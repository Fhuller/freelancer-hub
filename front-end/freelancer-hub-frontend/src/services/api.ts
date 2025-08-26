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

  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    'Accept': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {})
  }

  try {
    const response = await fetch(apiUrl + endpoint, {
      ...options,
      headers: {
        ...headers,
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
        toast.error(message)
      }
      throw new Error(`HTTP error: ${response.status}`)
    }

    return await response.json()
  } catch (err) {
    throw err
  }
}
