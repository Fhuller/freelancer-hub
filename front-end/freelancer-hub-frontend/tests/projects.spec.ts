// tests/projects.spec.ts
import { describe, it, expect, vi, beforeEach } from 'vitest'
import * as api from '@/services/api'
import * as projects from '@/services/projects'

vi.mock('@/services/api', () => ({
    apiFetch: vi.fn()
}))

describe('projects service', () => {
    const mockApiFetch = api.apiFetch as unknown as ReturnType<typeof vi.fn>

    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe('CRUD básico de projetos', () => {
        it('fetchProjects chama endpoint correto', async () => {
            await projects.fetchProjects()
            expect(mockApiFetch).toHaveBeenCalledWith('/Project', { method: 'GET' })
        })

        it('fetchProjectById chama endpoint com ID', async () => {
            await projects.fetchProjectById('123')
            expect(mockApiFetch).toHaveBeenCalledWith('/Project/123', { method: 'GET' })
        })

        it('createProject envia POST com body JSON', async () => {
            const data = { userId: '1', title: 'Teste', hourlyRate: 100 }
            await projects.createProject(data)
            expect(mockApiFetch).toHaveBeenCalledWith('/Project', {
                method: 'POST',
                body: JSON.stringify(data)
            })
        })

        it('updateProject envia PUT com body JSON', async () => {
            const data = { title: 'Novo', status: 'Ativo' }
            await projects.updateProject('10', data)
            expect(mockApiFetch).toHaveBeenCalledWith('/Project/10', {
                method: 'PUT',
                body: JSON.stringify(data)
            })
        })

        it('deleteProject chama DELETE corretamente', async () => {
            await projects.deleteProject('55')
            expect(mockApiFetch).toHaveBeenCalledWith('/Project/55', { method: 'DELETE' })
        })
    })

    describe('arquivos de projeto', () => {
        it('uploadProjectFile envia FormData', async () => {
            const mockFile = new File(['conteudo'], 'teste.txt', { type: 'text/plain' })
            await projects.uploadProjectFile('99', mockFile)
            const args = mockApiFetch.mock.calls[0]
            expect(args[0]).toBe('/Project/99/files')
            expect(args[1].method).toBe('POST')
            expect(args[1].body instanceof FormData).toBe(true)
        })

        it('getProjectFiles usa método GET', async () => {
            await projects.getProjectFiles('42')
            expect(mockApiFetch).toHaveBeenCalledWith('/Project/42/files', { method: 'GET' })
        })

        it('deleteProjectFile chama endpoint correto', async () => {
            await projects.deleteProjectFile('88', '999')
            expect(mockApiFetch).toHaveBeenCalledWith('/Project/88/files/999', { method: 'DELETE' })
        })

        it('downloadProjectFile retorna blob quando sucesso', async () => {
            const mockBlob = new Blob(['abc'])
            global.fetch = vi.fn().mockResolvedValue({ ok: true, blob: () => Promise.resolve(mockBlob) }) as any
            const result = await projects.downloadProjectFile('http://teste.com/file')
            expect(result).toBe(mockBlob)
        })

        it('downloadProjectFile lança erro se falhar', async () => {
            global.fetch = vi.fn().mockResolvedValue({ ok: false }) as any
            await expect(projects.downloadProjectFile('http://falha.com')).rejects.toThrow('Falha no download')
        })
    })

    describe('horas e taxa horária', () => {
        beforeEach(() => {
            vi.clearAllMocks()
        })

        it('updateProjectHours envia PUT com body JSON', async () => {
            const data = { hoursToAdd: 2 }
            await projects.updateProjectHours('10', data)
            expect(mockApiFetch).toHaveBeenCalledWith('/Project/10/hours', {
                method: 'PUT',
                body: JSON.stringify(data)
            })
        })

        it('getProjectHoursSummary usa método GET', async () => {
            await projects.getProjectHoursSummary('11')
            expect(mockApiFetch).toHaveBeenCalledWith('/Project/11/hours-summary', { method: 'GET' })
        })

        it('addHoursToProject chama apiFetch corretamente', async () => {
            await projects.addHoursToProject('p1', 5, 'teste')
            expect(mockApiFetch).toHaveBeenCalledWith('/Project/p1/hours', {
                method: 'PUT',
                body: JSON.stringify({ hoursToAdd: 5, description: 'teste' })
            })
        })

        it('setProjectTotalHours chama apiFetch corretamente', async () => {
            await projects.setProjectTotalHours('p2', 10, 'ajuste')
            expect(mockApiFetch).toHaveBeenCalledWith('/Project/p2/hours', {
                method: 'PUT',
                body: JSON.stringify({ totalHours: 10, description: 'ajuste' })
            })
        })

        it('updateProjectHourlyRate chama apiFetch corretamente', async () => {
            await projects.updateProjectHourlyRate('p3', 120)
            expect(mockApiFetch).toHaveBeenCalledWith('/Project/p3/hours', {
                method: 'PUT',
                body: JSON.stringify({ hourlyRate: 120 })
            })
        })
    })

    describe('funções utilitárias', () => {
        it('calculateTotalEarned multiplica corretamente', () => {
            expect(projects.calculateTotalEarned(10, 50)).toBe(500)
        })

        it('formatHoursDisplay mostra apenas horas inteiras', () => {
            expect(projects.formatHoursDisplay(5)).toBe('5h')
        })

        it('formatHoursDisplay mostra horas e minutos', () => {
            expect(projects.formatHoursDisplay(5.5)).toBe('5h 30min')
        })

        it('formatCurrency retorna valor formatado em BRL', () => {
            const result = projects.formatCurrency(1234.56)
            expect(result).toMatch(/^R\$/)
            expect(result).toContain('1.234')
        })

        it('formatCurrency aceita outro tipo de moeda', () => {
            const result = projects.formatCurrency(100, 'USD')
            expect(result).toContain('US')
        })
    })
})
