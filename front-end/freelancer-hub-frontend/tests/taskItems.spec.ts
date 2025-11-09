import { describe, it, expect, vi, beforeEach } from 'vitest'

// 1. Criar o mock para a 'apiFetch'
const { mockApiFetch } = vi.hoisted(() => {
  return {
    mockApiFetch: vi.fn()
  }
})

// 2. Mockar o módulo 'api' USANDO O PATH ALIAS '@'
// Isso garante que estamos mockando 'src/services/api.ts'
vi.mock('@/services/api', () => ({
  apiFetch: mockApiFetch
}))

// 3. Importar o serviço que queremos testar
import {
  fetchTaskItemsByProject,
  fetchTaskItemById,
  createTaskItem,
  updateTaskItem,
  deleteTaskItem
} from '@/services/taskitems' 

describe('TaskItems Service', () => {

  beforeEach(() => {
    vi.clearAllMocks()
  })

  // ... (todos os outros testes 'it' permanecem os mesmos) ...

  it('deve chamar apiFetch para fetchTaskItemsByProject', async () => {
    const mockResponse = { data: [{ id: '1', title: 'Task 1' }] }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await fetchTaskItemsByProject('proj-123')

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/TaskItem/project/proj-123',
      { method: 'GET' }
    )
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar apiFetch para fetchTaskItemById', async () => {
    const mockResponse = { data: { id: 'task-abc', title: 'Task ABC' } }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await fetchTaskItemById('task-abc')

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/TaskItem/task-abc',
      { method: 'GET' }
    )
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar apiFetch para createTaskItem', async () => {
    const taskData = {
      projectId: 'proj-1',
      title: 'Nova Tarefa',
      description: 'Descrição da tarefa'
    }
    const mockResponse = { data: { id: 'new-task-1', ...taskData } }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await createTaskItem(taskData)

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/TaskItem',
      {
        method: 'POST',
        body: JSON.stringify(taskData)
      }
    )
    expect(result).toEqual(mockResponse)
  })

  it('deve chamar apiFetch para updateTaskItem', async () => {
    const updateData = {
      title: 'Título Atualizado',
      status: 'Done',
      description: 'Desc atualizada'
    }
    const mockResponse = { data: { id: 'task-456', ...updateData } }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await updateTaskItem('task-456', updateData)

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/TaskItem/task-456',
      {
        method: 'PUT',
        body: JSON.stringify(updateData)
      }
    )
    expect(result).toEqual(mockResponse)
  })

  // Este teste agora deve passar, pois o mockApiFetch será usado
  // em vez do apiFetch real, e o Pinia nunca será chamado.
  it('deve chamar apiFetch para deleteTaskItem', async () => {
    const mockResponse = { data: null, error: null }
    mockApiFetch.mockResolvedValueOnce(mockResponse)

    const result = await deleteTaskItem('task-789')

    expect(mockApiFetch).toHaveBeenCalledWith(
      '/TaskItem/task-789',
      { method: 'DELETE' }
    )
    expect(result).toEqual(mockResponse)
  })
})