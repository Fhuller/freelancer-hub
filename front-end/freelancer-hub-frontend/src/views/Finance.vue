<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue'
import { fetchInvoices, deleteInvoice } from '../services/invoices'

interface Invoice {
  id: string
  userId: string
  clientId: string
  projectId: string
  issueDate: string
  dueDate: string
  amount: number
  status: string
  pdfUrl?: string
}

// Estados reativos
const invoices = ref<Invoice[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const searchQuery = ref('')
const currentPage = ref(1)
const itemsPerPage = ref(10)

// Carregar invoices
const loadInvoices = async () => {
  try {
    loading.value = true
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
    invoice.clientId.toLowerCase().includes(query) ||
    invoice.projectId.toLowerCase().includes(query) ||
    invoice.status.toLowerCase().includes(query) ||
    invoice.id.toLowerCase().includes(query)
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

// Excluir invoice
const handleDelete = async (id: string) => {
  if (!confirm('Tem certeza que deseja excluir este invoice?')) return
  
  try {
    await deleteInvoice(id)
    await loadInvoices()
  } catch (err) {
    error.value = 'Erro ao excluir invoice'
    console.error(err)
  }
}

// Formatações
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(amount)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('pt-BR')
}

const getStatusClass = (status: string) => {
  const statusMap: { [key: string]: string } = {
    'paid': 'status-paid',
    'pending': 'status-pending',
    'overdue': 'status-overdue',
    'draft': 'status-draft'
  }
  return statusMap[status] || 'status-draft'
}

const getStatusText = (status: string) => {
  const statusMap: { [key: string]: string } = {
    'paid': 'Pago',
    'pending': 'Pendente',
    'overdue': 'Atrasado',
    'draft': 'Rascunho'
  }
  return statusMap[status] || status
}

// Carregar dados ao montar o componente
onMounted(() => {
  loadInvoices()
})
</script>

<template>
  <AuthenticatedLayout>
    <div class="invoices-page">
      <div class="header">
        <h1 class="page-title">Meus Invoices</h1>
      </div>
      
      <div class="invoices-container">
        <div class="invoices-header">
          <h2 class="invoices-title">Todos os Invoices</h2>
          <div class="invoices-actions">
            <div class="search-box">
              <span class="search-icon">🔍</span>
              <input 
                v-model="searchQuery" 
                type="text" 
                placeholder="Buscar invoice..." 
                class="search-input"
              >
            </div>
            <button class="btn btn-secondary">
              <span class="btn-icon">📥</span>
              Exportar
            </button>
            <button class="btn btn-primary">
              <span class="btn-icon">➕</span>
              Novo Invoice
            </button>
          </div>
        </div>
        
        <div class="table-container">
          <!-- Loading State -->
          <div v-if="loading" class="loading-state">
            <div class="loading-spinner"></div>
            <p>Carregando invoices...</p>
          </div>
          
          <!-- Error State -->
          <div v-else-if="error" class="error-state">
            <div class="error-icon">⚠️</div>
            <p>{{ error }}</p>
            <button @click="loadInvoices" class="btn btn-primary">Tentar Novamente</button>
          </div>
          
          <!-- Empty State -->
          <div v-else-if="invoices.length === 0" class="empty-state">
            <div class="empty-icon">📄</div>
            <p>Nenhum invoice encontrado</p>
            <button class="btn btn-primary">Criar Primeiro Invoice</button>
          </div>
          
          <!-- Table -->
          <table v-else class="invoices-table">
            <thead>
              <tr>
                <th>Nº Invoice</th>
                <th>Cliente</th>
                <th>Projeto</th>
                <th>Emissão</th>
                <th>Vencimento</th>
                <th>Valor</th>
                <th>Status</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="invoice in paginatedInvoices" :key="invoice.id">
                <td>{{ invoice.id }}</td>
                <td>{{ invoice.clientId }}</td>
                <td>{{ invoice.projectId }}</td>
                <td>{{ formatDate(invoice.issueDate) }}</td>
                <td>{{ formatDate(invoice.dueDate) }}</td>
                <td class="amount">{{ formatCurrency(invoice.amount) }}</td>
                <td>
                  <span :class="['status', getStatusClass(invoice.status)]">
                    {{ getStatusText(invoice.status) }}
                  </span>
                </td>
                <td class="actions">
                  <button 
                    v-if="invoice.pdfUrl" 
                    :href="invoice.pdfUrl"
                    target="_blank"
                    class="action-btn"
                    title="Visualizar PDF"
                  >
                    👁️
                  </button>
                  <button class="action-btn" title="Editar">
                    ✏️
                  </button>
                  <button 
                    @click="handleDelete(invoice.id)"
                    class="action-btn delete"
                    title="Excluir"
                  >
                    🗑️
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
              ‹
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
              ›
            </button>
          </div>
        </div>
      </div>
    </div>
  </AuthenticatedLayout>
</template>

<style scoped>
.invoices-page {
  padding: 20px;
  background-color: #f5f7fa;
  min-height: 100vh;
}

.header {
  margin-bottom: 30px;
}

.page-title {
  font-size: 2rem;
  font-weight: 600;
  color: #2c3e50;
  margin: 0;
}

.invoices-container {
  background-color: white;
  border-radius: 10px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  overflow: hidden;
}

.invoices-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 25px;
  border-bottom: 1px solid #eee;
}

.invoices-title {
  font-size: 1.4rem;
  font-weight: 600;
  color: #2c3e50;
  margin: 0;
}

.invoices-actions {
  display: flex;
  gap: 10px;
  align-items: center;
}

.search-box {
  position: relative;
  margin-right: 10px;
}

.search-input {
  padding: 8px 15px 8px 35px;
  border-radius: 5px;
  border: 1px solid #ddd;
  width: 200px;
  font-size: 0.9rem;
  transition: border-color 0.3s;
}

.search-input:focus {
  outline: none;
  border-color: #3498db;
}

.search-icon {
  position: absolute;
  left: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: #7f8c8d;
}

.btn {
  padding: 8px 16px;
  border-radius: 5px;
  border: none;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 0.9rem;
}

.btn-primary {
  background-color: #3498db;
  color: white;
}

.btn-primary:hover {
  background-color: #2980b9;
}

.btn-secondary {
  background-color: #ecf0f1;
  color: #2c3e50;
}

.btn-secondary:hover {
  background-color: #d5dbdb;
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
  background-color: #f8f9fa;
}

.invoices-table th {
  padding: 15px 20px;
  text-align: left;
  font-weight: 600;
  color: #2c3e50;
  border-bottom: 1px solid #eee;
  font-size: 0.9rem;
}

.invoices-table td {
  padding: 15px 20px;
  border-bottom: 1px solid #eee;
  color: #555;
}

.invoices-table tbody tr {
  transition: background-color 0.2s;
}

.invoices-table tbody tr:hover {
  background-color: #f8f9fa;
}

.status {
  padding: 5px 10px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 500;
  text-align: center;
  display: inline-block;
  min-width: 80px;
}

.status-paid {
  background-color: #d4edda;
  color: #155724;
}

.status-pending {
  background-color: #fff3cd;
  color: #856404;
}

.status-overdue {
  background-color: #f8d7da;
  color: #721c24;
}

.status-draft {
  background-color: #e2e3e5;
  color: #383d41;
}

.amount {
  font-weight: 600;
  color: #2c3e50;
}

.actions {
  display: flex;
  gap: 10px;
}

.action-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: #7f8c8d;
  transition: color 0.3s;
  font-size: 1rem;
  padding: 5px;
}

.action-btn:hover {
  color: #3498db;
}

.action-btn.delete:hover {
  color: #e74c3c;
}

.loading-state, .error-state, .empty-state {
  padding: 50px 20px;
  text-align: center;
  color: #7f8c8d;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 15px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-icon, .empty-icon {
  font-size: 3rem;
  margin-bottom: 15px;
}

.empty-state p, .error-state p {
  margin-bottom: 20px;
}

.pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 25px;
  border-top: 1px solid #eee;
}

.pagination-info {
  color: #7f8c8d;
  font-size: 0.9rem;
}

.pagination-controls {
  display: flex;
  gap: 5px;
}

.pagination-btn {
  padding: 8px 12px;
  border-radius: 5px;
  border: 1px solid #ddd;
  background-color: white;
  cursor: pointer;
  transition: all 0.3s;
  min-width: 40px;
}

.pagination-btn:hover:not(:disabled) {
  background-color: #f8f9fa;
}

.pagination-btn.active {
  background-color: #3498db;
  color: white;
  border-color: #3498db;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Responsividade */
@media (max-width: 992px) {
  .invoices-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 15px;
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
  .invoices-page {
    padding: 15px;
  }
  
  .invoices-actions {
    flex-direction: column;
    width: 100%;
    gap: 10px;
  }
  
  .search-box {
    width: 100%;
    margin-right: 0;
  }
  
  .pagination {
    flex-direction: column;
    gap: 15px;
  }
  
  .actions {
    flex-direction: column;
    gap: 5px;
  }
}
</style>