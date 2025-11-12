import { mount, flushPromises } from '@vue/test-utils'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import Clients from '@/views/Clients.vue'

// ===== MOCKS =====

// Router mock (corrigido com useRoute + beforeEach)
vi.mock('vue-router', () => {
  const push = vi.fn()
  const beforeEach = vi.fn()
  const mockRouter = { push, beforeEach }

  const mockRoute = {
    name: 'Clients',
    path: '/clients',
    params: {},
    query: {},
  }

  return {
    useRouter: () => mockRouter,
    useRoute: () => mockRoute,
    createRouter: vi.fn(() => mockRouter),
    createWebHistory: vi.fn(),
    RouterView: { name: 'RouterView', template: '<div />' },
    RouterLink: { name: 'RouterLink', props: ['to'], template: '<a><slot /></a>' },
  }
})

// i18n mock
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
  }),
}))

// Toast mock
const mockToastSuccess = vi.fn()
vi.mock('vue-toast-notification', () => ({
  useToast: () => ({
    success: mockToastSuccess,
  }),
}))

// Pinia store mock (auth)
vi.mock('@/stores/auth', () => ({
  useAuthStore: () => ({
    isAuthenticated: true,
    user: { id: 1, name: 'Teste' },
    logout: vi.fn(),
  }),
}))

// Services mock
vi.mock('@/services/clients', () => ({
  fetchClients: vi.fn(),
  createClient: vi.fn(),
  updateClient: vi.fn(),
  deleteClient: vi.fn(),
}))

import * as clientService from '@/services/clients'

// ===== TESTES =====
describe('Clients.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('carrega clientes no mounted', async () => {
    clientService.fetchClients.mockResolvedValue([
      { id: '1', name: 'Cliente Teste', email: 'a@b.com' },
    ])

    const wrapper = mount(Clients)
    await flushPromises()

    expect(clientService.fetchClients).toHaveBeenCalled()
    expect(wrapper.findComponent({ name: 'ContentCard' }).exists()).toBe(true)
  })

  it('mostra mensagem de erro se fetchClients falhar', async () => {
    clientService.fetchClients.mockRejectedValue(new Error('Erro'))

    const wrapper = mount(Clients)
    await flushPromises()

    expect(wrapper.text()).toContain('errorLoadingClients')
  })

  it('abre modal ao clicar em novo cliente', async () => {
    clientService.fetchClients.mockResolvedValue([])

    const wrapper = mount(Clients)
    await flushPromises()

    const addCard = wrapper.findComponent({ name: 'AddCard' })
    expect(addCard.exists()).toBe(true)

    await addCard.props('onClick')()
    expect(wrapper.vm.showModal).toBe(true)
  })

  it('abre modal para edição de cliente', async () => {
    clientService.fetchClients.mockResolvedValue([{ id: '1', name: 'Cliente X', email: 'x@y.com' }])

    const wrapper = mount(Clients)
    await flushPromises()

    const contentCard = wrapper.findComponent({ name: 'ContentCard' })
    expect(contentCard.exists()).toBe(true)

    await contentCard.props('onEdit')()
    expect(wrapper.vm.showModal).toBe(true)
    expect(wrapper.vm.editingClient).not.toBeNull()
  })

  it('remove cliente e recarrega lista', async () => {
    clientService.fetchClients.mockResolvedValue([{ id: '1', name: 'Cliente X', email: 'x@y.com' }])
    clientService.deleteClient.mockResolvedValue(undefined)

    const wrapper = mount(Clients)
    await flushPromises()

    const contentCard = wrapper.findComponent({ name: 'ContentCard' })
    await contentCard.props('onDelete')()
    await flushPromises()

    expect(clientService.deleteClient).toHaveBeenCalledWith('1')
    expect(clientService.fetchClients).toHaveBeenCalledTimes(2)
    expect(mockToastSuccess).toHaveBeenCalledWith('clientDeleted')
  })

  it('cria novo cliente corretamente', async () => {
    clientService.fetchClients.mockResolvedValue([])
    clientService.createClient.mockResolvedValue(undefined)

    const wrapper = mount(Clients)
    await flushPromises()

    const data = {
      name: 'Novo',
      email: 'n@e.com',
      phone: '123',
      companyName: 'Empresa',
      notes: 'obs',
    }

    await wrapper.vm.saveNewClient(data)
    await flushPromises()

    expect(clientService.createClient).toHaveBeenCalledWith({
      name: 'Novo',
      email: 'n@e.com',
      phone: '123',
      companyName: 'Empresa',
      notes: 'obs',
    })
    expect(mockToastSuccess).toHaveBeenCalledWith('clientCreated')
    expect(wrapper.vm.showModal).toBe(false)
  })

  it('atualiza cliente existente corretamente', async () => {
    clientService.fetchClients.mockResolvedValue([])
    clientService.updateClient.mockResolvedValue(undefined)

    const wrapper = mount(Clients)
    await flushPromises()

    wrapper.vm.editingClient = { id: '5', name: 'Antigo', email: 'a@a.com' }
    const data = {
      name: 'Atualizado',
      email: 'u@u.com',
      phone: '',
      companyName: '',
      notes: '',
    }

    await wrapper.vm.saveNewClient(data)
    await flushPromises()

    expect(clientService.updateClient).toHaveBeenCalledWith('5', {
      name: 'Atualizado',
      email: 'u@u.com',
      phone: undefined,
      companyName: undefined,
      notes: '',
    })
    expect(mockToastSuccess).toHaveBeenCalledWith('clientUpdated')
    expect(wrapper.vm.showModal).toBe(false)
  })

  it('fecha modal ao emitir evento close do BaseModal', async () => {
    clientService.fetchClients.mockResolvedValue([])
    const wrapper = mount(Clients)
    await flushPromises()

    wrapper.vm.showModal = true
    const modal = wrapper.findComponent({ name: 'BaseModal' })

    await modal.vm.$emit('close')
    expect(wrapper.vm.showModal).toBe(false)
  })
})
