import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toast-notification'

const apiUrl = import.meta.env.VITE_API_URL

function buildHeaders(token: string | undefined, options: RequestInit = {}): HeadersInit {
  const headers: HeadersInit = {
    Accept: 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {})
  }

  // Only add JSON content type if body is not FormData
  if (!(options.body instanceof FormData)) {
    headers['Content-Type'] = 'application/json'
  }

  return {
    ...headers,
    ...(options.headers || {})
  }
}

async function handleError(response: Response, toast: ReturnType<typeof useToast>, auth: ReturnType<typeof useAuthStore>) {
  if (response.status === 401) {
    toast.error('Sessão expirada, faça login novamente.')
    auth.logout()
    throw new Error(`HTTP error: ${response.status}`)
  }

  let message = response.statusText || `Erro ${response.status}`

  try {
    const data = await response.json()
    if (data && data.message) message = data.message
  } catch {
    // ignore non-JSON responses
  }

  if (response.status === 415) {
    message = 'Tipo de mídia não suportado. O servidor não aceitou o formato do arquivo.'
  }

  toast.error(message)
  throw new Error(`HTTP error: ${response.status}`)
}

async function parseResponse(response: Response) {
  if (response.status === 204 || response.headers.get('Content-Length') === '0') return undefined

  const text = await response.text()
  return text ? JSON.parse(text) : undefined
}

export async function apiFetch(endpoint: string, options: RequestInit = {}) {
  const auth = useAuthStore()
  const toast = useToast()
  const token = auth.accessToken

  const response = await fetch(apiUrl + endpoint, {
    ...options,
    headers: buildHeaders(token, options)
  })

  if (!response.ok) {
    await handleError(response, toast, auth)
  }

  return await parseResponse(response)
}