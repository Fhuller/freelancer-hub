import { describe, it, vi, expect } from 'vitest'
import { mount } from '@vue/test-utils'

// mock do Pinia e do store
const checkAuthMock = vi.fn()
vi.mock('@/stores/auth', () => ({
  useAuthStore: vi.fn(() => ({
    checkAuth: checkAuthMock
  }))
}))

// mock global do RouterView
vi.mock('vue-router', () => ({
  RouterView: {
    template: '<div data-test="router-view"></div>'
  }
}))

import App from '@/App.vue'

describe('App.vue', () => {
  it('chama checkAuth() no onMounted', async () => {
    checkAuthMock.mockClear()
    mount(App, {
      global: {
        components: {
          RouterView: { template: '<div />' }
        }
      }
    })

    // valida apenas o efeito colateral relevante
    expect(checkAuthMock).toHaveBeenCalledTimes(1)
  })

  it('renderiza o RouterView', () => {
    const wrapper = mount(App, {
      global: {
        components: {
          RouterView: { template: '<div data-test="router-view"></div>' }
        }
      }
    })
    expect(wrapper.find('[data-test="router-view"]').exists()).toBe(true)
  })
})
