import { describe, it, expect, vi, beforeEach, afterEach } from 'vitest'
import { apiFetch } from '@/services/api.ts'
import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toast-notification'

// Mock do ambiente
vi.stubEnv('VITE_API_URL', 'https://api.test')

// Mocks de dependências externas
vi.mock('@/stores/auth', () => ({
    useAuthStore: vi.fn()
}))

vi.mock('vue-toast-notification', () => ({
    useToast: vi.fn()
}))

describe('apiFetch', () => {
    const mockLogout = vi.fn()
    const mockToastError = vi.fn()
    const mockAuth = { accessToken: 'fake_token', logout: mockLogout }

    beforeEach(() => {
        vi.clearAllMocks()
            ; (useAuthStore as any).mockReturnValue(mockAuth)
            ; (useToast as any).mockReturnValue({ error: mockToastError })
    })

    afterEach(() => {
        vi.restoreAllMocks()
    })

    it('faz requisição com Authorization se houver token', async () => {
        const mockResponse = new Response(JSON.stringify({ ok: true }), { status: 200 })
        global.fetch = vi.fn().mockResolvedValue(mockResponse)

        await apiFetch('/test')

        expect(global.fetch).toHaveBeenCalledWith(
            expect.stringContaining('/test'),
            expect.objectContaining({
                headers: expect.objectContaining({
                    Authorization: 'Bearer fake_token',
                    'Content-Type': 'application/json',
                    Accept: 'application/json'
                })
            })
        )
    })

    it('não adiciona Content-Type se body for FormData', async () => {
        const formData = new FormData()
        const mockResponse = new Response('{}', { status: 200 })
        global.fetch = vi.fn().mockResolvedValue(mockResponse)

        await apiFetch('/upload', { method: 'POST', body: formData })

        const callArgs = (global.fetch as any).mock.calls[0][1]
        expect(callArgs.headers['Content-Type']).toBeUndefined()
    })

    it('retorna JSON quando a resposta for válida', async () => {
        const mockResponse = new Response(JSON.stringify({ id: 1 }), { status: 200 })
        global.fetch = vi.fn().mockResolvedValue(mockResponse)

        const data = await apiFetch('/ok')
        expect(data).toEqual({ id: 1 })
    })

    it('retorna undefined em respostas 204', async () => {
        const mockResponse = new Response('', { status: 204 })
        global.fetch = vi.fn().mockResolvedValue(mockResponse)

        const result = await apiFetch('/no-content')
        expect(result).toBeUndefined()
    })

    it('trata erro 401, mostra toast e faz logout', async () => {
        const mockResponse = new Response('Unauthorized', { status: 401 })
        global.fetch = vi.fn().mockResolvedValue(mockResponse)

        await expect(apiFetch('/unauthorized')).rejects.toThrow('HTTP error: 401')
        expect(mockToastError).toHaveBeenCalledWith('Sessão expirada, faça login novamente.')
        expect(mockLogout).toHaveBeenCalled()
    })

    it('trata erro 415 com mensagem específica', async () => {
        const mockResponse = new Response('Unsupported Media Type', { status: 415 })
        global.fetch = vi.fn().mockResolvedValue(mockResponse)

        await expect(apiFetch('/bad-media')).rejects.toThrow('HTTP error: 415')
        expect(mockToastError).toHaveBeenCalledWith(
            'Tipo de mídia não suportado. O servidor não aceitou o formato do arquivo.'
        )
    })

    it('trata erro genérico com mensagem do servidor', async () => {
        const mockResponse = new Response(JSON.stringify({ message: 'Erro customizado' }), {
            status: 500
        })
        global.fetch = vi.fn().mockResolvedValue(mockResponse)

        await expect(apiFetch('/server-error')).rejects.toThrow('HTTP error: 500')
        expect(mockToastError).toHaveBeenCalledWith('Erro customizado')
    })

    it('trata erro genérico sem JSON válido', async () => {
        const mockResponse = new Response('Internal Server Error', {
            status: 500,
            statusText: 'Internal Server Error'
        })
        global.fetch = vi.fn().mockResolvedValue(mockResponse)

        await expect(apiFetch('/server-error')).rejects.toThrow('HTTP error: 500')
        expect(mockToastError).toHaveBeenCalledWith('Internal Server Error')
    })
})
