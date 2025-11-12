import { describe, it, vi, expect, beforeEach } from 'vitest'
import { createRouter, createWebHistory } from 'vue-router'
import { nextTick } from 'vue'
import router from '../src/router'

// Mock das dependências do router
vi.mock('../src/stores/auth', () => ({
    useAuthStore: vi.fn()
}))
vi.mock('vue-toast-notification', () => ({
    useToast: vi.fn(() => ({
        warning: vi.fn()
    }))
}))

const { useAuthStore } = await import('../src/stores/auth')
const { useToast } = await import('vue-toast-notification')

describe('Router', () => {
    let mockAuthStore
    let mockToast

    beforeEach(() => {
        mockToast = { warning: vi.fn() }
        useToast.mockReturnValue(mockToast)

        mockAuthStore = {
            isAuthenticated: false,
            checkAuth: vi.fn(),
            error: null
        }
        useAuthStore.mockReturnValue(mockAuthStore)
    })

    it('deve conter todas as rotas esperadas', () => {
        const routeNames = router.getRoutes().map(r => r.name)
        expect(routeNames).toContain('LandingPage')
        expect(routeNames).toContain('Login')
        expect(routeNames).toContain('Dashboard')
        expect(routeNames).toContain('ClientProject')
    })

    it('permite navegar para rotas públicas sem autenticação', async () => {
        await router.push('/')
        await router.isReady()
        expect(router.currentRoute.value.name).toBe('LandingPage')
    })

    it('redireciona para Login se tentar acessar rota protegida sem autenticação', async () => {
        mockAuthStore.isAuthenticated = true // simula autenticado antes
        mockAuthStore.checkAuth.mockImplementationOnce(async () => {
            // durante o checkAuth, o router vai aguardar esse await
            mockAuthStore.isAuthenticated = false // após checar, ficou inválido
        })

        await router.push('/app/dashboard')
        await router.isReady()
        await nextTick()

        expect(router.currentRoute.value.name).toBe('Login')
    })



    it('permite acessar rota protegida se autenticado', async () => {
        mockAuthStore.isAuthenticated = true
        mockAuthStore.checkAuth.mockResolvedValueOnce(true)

        await router.push('/app/dashboard')
        await nextTick()

        expect(router.currentRoute.value.name).toBe('Dashboard')
    })

    it('redireciona usuário autenticado que tenta acessar rota de convidado (ex: Login)', async () => {
        mockAuthStore.isAuthenticated = true
        await router.push('/login')
        await nextTick()

        expect(router.currentRoute.value.name).toBe('Dashboard')
    })

    it('redireciona rotas inexistentes para LandingPage', async () => {
        await router.push('/rota-inexistente')
        await router.isReady()
        expect(router.currentRoute.value.name).toBe('LandingPage')
    })
})
