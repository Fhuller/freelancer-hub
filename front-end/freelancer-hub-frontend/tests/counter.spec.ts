import { setActivePinia, createPinia } from 'pinia'
import { describe, it, expect, beforeEach } from 'vitest'
import { useCounterStore } from '@/stores/counter'

describe('Counter Store', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('inicia com count = 0', () => {
    const store = useCounterStore()
    expect(store.count).toBe(0)
  })

  it('incrementa o count corretamente', () => {
    const store = useCounterStore()
    store.increment()
    expect(store.count).toBe(1)
  })

  it('calcula o doubleCount corretamente', () => {
    const store = useCounterStore()
    store.count = 3
    expect(store.doubleCount).toBe(6)
  })
})
