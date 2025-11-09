import { describe, it, vi, expect, beforeEach } from 'vitest'

// mocka as dependências
vi.mock('vue-i18n', () => ({
  createI18n: vi.fn(() => 'i18n-instance')
}))

vi.mock('@/locales/pt-br.json', () => ({
  default: { hello: 'Olá' }
}), { virtual: true })

vi.mock('@/locales/en.json', () => ({
  default: { hello: 'Hello' }
}), { virtual: true })

import { createI18n } from 'vue-i18n'
import i18n from '@/i18n'

describe('i18n', () => {
  it('cria a instância de i18n com as configurações corretas', () => {
    expect(createI18n).toHaveBeenCalledWith({
      locale: 'pt',
      fallbackLocale: 'en',
      messages: {
        pt: { hello: 'Olá' },
        en: { hello: 'Hello' }
      }
    })
  })

  it('exporta a instância criada pelo createI18n', () => {
    expect(i18n).toBe('i18n-instance')
  })
})
