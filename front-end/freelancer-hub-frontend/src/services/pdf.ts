// services/pdfService.ts
import { formatCurrency } from './projects'

export interface PdfInvoiceData {
  invoiceNumber: string
  companyName: string
  companyAddress: string
  companyContact: string
  clientName: string
  clientEmail: string
  invoiceIssueDate: string
  invoiceDueDate: string
  invoiceStatus: string
  projectName: string
  projectDescription: string
  projectTotalHours: number
  projectHourlyRate: number
  projectTotalEarned: number
}

export class PdfService {
  /**
   * Gera um PDF de invoice baseado nos dados fornecidos
   */
  static async generateInvoicePdf(invoiceData: PdfInvoiceData): Promise<string> {
    return new Promise((resolve) => {
      const printWindow = window.open('', '_blank')
      if (!printWindow) {
        resolve('')
        return
      }

      const pdfContent = this.generatePdfContent(invoiceData)
      
      printWindow.document.write(pdfContent)
      printWindow.document.close()

      // Aguardar o conteúdo carregar e então imprimir/baixar
      printWindow.onload = () => {
        printWindow.print()
        printWindow.onafterprint = () => {
          const filename = `fatura-${invoiceData.invoiceNumber}.pdf`
          printWindow.close()
          resolve(filename)
        }
      }
    })
  }

  /**
   * Gera o conteúdo HTML do PDF
   */
  private static generatePdfContent(data: PdfInvoiceData): string {
    return `
      <!DOCTYPE html>
      <html>
      <head>
        <title>Fatura ${data.invoiceNumber}</title>
        <style>
          body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            color: #333;
          }
          .invoice-pdf {
            max-width: 800px;
            margin: 0 auto;
            background: white;
          }
          .pdf-header {
            display: flex;
            justify-content: space-between;
            margin-bottom: 30px;
            border-bottom: 2px solid #333;
            padding-bottom: 20px;
          }
          .company-info h2 {
            margin: 0 0 10px 0;
            color: #333;
            font-size: 18px;
          }
          .company-info p {
            margin: 5px 0;
            font-size: 12px;
            color: #666;
          }
          .invoice-title h1 {
            margin: 0;
            color: #333;
            font-size: 24px;
            text-align: right;
          }
          .invoice-number {
            margin: 10px 0 0 0;
            font-size: 14px;
            font-weight: bold;
            text-align: right;
          }
          .pdf-client-info {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 30px;
            margin-bottom: 30px;
          }
          .client-section h3, .invoice-details h3 {
            margin: 0 0 15px 0;
            color: #333;
            font-size: 16px;
            border-bottom: 1px solid #ddd;
            padding-bottom: 8px;
          }
          .client-section p, .invoice-details p {
            margin: 8px 0;
            font-size: 14px;
          }
          .pdf-project-info {
            margin-bottom: 30px;
          }
          .pdf-project-info h3 {
            margin: 0 0 15px 0;
            color: #333;
            font-size: 16px;
            border-bottom: 1px solid #ddd;
            padding-bottom: 8px;
          }
          .pdf-project-info p {
            margin: 8px 0;
            font-size: 14px;
          }
          .pdf-items {
            margin-bottom: 30px;
          }
          .pdf-items h3 {
            margin: 0 0 15px 0;
            color: #333;
            font-size: 16px;
            border-bottom: 1px solid #ddd;
            padding-bottom: 8px;
          }
          .items-table {
            width: 100%;
            border-collapse: collapse;
            font-size: 14px;
          }
          .items-table th {
            background-color: #f8f9fa;
            border: 1px solid #ddd;
            padding: 12px;
            text-align: left;
            font-weight: bold;
          }
          .items-table td {
            border: 1px solid #ddd;
            padding: 12px;
          }
          .pdf-summary {
            display: flex;
            justify-content: flex-end;
            margin-bottom: 30px;
          }
          .summary-total {
            width: 300px;
          }
          .total-row {
            display: flex;
            justify-content: space-between;
            padding: 8px 0;
            font-size: 14px;
          }
          .grand-total {
            border-top: 2px solid #333;
            margin-top: 10px;
            padding-top: 15px;
            font-size: 16px;
          }
          .pdf-footer {
            border-top: 2px solid #333;
            padding-top: 20px;
            font-size: 12px;
          }
          .pdf-footer p {
            margin: 8px 0;
          }
          .thank-you {
            text-align: center;
            font-style: italic;
            margin-top: 30px !important;
            color: #666;
          }
          @media print {
            body { margin: 0; }
            .invoice-pdf { box-shadow: none; }
          }
        </style>
      </head>
      <body>
        <div class="invoice-pdf">
          <div class="pdf-header">
            <div class="company-info">
              <h2>${data.companyName}</h2>
              <p>${data.companyAddress}</p>
              <p>${data.companyContact}</p>
            </div>
            <div class="invoice-title">
              <h1>FATURA</h1>
              <p class="invoice-number">Nº: ${data.invoiceNumber}</p>
            </div>
          </div>

          <div class="pdf-client-info">
            <div class="client-section">
              <h3>Cliente</h3>
              <p><strong>Nome:</strong> ${data.clientName}</p>
              <p><strong>Email:</strong> ${data.clientEmail}</p>
            </div>
            <div class="invoice-details">
              <h3>Detalhes da Fatura</h3>
              <p><strong>Data de Emissão:</strong> ${data.invoiceIssueDate}</p>
              <p><strong>Data de Vencimento:</strong> ${data.invoiceDueDate}</p>
              <p><strong>Status:</strong> ${data.invoiceStatus}</p>
            </div>
          </div>

          <div class="pdf-project-info">
            <h3>Projeto</h3>
            <p><strong>Nome do Projeto:</strong> ${data.projectName}</p>
            <p><strong>Descrição:</strong> ${data.projectDescription}</p>
          </div>

          <div class="pdf-items">
            <h3>Itens da Fatura</h3>
            <table class="items-table">
              <thead>
                <tr>
                  <th>Descrição</th>
                  <th>Quantidade</th>
                  <th>Taxa</th>
                  <th>Total</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>Serviços de desenvolvimento - ${data.projectName}</td>
                  <td>${data.projectTotalHours.toFixed(2)} horas</td>
                  <td>${formatCurrency(data.projectHourlyRate)}/h</td>
                  <td>${formatCurrency(data.projectTotalEarned)}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="pdf-summary">
            <div class="summary-total">
              <div class="total-row">
                <span>Subtotal:</span>
                <span>${formatCurrency(data.projectTotalEarned)}</span>
              </div>
              <div class="total-row">
                <span>Taxas:</span>
                <span>${formatCurrency(0)}</span>
              </div>
              <div class="total-row grand-total">
                <span><strong>TOTAL:</strong></span>
                <span><strong>${formatCurrency(data.projectTotalEarned)}</strong></span>
              </div>
            </div>
          </div>

          <div class="pdf-footer">
            <p><strong>Observações:</strong></p>
            <p>Esta fatura refere-se aos serviços prestados no período indicado. Pagamento devido em ${data.invoiceDueDate}.</p>
            <p class="thank-you">Obrigado pelo seu negócio!</p>
          </div>
        </div>
      </body>
      </html>
    `
  }

  /**
   * Gera um número único para a fatura
   */
  static generateInvoiceNumber(): string {
    const timestamp = Date.now().toString(36)
    const random = Math.random().toString(36).substr(2, 9)
    return `INV-${new Date().getFullYear()}-${timestamp}-${random}`.toUpperCase()
  }

  /**
   * Calcula a data de vencimento (30 dias a partir de hoje)
   */
  static calculateDueDate(): string {
    const dueDate = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000)
    return dueDate.toLocaleDateString('pt-BR')
  }

  /**
   * Retorna a data atual formatada
   */
  static getCurrentDate(): string {
    return new Date().toLocaleDateString('pt-BR')
  }
}