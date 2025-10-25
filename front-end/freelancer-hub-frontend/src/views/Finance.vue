<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue'
import { fetchInvoices, deleteInvoice } from '../services/invoices'
import { PdfService, type PdfInvoiceData } from '../services/pdf'
import { formatCurrency } from '../services/projects'

interface Invoice {
  id: string
  userId: string
  clientId: string
  projectId: string
  issueDate: string
  dueDate: string
  amount: number
  pdfUrl?: string
  projectName: string
  clientName: string
}

// Estados reativos
const invoices = ref<Invoice[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const searchQuery = ref('')
const currentPage = ref(1)
const itemsPerPage = ref(10)
const generatingPdf = ref<string | null>(null)

// Carregar invoices
const loadInvoices = async () => {
  try {
    loading.value = true
    error.value = null
    const data = await fetchInvoices()
    invoices.value = data
  } catch (err) {
    error.value = 'Erro ao carregar invoices'
    console.error(err)
  } finally {
    loading.value = false
  }
}

// Filtrar invoices baseado na busca
const filteredInvoices = computed(() => {
  if (!searchQuery.value) return invoices.value
  
  const query = searchQuery.value.toLowerCase()
  return invoices.value.filter(invoice => 
    invoice.clientName.toLowerCase().includes(query) ||
    invoice.projectName.toLowerCase().includes(query) ||
    invoice.amount.toString().includes(query)
  )
})

// Paginação
const paginatedInvoices = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredInvoices.value.slice(start, end)
})

const totalPages = computed(() => {
  return Math.ceil(filteredInvoices.value.length / itemsPerPage.value)
})

// Gerar PDF para invoice
const generateInvoicePdf = async (invoice: Invoice) => {
  try {
    generatingPdf.value = invoice.id
    
    const invoiceData: PdfInvoiceData = {
      invoiceNumber: invoice.id,
      clientName: invoice.clientName,
      clientEmail: 'cliente@email.com',
      invoiceIssueDate: new Date(invoice.issueDate).toLocaleDateString('pt-BR'),
      invoiceDueDate: new Date(invoice.dueDate).toLocaleDateString('pt-BR'),
      invoiceStatus: 'Emitido',
      projectName: invoice.projectName,
      projectDescription: 'Serviços de desenvolvimento',
      projectTotalHours: invoice.amount / 100,
      projectHourlyRate: 100,
      projectTotalEarned: invoice.amount
    }

    await PdfService.generateInvoicePdf(invoiceData)
    
  } catch (err) {
    console.error('Erro ao gerar PDF:', err)
    error.value = 'Erro ao gerar PDF do invoice'
    setTimeout(() => { error.value = null }, 5000)
  } finally {
    generatingPdf.value = null
  }
}

// Excluir invoice
const handleDelete = async (id: string) => {
  if (!confirm('Tem certeza que deseja excluir este invoice?')) return
  
  try {
    await deleteInvoice(id)
    await loadInvoices()
  } catch (err) {
    error.value = 'Erro ao excluir invoice'
    console.error(err)
    setTimeout(() => { error.value = null }, 5000)
  }
}

// Formatações
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('pt-BR')
}

// Carregar dados ao montar o componente
onMounted(() => {
  loadInvoices()
})
</script>

<template>
  <AuthenticatedLayout>
    <div class="invoices-page">
      <div class="page-header">
        <h1 class="page-title">
          <i class="fas fa-file-invoice-dollar"></i>
          Meus Invoices
        </h1>
        <p class="page-description">
          Gerencie e visualize todos os seus invoices
        </p>
      </div>
      
      <!-- Mensagens de erro -->
      <div v-if="error" class="error-message">
        <i class="fas fa-exclamation-triangle"></i>
        {{ error }}
      </div>

      <div class="invoices-container">
        <div class="invoices-header">
          <div class="header-content">
            <h2 class="invoices-title">
              <i class="fas fa-list"></i>
              Todos os Invoices
            </h2>
            <div class="invoices-actions">
              <div class="search-box">
                <i class="fas fa-search search-icon"></i>
                <input 
                  v-model="searchQuery" 
                  type="text" 
                  placeholder="Buscar por cliente, projeto ou valor..." 
                  class="search-input"
                >
              </div>
            </div>
          </div>
        </div>
        
        <div class="table-container">
          <!-- Loading State -->
          <div v-if="loading" class="loading-state">
            <i class="fas fa-spinner fa-spin loading-icon"></i>
            <p>Carregando invoices...</p>
          </div>
          
          <!-- Empty State -->
          <div v-else-if="invoices.length === 0" class="empty-state">
            <i class="fas fa-file-invoice empty-icon"></i>
            <p>Nenhum invoice encontrado</p>
            <button class="btn btn-primary">
              <i class="fas fa-plus"></i>
              Criar Primeiro Invoice
            </button>
          </div>
          
          <!-- Table -->
          <table v-else class="invoices-table">
            <thead>
              <tr>
                <th>Cliente</th>
                <th>Projeto</th>
                <th>Emissão</th>
                <th>Vencimento</th>
                <th>Valor</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="invoice in paginatedInvoices" :key="invoice.id">
                <td class="client-name">{{ invoice.clientName }}</td>
                <td class="project-name">{{ invoice.projectName }}</td>
                <td>{{ formatDate(invoice.issueDate) }}</td>
                <td>{{ formatDate(invoice.dueDate) }}</td>
                <td class="amount">{{ formatCurrency(invoice.amount) }}</td>
                <td class="actions">
                  <button 
                    @click="generateInvoicePdf(invoice)"
                    class="action-btn download-btn"
                    :title="'Baixar PDF'"
                  >
                    <i class="fas fa-download" ></i>
                  </button>
                  <button 
                    @click="handleDelete(invoice.id)"
                    class="action-btn delete-btn"
                    title="Excluir"
                  >
                    <i class="fas fa-trash"></i>
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <!-- Pagination -->
        <div v-if="!loading && invoices.length > 0" class="pagination">
          <div class="pagination-info">
            Mostrando {{ Math.min((currentPage - 1) * itemsPerPage + 1, filteredInvoices.length) }}-{{ Math.min(currentPage * itemsPerPage, filteredInvoices.length) }} de {{ filteredInvoices.length }} invoices
          </div>
          <div class="pagination-controls">
            <button 
              @click="currentPage--" 
              :disabled="currentPage === 1"
              class="pagination-btn"
            >
              <i class="fas fa-chevron-left"></i>
            </button>
            <button 
              v-for="page in totalPages" 
              :key="page"
              @click="currentPage = page"
              :class="['pagination-btn', { active: currentPage === page }]"
            >
              {{ page }}
            </button>
            <button 
              @click="currentPage++" 
              :disabled="currentPage === totalPages"
              class="pagination-btn"
            >
              <i class="fas fa-chevron-right"></i>
            </button>
          </div>
        </div>
      </div>
    </div>
  </AuthenticatedLayout>
</template>

<style scoped>
.invoices-page {
  padding: 2rem;
  background-color: #f5f7fa;
  min-height: 100vh;
  width: 100%;
  grid-column: 1/-1;
}

.page-header {
  margin-bottom: 2rem;
  text-align: center;
}

.page-title {
  font-size: 2rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0 0 0.5rem 0;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
}

.page-description {
  color: #64748b;
  margin: 0;
  font-size: 1rem;
}

.error-message {
  background: #fef2f2;
  color: #dc2626;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  margin-bottom: 1rem;
  border: 1px solid #fecaca;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  width: 100%;
}

.invoices-container {
  background-color: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  overflow: hidden;
  border: 1px solid #e2e8f0;
}

.invoices-header {
  padding: 1.5rem;
  border-bottom: 1px solid #e2e8f0;
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.invoices-title {
  font-size: 1.4rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.invoices-title i {
  color: #3b82f6;
}

.invoices-actions {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.search-box {
  position: relative;
}

.search-input {
  padding: 0.75rem 1rem 0.75rem 2.5rem;
  border-radius: 8px;
  border: 1px solid #d1d5db;
  width: 300px;
  font-size: 0.9rem;
  transition: all 0.3s ease;
  background: white;
}

.search-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.search-icon {
  position: absolute;
  left: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  color: #64748b;
}

.btn {
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.9rem;
}

.btn-primary {
  background: linear-gradient(135deg, #3b82f6, #1d4ed8);
  color: white;
  box-shadow: 0 2px 4px rgba(59, 130, 246, 0.3);
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(59, 130, 246, 0.4);
}

.btn-secondary {
  background: white;
  color: #374151;
  border: 1px solid #d1d5db;
}

.btn-secondary:hover:not(:disabled) {
  background: #f9fafb;
  border-color: #9ca3af;
}

.table-container {
  overflow-x: auto;
  min-height: 400px;
}

.invoices-table {
  width: 100%;
  border-collapse: collapse;
}

.invoices-table thead {
  background-color: #f8fafc;
}

.invoices-table th {
  padding: 1rem 1.5rem;
  text-align: left;
  font-weight: 600;
  color: #374151;
  border-bottom: 1px solid #e5e7eb;
  font-size: 0.9rem;
  white-space: nowrap;
}

.invoices-table td {
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #e5e7eb;
  color: #4b5563;
}

.invoices-table tbody tr {
  transition: background-color 0.2s ease;
}

.invoices-table tbody tr:hover {
  background-color: #f8fafc;
}

.client-name {
  font-weight: 600;
  color: #1e293b;
}

.project-name {
  color: #3b82f6;
  font-weight: 500;
}

.amount {
  font-weight: 700;
  color: #059669;
  font-size: 1.1rem;
}

.actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-start;
  margin-bottom: -1px;
}

.action-btn {
  background: none;
  border: none;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 6px;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 2.5rem;
  height: 2.5rem;
}

.download-btn {
  color: #3b82f6;
  background: #eff6ff;
  border: 1px solid #dbeafe;
}

.download-btn:hover:not(:disabled) {
  background: #dbeafe;
  transform: scale(1.05);
}

.download-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.delete-btn {
  color: #ef4444;
  background: #fef2f2;
  border: 1px solid #fecaca;
}

.delete-btn:hover {
  background: #fecaca;
  transform: scale(1.05);
}

.loading-state, .empty-state {
  padding: 3rem 2rem;
  text-align: center;
  color: #64748b;
}

.loading-icon {
  font-size: 2rem;
  color: #3b82f6;
  margin-bottom: 1rem;
}

.empty-icon {
  font-size: 3rem;
  color: #cbd5e1;
  margin-bottom: 1rem;
}

.empty-state p {
  margin-bottom: 1.5rem;
  font-size: 1.1rem;
}

.pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-top: 1px solid #e5e7eb;
  background: #f8fafc;
}

.pagination-info {
  color: #64748b;
  font-size: 0.9rem;
  font-weight: 500;
}

.pagination-controls {
  display: flex;
  gap: 0.5rem;
}

.pagination-btn {
  padding: 0.5rem 0.75rem;
  border-radius: 6px;
  border: 1px solid #d1d5db;
  background-color: white;
  cursor: pointer;
  transition: all 0.2s ease;
  min-width: 2.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #374151;
}

.pagination-btn:hover:not(:disabled) {
  background-color: #f3f4f6;
  border-color: #9ca3af;
}

.pagination-btn.active {
  background-color: #3b82f6;
  color: white;
  border-color: #3b82f6;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Responsividade */
@media (max-width: 1024px) {
  .invoices-page {
    padding: 1rem;
  }
  
  .header-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
  
  .invoices-actions {
    width: 100%;
    justify-content: space-between;
  }
  
  .search-box {
    flex: 1;
  }
  
  .search-input {
    width: 100%;
  }
}

@media (max-width: 768px) {
  .invoices-actions {
    flex-direction: column;
    width: 100%;
    gap: 0.75rem;
  }
  
  .search-box {
    width: 100%;
  }
  
  .pagination {
    flex-direction: column;
    gap: 1rem;
  }
  
  .invoices-table {
    font-size: 0.8rem;
  }
  
  .invoices-table th,
  .invoices-table td {
    padding: 0.75rem 1rem;
  }
  
  .actions {
    flex-direction: column;
    gap: 0.25rem;
  }
  
  .action-btn {
    width: 2rem;
    height: 2rem;
  }
}

@media (max-width: 480px) {
  .page-title {
    font-size: 1.5rem;
  }
  
  .invoices-title {
    font-size: 1.2rem;
  }
  
  .btn {
    padding: 0.6rem 1rem;
    font-size: 0.8rem;
  }
}
</style>