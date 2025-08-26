import { apiFetch } from './api'

export function fetchUsers() {
  return apiFetch('/User', { method: 'GET' })
}

export function createUser(data: any) {
  return apiFetch('/User', {
    method: 'POST',
    body: JSON.stringify(data)
  })
}