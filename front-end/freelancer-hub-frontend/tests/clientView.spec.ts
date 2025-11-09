import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'

// Criamos a variável fora para capturar o mock do push
const mockPush = vi.fn()

// Mock do vue-router (feito antes de importar o componente)
vi.mock('vue-router', async (importOriginal) => {
  const actual = await importOriginal()
  return {
    ...actual,
    useRoute: () => ({
      params: { id: '123' }
    }),
    useRouter: () => ({
      push: mockPush
    }),
    createRouter: vi.fn(() => ({
      beforeEach: vi.fn(),
      push: vi.fn(),
      replace: vi.fn(),
      currentRoute: { value: {} }
    })),
    createWebHistory: vi.fn()
  }
})

// Mock do i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key
  })
}))

// Mock dos serviços
vi.mock('@/services/clients', () => ({
  fetchClientById: vi.fn()
}))
vi.mock('@/services/projects', () => ({
  fetchProjects: vi.fn(),
  createProject: vi.fn(),
  updateProject: vi.fn(),
  deleteProject: vi.fn()
}))

// Agora importamos o componente e serviços
import ClientView from '@/views/Client.vue'
import { fetchClientById } from '@/services/clients'
import { fetchProjects, createProject, updateProject, deleteProject } from '@/services/projects'

// Stubs dos componentes
const stubs = {
  AuthenticatedLayout: { template: '<div><slot /></div>', props: ['loading'] },
  BaseHeader: { template: '<div data-test="base-header"></div>', props: ['model', 'modelName'] },
  AddCard: { template: '<button data-test="add-card" @click="$emit(\'click\')"></button>', props: ['label', 'onClick'] },
  ContentCard: { 
    template: `
      <div class="content-card">
        <button class="main" @click="$props.onMainClick && $props.onMainClick()"></button>
        <button class="edit" @click="$props.onEdit && $props.onEdit()"></button>
        <button class="delete" @click="$props.onDelete && $props.onDelete()"></button>
      </div>
    `,
    props: ['label', 'onMainClick', 'onEdit', 'onDelete']
  },
  BaseModal: { 
    template: '<div v-if="visible" data-test="modal"></div>', 
    props: ['visible', 'modelValues', 'modelDisplay', 'onSave', 'modelName']
  }
}

describe('Client.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('carrega cliente e projetos ao montar', async () => {
    fetchClientById.mockResolvedValueOnce({ id: '123', name: 'Cliente Teste' })
    fetchProjects.mockResolvedValueOnce([{ id: 1, title: 'Proj A', clientId: '123' }])

    mount(ClientView, { global: { stubs } })
    await flushPromises()

    expect(fetchClientById).toHaveBeenCalledWith('123')
    expect(fetchProjects).toHaveBeenCalled()
  })

  it('exibe erro quando fetchClientById falha', async () => {
    fetchClientById.mockRejectedValueOnce('Erro ao buscar cliente')

    const wrapper = mount(ClientView, { global: { stubs } })
    await flushPromises()

    expect(wrapper.text()).toContain('Erro ao buscar cliente')
  })

  it('abre modal ao clicar em AddCard', async () => {
    fetchClientById.mockResolvedValueOnce({ id: '123', name: 'Cliente Teste' })
    fetchProjects.mockResolvedValueOnce([])
    const wrapper = mount(ClientView, { global: { stubs } })
    await flushPromises()

    await wrapper.vm.openNewProjectModal()
    await wrapper.vm.$nextTick()

    expect(wrapper.vm.showModal).toBe(true)
  })

  it('chama updateProject ao salvar projeto em edição', async () => {
    fetchClientById.mockResolvedValueOnce({ id: '123', name: 'Cliente Teste' })
    fetchProjects.mockResolvedValueOnce([{ id: 1, title: 'Teste', clientId: '123' }])
    updateProject.mockResolvedValueOnce({})

    const wrapper = mount(ClientView, { global: { stubs } })
    await flushPromises()

    wrapper.vm.client = { id: '123' }
    wrapper.vm.editProject({ id: 1, title: 'Teste' })
    await wrapper.vm.saveProject({ title: 'Atualizado', status: 'Pendente' })

    expect(updateProject).toHaveBeenCalledWith(1, expect.objectContaining({
      title: 'Atualizado',
      clientId: '123'
    }))
  })

  it('chama createProject ao salvar novo projeto', async () => {
    fetchClientById.mockResolvedValueOnce({ id: '123', name: 'Cliente Teste' })
    fetchProjects.mockResolvedValueOnce([])
    createProject.mockResolvedValueOnce({})

    const wrapper = mount(ClientView, { global: { stubs } })
    await flushPromises()

    wrapper.vm.client = { id: '123' }
    wrapper.vm.editingProject = null
    await wrapper.vm.saveProject({ title: 'Novo Projeto', status: 'Pendente' })

    expect(createProject).toHaveBeenCalledWith(expect.objectContaining({
      title: 'Novo Projeto',
      clientId: '123'
    }))
  })

  it('chama deleteProject ao remover um projeto', async () => {
    fetchClientById.mockResolvedValueOnce({ id: '123', name: 'Cliente Teste' })
    fetchProjects.mockResolvedValueOnce([{ id: 1, title: 'P', clientId: '123' }])
    deleteProject.mockResolvedValueOnce({})

    const wrapper = mount(ClientView, { global: { stubs } })
    await flushPromises()

    await wrapper.vm.removeProject({ id: 1 })
    expect(deleteProject).toHaveBeenCalledWith(1)
  })

  it('navega para detalhes do projeto ao clicar no ContentCard', async () => {
    fetchClientById.mockResolvedValueOnce({ id: '123', name: 'Cliente Teste' })
    fetchProjects.mockResolvedValueOnce([{ id: 7, title: 'Proj', clientId: '123' }])

    const wrapper = mount(ClientView, { global: { stubs } })
    await flushPromises()

    wrapper.vm.client = { id: '123' }
    await wrapper.vm.openProjectDetails({ id: 7 })

    expect(mockPush).toHaveBeenCalledWith({
      name: 'ClientProject',
      params: { id: '123', projectId: 7 }
    })
  })
})
