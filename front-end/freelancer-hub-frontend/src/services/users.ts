import { apiFetch } from './api'

export function fetchUsers() {
  return apiFetch('/User', { method: 'GET' })
}

export function fetchUserById(id: string) {
  return apiFetch(`/User/${id}`, { method: 'GET' })
}

export function fetchCurrentUser() {
  return apiFetch('/User/me', { method: 'GET' })
}

export function createUser(data: { name: string; email: string }) {
  return apiFetch('/User', {
    method: 'POST',
    body: JSON.stringify(data)
  })
}

export function updateUser(id: string, data: { name: string; email: string }) {
  return apiFetch(`/User/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data)
  })
}

export function deleteUser(id: string) {
  return apiFetch(`/User/${id}`, { method: 'DELETE' })
}

export function updateUserLanguage(id: string, language: string) {
  return apiFetch(`/User/${id}/language`, {
    method: 'PATCH',
    body: JSON.stringify(language)
  });
}
