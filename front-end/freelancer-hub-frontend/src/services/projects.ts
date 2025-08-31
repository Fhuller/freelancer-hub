import { apiFetch } from './api'

export function fetchProjects() {
  return apiFetch('/Project', { method: 'GET' })
}

export function fetchProjectById(id: string) {
  return apiFetch(`/Project/${id}`, { method: 'GET' })
}

export function createProject(data: {
  userId: string
  clientId?: string
  title: string
  description?: string
  status?: string
  dueDate?: string
}) {
  return apiFetch('/Project', {
    method: 'POST',
    body: JSON.stringify(data)
  })
}

export function updateProject(id: string, data: {
  clientId?: string
  title: string
  description?: string
  status: string
  dueDate?: string
}) {
  return apiFetch(`/Project/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data)
  })
}

export function deleteProject(id: string) {
  return apiFetch(`/Project/${id}`, { method: 'DELETE' })
}
