import { describe, it, vi, expect, beforeEach } from 'vitest'

// mock parcial do pinia para não quebrar o router
vi.mock('pinia', async (importOriginal) => {
  const actual = await importOriginal()
  return {
    ...actual,
    createPinia: vi.fn(() => 'pinia-instance'),
    defineStore: vi.fn(() => vi.fn(() => ({})))
  }
})

vi.mock('vue', () => ({
  createApp: vi.fn(() => ({
    use: vi.fn().mockReturnThis(),
    mount: vi.fn(),
  })),
}))
vi.mock('vue-toast-notification', () => ({
  default: 'ToastPlugin',
}))
vi.mock('@/router', () => ({
  default: 'router-instance',
}))
vi.mock('@/i18n', () => ({
  default: 'i18n-instance',
}))
vi.mock('@/App.vue', () => ({
  default: 'AppComponent',
}))

import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from '@/router'
import i18n from '@/i18n'
import ToastPlugin from 'vue-toast-notification'
import '@/main'

describe('main.ts', () => {
  let appMock

  beforeEach(() => {
    appMock = createApp.mock.results[0].value
  })

  it('cria a aplicação com o componente App', () => {
    expect(createApp).toHaveBeenCalledWith('AppComponent')
  })

  it('usa o Pinia, i18n, router e ToastPlugin', () => {
    const usedPlugins = appMock.use.mock.calls.map(c => c[0])
    expect(usedPlugins).toContain(createPinia.mock.results[0].value)
    expect(usedPlugins).toContain(i18n)
    expect(usedPlugins).toContain(router)
    expect(usedPlugins).toContain(ToastPlugin)
  })

  it('monta a aplicação no #app', () => {
    expect(appMock.mount).toHaveBeenCalledWith('#app')
  })
})
