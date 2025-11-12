// tests/pdfService.spec.ts
import { PdfService, PdfInvoiceData } from '@/services/pdf'
import { formatCurrency } from '@/services/projects'

vi.mock('@/services/projects', () => ({
  formatCurrency: vi.fn((value) => `R$ ${value.toFixed(2)}`)
}))

describe('PdfService', () => {
  const baseData: PdfInvoiceData = {
    invoiceNumber: 'INV-2025-TEST',
    clientName: 'Cliente Teste',
    clientEmail: 'cliente@teste.com',
    invoiceIssueDate: '01/11/2025',
    invoiceDueDate: '30/11/2025',
    invoiceStatus: 'Pendente',
    projectName: 'Projeto X',
    projectDescription: 'Desenvolvimento de sistema',
    projectTotalHours: 10,
    projectHourlyRate: 100,
    projectTotalEarned: 1000
  }

  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('generatePdfContent', () => {
    it('gera HTML com todas as seções principais', () => {
      const html = (PdfService as any).generatePdfContent(baseData)
      expect(html).toContain('FreelancerHub')
      expect(html).toContain(baseData.clientName)
      expect(html).toContain(baseData.projectName)
      expect(html).toContain('Itens da Fatura')
      expect(formatCurrency).toHaveBeenCalledWith(baseData.projectHourlyRate)
    })

    it('omite seções vazias corretamente', () => {
      const minimalData: PdfInvoiceData = {
        invoiceNumber: 'INV-2025-EMPTY',
        clientName: '',
        clientEmail: '',
        invoiceIssueDate: '',
        invoiceDueDate: '',
        invoiceStatus: '',
        projectName: '',
        projectDescription: '',
        projectTotalHours: 0,
        projectHourlyRate: 0,
        projectTotalEarned: 0
      }
      const html = (PdfService as any).generatePdfContent(minimalData)
      expect(html).not.toContain('Cliente')
      expect(html).not.toContain('Itens da Fatura')
    })
  })

  describe('generateInvoicePdf', () => {
    let mockWindow: any

    beforeEach(() => {
      mockWindow = {
        document: {
          write: vi.fn(),
          close: vi.fn()
        },
        print: vi.fn(),
        close: vi.fn()
      }
      vi.spyOn(window, 'open').mockImplementation(() => mockWindow)
    })

    it('retorna nome do arquivo após impressão', async () => {
      const promise = PdfService.generateInvoicePdf(baseData)
      mockWindow.onload() // simula carregamento
      mockWindow.onafterprint() // simula finalização
      const filename = await promise
      expect(filename).toContain('fatura-INV-2025-TEST.pdf')
      expect(mockWindow.document.write).toHaveBeenCalled()
    })

    it('resolve string vazia se não conseguir abrir janela', async () => {
      vi.spyOn(window, 'open').mockImplementation(() => null)
      const result = await PdfService.generateInvoicePdf(baseData)
      expect(result).toBe('')
    })
  })

  describe('generateInvoiceNumber', () => {
    it('gera número com prefixo e ano atual', () => {
      const number = PdfService.generateInvoiceNumber()
      const year = new Date().getFullYear()
      expect(number).toMatch(new RegExp(`INV-${year}-[A-Z0-9]+-[A-Z0-9]+`))
    })
  })

  describe('calculateDueDate', () => {
    it('retorna data ~30 dias à frente', () => {
      const today = new Date()
      const due = new Date(today.getTime() + 30 * 24 * 60 * 60 * 1000)
      const result = PdfService.calculateDueDate()
      expect(result).toBe(due.toLocaleDateString('pt-BR'))
    })
  })

  describe('getCurrentDate', () => {
    it('retorna data atual formatada', () => {
      const result = PdfService.getCurrentDate()
      const expected = new Date().toLocaleDateString('pt-BR')
      expect(result).toBe(expected)
    })
  })
})
