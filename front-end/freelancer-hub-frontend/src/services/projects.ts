import { apiFetch } from './api'

export interface ProjectReadDto {
  id: string;
  userId: string
  clientId?: string
  title: string
  description?: string
  status?: string
  dueDate?: string
}

export interface FileDto {
  id: string;
  fileName: string;
  fileExtension: string;
  fileUrl: string;
  fileSize: number;
  createdAt: string;
}

// Métodos existentes...
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

export function uploadProjectFile(projectId: string, file: File) {
  const formData = new FormData()
  formData.append('file', file)
  
  return apiFetch(`/Project/${projectId}/files`, {
    method: 'POST',
    body: formData
  })
}

export function getProjectFiles(projectId: string): Promise<FileDto[]> {
  return apiFetch(`/Project/${projectId}/files`, { 
    method: 'GET' 
  })
}

export function deleteProjectFile(projectId: string, fileId: string) {
  return apiFetch(`/Project/${projectId}/files/${fileId}`, { 
    method: 'DELETE' 
  })
}

export function downloadProjectFile(fileUrl: string): Promise<Blob> {
  return fetch(fileUrl).then(response => {
    if (!response.ok) throw new Error('Falha no download')
    return response.blob()
  })
}