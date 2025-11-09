import { mount } from '@vue/test-utils'
import { describe, it, expect, vi } from 'vitest'
import AuthenticatedLayout from '@/layouts/AuthenticatedLayout.vue'

// Mock do store
vi.mock('@/stores/auth', () => ({
  useAuthStore: vi.fn(() => ({
    user: { name: 'Teste' },
    isAuthenticated: true
  }))
}))

// Mock do Vue Router
vi.mock('vue-router', () => ({
  useRoute: () => ({
    name: 'Home',
    params: {},
    query: {},
  }),
  useRouter: () => ({
    push: vi.fn(),
    replace: vi.fn(),
  }),
}))

// Mock do Vue I18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => key,
    locale: { value: 'pt-BR' }
  })
}))

describe('AuthenticatedLayout.vue', () => {
  function createWrapper(options: any = {}) {
    return mount(AuthenticatedLayout, {
      props: options.props || {},
      slots: options.slots || {},
      global: {
        stubs: {
          Header: {
            template: '<div data-test="header" @click="$emit(\'toggle-sidebar\')"></div>'
          },
          Sidebar: {
            name: 'Sidebar',
            props: ['isOpen'],
            template: '<div data-test="sidebar"></div>'
          },
          ContentSection: {
            template: '<div data-test="content"><slot /></div>'
          }
        }
      }
    })
  }

  it('renderiza corretamente os componentes base', () => {
    const wrapper = createWrapper()
    expect(wrapper.find('[data-test="header"]').exists()).toBe(true)
    expect(wrapper.find('[data-test="sidebar"]').exists()).toBe(true)
    expect(wrapper.find('[data-test="content"]').exists()).toBe(true)
  })

  it('renderiza o conteúdo do slot quando não está carregando', () => {
    const wrapper = createWrapper({
      props: { loading: false },
      slots: { default: '<div class="conteudo-teste">Conteúdo</div>' }
    })

    expect(wrapper.find('.conteudo-teste').exists()).toBe(true)
    expect(wrapper.find('.loading-container').exists()).toBe(false)
  })

  it('mostra a tela de loading quando loading=true', () => {
    const wrapper = createWrapper({
      props: { loading: true }
    })

    expect(wrapper.find('.loading-container').exists()).toBe(true)
    expect(wrapper.find('img.loading-image').exists()).toBe(true)
  })

  it('alterna o estado do sidebar ao emitir o evento do Header', async () => {
    const wrapper = createWrapper()
    const header = wrapper.find('[data-test="header"]')

    await header.trigger('click')
    expect(wrapper.vm.isSidebarOpen).toBe(true)

    await header.trigger('click')
    expect(wrapper.vm.isSidebarOpen).toBe(false)
  })

  it('fecha o sidebar quando evento close é emitido', async () => {
    const wrapper = createWrapper()
    const sidebar = wrapper.findComponent({ name: 'Sidebar' })

    await wrapper.vm.toggleSidebar() // abre primeiro
    expect(wrapper.vm.isSidebarOpen).toBe(true)

    await sidebar.vm.$emit('close')
    expect(wrapper.vm.isSidebarOpen).toBe(false)
  })
})
