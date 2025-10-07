import { apiFetch } from './api'

export interface ProjectReadDto {
  id: string;
  userId: string
  clientId?: string
  title: string
  description?: string
  status?: string
  dueDate?: string
  hourlyRate: number
  totalHours: number
  totalEarned: number
  createdAt: string
}

export interface FileDto {
  id: string;
  fileName: string;
  fileExtension: string;
  fileUrl: string;
  fileSize: number;
  createdAt: string;
}

export interface UpdateProjectHoursDto {
  totalHours?: number;
  hourlyRate?: number;
  hoursToAdd?: number;
  description?: string;
}

export interface ProjectHoursSummaryDto {
  projectId: string;
  projectTitle: string;
  totalHours: number;
  hourlyRate: number;
  totalEarned: number;
  lastUpdated: string;
}

// Métodos existentes...
export function fetchProjects(): Promise<ProjectReadDto[]> {
  return apiFetch('/Project', { method: 'GET' })
}

export function fetchProjectById(id: string): Promise<ProjectReadDto> {
  return apiFetch(`/Project/${id}`, { method: 'GET' })
}

export function createProject(data: {
  userId: string
  clientId?: string
  title: string
  description?: string
  status?: string
  dueDate?: string
  hourlyRate: number
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
  hourlyRate?: number
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

// NOVOS MÉTODOS PARA HORAS E HOURLY RATE

/**
 * Atualiza as horas e/ou taxa horária de um projeto
 */
export function updateProjectHours(
  projectId: string, 
  data: UpdateProjectHoursDto
): Promise<ProjectHoursSummaryDto> {
  return apiFetch(`/Project/${projectId}/hours`, {
    method: 'PUT',
    body: JSON.stringify(data)
  })
}

/**
 * Obtém o resumo de horas e valores de um projeto
 */
export function getProjectHoursSummary(projectId: string): Promise<ProjectHoursSummaryDto> {
  return apiFetch(`/Project/${projectId}/hours-summary`, { 
    method: 'GET' 
  })
}

/**
 * Adiciona horas incrementalmente a um projeto (convenience method)
 */
export function addHoursToProject(
  projectId: string, 
  hoursToAdd: number, 
  description?: string
): Promise<ProjectHoursSummaryDto> {
  return updateProjectHours(projectId, { hoursToAdd, description })
}

/**
 * Define horas totais de um projeto (convenience method)
 */
export function setProjectTotalHours(
  projectId: string, 
  totalHours: number, 
  description?: string
): Promise<ProjectHoursSummaryDto> {
  return updateProjectHours(projectId, { totalHours, description })
}

/**
 * Atualiza a taxa horária de um projeto (convenience method)
 */
export function updateProjectHourlyRate(
  projectId: string, 
  hourlyRate: number
): Promise<ProjectHoursSummaryDto> {
  return updateProjectHours(projectId, { hourlyRate })
}

/**
 * Calcula o valor total baseado nas horas e taxa
 */
export function calculateTotalEarned(totalHours: number, hourlyRate: number): number {
  return totalHours * hourlyRate
}

/**
 * Formata horas para exibição (ex: 5.5 → "5h 30min")
 */
export function formatHoursDisplay(totalHours: number): string {
  const hours = Math.floor(totalHours)
  const minutes = Math.round((totalHours - hours) * 60)
  
  if (minutes === 0) {
    return `${hours}h`
  }
  return `${hours}h ${minutes}min`
}

/**
 * Formata valor monetário
 */
export function formatCurrency(value: number, currency: string = 'BRL'): string {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: currency
  }).format(value)
}