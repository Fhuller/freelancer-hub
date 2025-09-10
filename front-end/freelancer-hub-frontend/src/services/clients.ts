import { apiFetch } from './api'

export interface ClientReadDto {
  id: string;
  name: string;
  email: string;
  phone?: string;
  companyName?: string;
  createdAt: string;
  updatedAt: string;
}

export interface ClientCreateDto {
  name: string;
  email: string;
  phone?: string;
  companyName?: string;
}

export interface ClientUpdateDto {
  name?: string;
  email?: string;
  phone?: string;
  companyName?: string;
}

export function fetchClients(): Promise<ClientReadDto[]> {
  return apiFetch('/Client', { method: 'GET' })
}

export function fetchClientById(id: string): Promise<ClientReadDto> {
  return apiFetch(`/Client/${id}`, { method: 'GET' })
}

export function createClient(data: ClientCreateDto): Promise<ClientReadDto> {
  return apiFetch('/Client', {
    method: 'POST',
    body: JSON.stringify(data)
  })
}

export function updateClient(id: string, data: ClientUpdateDto): Promise<void> {
  return apiFetch(`/Client/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data)
  })
}

export function deleteClient(id: string): Promise<void> {
  return apiFetch(`/Client/${id}`, {
    method: 'DELETE'
  })
}