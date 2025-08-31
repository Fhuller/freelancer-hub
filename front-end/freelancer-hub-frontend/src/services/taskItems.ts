import { apiFetch } from './api'

export function fetchTaskItemsByProject(projectId: string) {
  return apiFetch(`/TaskItem/project/${projectId}`, { method: 'GET' })
}

export function fetchTaskItemById(id: string) {
  return apiFetch(`/TaskItem/${id}`, { method: 'GET' })
}

export function createTaskItem(data: {
  projectId: string
  title: string
  description?: string
  status?: string
  dueDate?: string
}) {
  return apiFetch('/TaskItem', {
    method: 'POST',
    body: JSON.stringify(data)
  })
}

export function updateTaskItem(id: string, data: {
  title: string
  description?: string
  status: string
  dueDate?: string
}) {
  return apiFetch(`/TaskItem/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data)
  })
}

export function deleteTaskItem(id: string) {
  return apiFetch(`/TaskItem/${id}`, { method: 'DELETE' })
}
