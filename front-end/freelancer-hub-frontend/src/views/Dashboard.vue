<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue'
import BaseHeader from '../components/BaseHeader.vue'
import { fetchProjects, type ProjectReadDto } from '../services/projects'
import { fetchInvoices } from '../services/invoices'
import { fetchTaskItemsByProject } from '../services/taskItems'
import { fetchClients, type ClientReadDto } from '../services/clients'

// Configuração do i18n
const { t } = useI18n()

// Estado reativo para os dados
const projects = ref<ProjectReadDto[]>([])
const invoices = ref<any[]>([])
const tasks = ref<any[]>([])
const clients = ref<ClientReadDto[]>([])
const loading = ref(true)

// Métricas calculadas
const ongoingProjects = ref(0)
const monthlyInvoicesTotal = ref(0)
const monthlyInvoicesCount = ref(0)
const ongoingTasks = ref(0)
const totalClients = ref(0)

// Modelo para o BaseHeader
const dashboardModel = ref({
  totalProjects: 0,
  totalInvoices: 0,
  totalTasks: 0,
  totalClients: 0,
})

// Funções para buscar dados
const loadProjects = async () => {
  try {
    projects.value = await fetchProjects()
    ongoingProjects.value = projects.value.filter(p => p.status === 'Em Andamento').length
    dashboardModel.value.totalProjects = projects.value.length
  } catch (error) {
    console.error('Erro ao carregar projetos:', error)
  }
}

const loadInvoices = async () => {
  try {
    invoices.value = await fetchInvoices()
    dashboardModel.value.totalInvoices = invoices.value.length
    
    // Filtra invoices do mês atual e calcula totais
    const currentMonth = new Date().getMonth()
    const currentYear = new Date().getFullYear()
    
    const monthlyInvoices = invoices.value.filter(invoice => {
      const invoiceDate = new Date(invoice.issueDate)
      return invoiceDate.getMonth() === currentMonth && 
             invoiceDate.getFullYear() === currentYear
    })
    
    monthlyInvoicesCount.value = monthlyInvoices.length
    monthlyInvoicesTotal.value = monthlyInvoices.reduce((total, invoice) => total + invoice.amount, 0)
  } catch (error) {
    console.error('Erro ao carregar invoices:', error)
  }
}

const loadTasks = async () => {
  try {
    // Busca tarefas de todos os projetos
    const allTasks = []
    for (const project of projects.value) {
      const projectTasks = await fetchTaskItemsByProject(project.id)
      allTasks.push(...projectTasks)
    }
    
    tasks.value = allTasks
    ongoingTasks.value = allTasks.filter(task => task.status === 'Em Andamento').length
    dashboardModel.value.totalTasks = allTasks.length
  } catch (error) {
    console.error('Erro ao carregar tarefas:', error)
  }
}

const loadClients = async () => {
  try {
    clients.value = await fetchClients()
    totalClients.value = clients.value.length
    dashboardModel.value.totalClients = clients.value.length
  } catch (error) {
    console.error('Erro ao carregar clientes:', error)
  }
}

// Carrega todos os dados quando o componente é montado
onMounted(async () => {
  try {
    loading.value = true
    await loadProjects()
    await Promise.all([loadInvoices(), loadTasks(), loadClients()])
  } finally {
    loading.value = false
  }
})

// Formatação de valores
const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('pt-BR', {
    style: 'currency',
    currency: 'BRL'
  }).format(value)
}
</script>

<template>
  <AuthenticatedLayout>
    <div class="dashboard">
      <!-- Header usando BaseHeader -->
      <BaseHeader 
        :model="dashboardModel"
        modelName="dashboard"
      />
      
      <div v-if="loading" class="loading">
        <div class="loading-spinner"></div>
        <p>Carregando dados do dashboard...</p>
      </div>
      
      <div v-else class="dashboard-grid">
        <!-- Card: Projetos em Andamento -->
        <div class="metric-card large-card">
          <div class="metric-content">
            <div class="metric-header">
              <div class="metric-info">
                <h3>Projetos em Andamento</h3>
                <p class="metric-description">Projetos ativos no momento</p>
              </div>
              <div class="metric-icon projects">
                <i class="fas fa-chart-line"></i>
              </div>
            </div>
            <div class="metric-main">
              <div class="metric-value">{{ ongoingProjects }}</div>
              <div class="metric-trend">
              </div>
            </div>
            <div class="metric-footer">
              <span class="total-projects">{{ projects.length }} projetos no total</span>
            </div>
          </div>
        </div>

        <!-- Card: Faturas Mensais -->
        <div class="metric-card large-card">
          <div class="metric-content">
            <div class="metric-header">
              <div class="metric-info">
                <h3>Faturas do Mês</h3>
                <p class="metric-description">Resumo financeiro mensal</p>
              </div>
              <div class="metric-icon invoices">
                <i class="fas fa-money-bill-wave"></i>
              </div>
            </div>
            <div class="metric-main">
              <div class="metric-value">{{ formatCurrency(monthlyInvoicesTotal) }}</div>
              <div class="metric-trend">
              </div>
            </div>
            <div class="metric-footer">
              <span class="invoice-count">{{ monthlyInvoicesCount }} {{ monthlyInvoicesCount === 1 ? 'emissão' : 'emissões' }} este mês</span>
            </div>
          </div>
        </div>

        <!-- Card: Tarefas em Andamento -->
        <div class="metric-card large-card">
          <div class="metric-content">
            <div class="metric-header">
              <div class="metric-info">
                <h3>Tarefas em Andamento</h3>
                <p class="metric-description">Tarefas sendo executadas</p>
              </div>
              <div class="metric-icon tasks">
                <i class="fas fa-tasks"></i>
              </div>
            </div>
            <div class="metric-main">
              <div class="metric-value">{{ ongoingTasks }}</div>
              <div class="metric-trend">
              </div>
            </div>
            <div class="metric-footer">
              <span class="total-tasks">{{ tasks.length }} tarefas no total</span>
            </div>
          </div>
        </div>

        <!-- Card: Total de Clientes -->
        <div class="metric-card large-card">
          <div class="metric-content">
            <div class="metric-header">
              <div class="metric-info">
                <h3>Total de Clientes</h3>
                <p class="metric-description">Clientes cadastrados</p>
              </div>
              <div class="metric-icon clients">
                <i class="fas fa-users"></i>
              </div>
            </div>
            <div class="metric-main">
              <div class="metric-value">{{ totalClients }}</div>
              <div class="metric-trend">
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </AuthenticatedLayout>
</template>

<style scoped>
.dashboard {
  padding: 24px;
  min-height: 100vh;
  background: #f8fafc;
  grid-column: 1/-1;
  width: 100%;
}

.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80px 20px;
  color: #64748b;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #e2e8f0;
  border-left: 4px solid #3b82f6;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.dashboard-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 24px;
  max-width: 1400px;
  margin: 0 auto;
  margin-top: 24px;
}

.metric-card.large-card {
  background: white;
  border-radius: 16px;
  padding: 32px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  border: 1px solid #e2e8f0;
  transition: all 0.3s ease;
  min-height: 220px;
  display: flex;
  flex-direction: column;
}

.metric-card.large-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}

.metric-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.metric-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 24px;
}

.metric-info h3 {
  margin: 0 0 8px 0;
  color: #1e293b;
  font-size: 1.25rem;
  font-weight: 600;
}

.metric-description {
  color: #64748b;
  font-size: 0.95rem;
  margin: 0;
}

.metric-icon {
  width: 64px;
  height: 64px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  color: white;
}

.metric-icon.projects {
  background: linear-gradient(135deg, #6366f1, #8b5cf6);
}

.metric-icon.invoices {
  background: linear-gradient(135deg, #10b981, #34d399);
}

.metric-icon.tasks {
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
}

.metric-icon.clients {
  background: linear-gradient(135deg, #ec4899, #f472b6);
}

.metric-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.metric-value {
  font-size: 3.5rem;
  font-weight: 800;
  color: #1e293b;
  line-height: 1;
  margin-bottom: 12px;
}

.metric-trend {
  margin-bottom: 16px;
}

.trend-positive {
  color: #10b981;
  font-weight: 600;
  font-size: 0.9rem;
  padding: 4px 8px;
  background: #d1fae5;
  border-radius: 6px;
}

.trend-neutral {
  color: #6b7280;
  font-weight: 600;
  font-size: 0.9rem;
  padding: 4px 8px;
  background: #f3f4f6;
  border-radius: 6px;
}

.metric-footer {
  border-top: 1px solid #f1f5f9;
  padding-top: 16px;
  margin-top: auto;
}

.metric-footer span {
  color: #64748b;
  font-size: 0.9rem;
  font-weight: 500;
}

/* Responsividade */
@media (max-width: 1024px) {
  .dashboard-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  
  .metric-card.large-card {
    padding: 24px;
  }
  
  .metric-value {
    font-size: 3rem;
  }
}

@media (max-width: 768px) {
  .dashboard {
    padding: 16px;
  }
  
  .metric-card.large-card {
    padding: 20px;
    min-height: 180px;
  }
  
  .metric-value {
    font-size: 2.5rem;
  }
  
  .metric-icon {
    width: 52px;
    height: 52px;
    font-size: 1.3rem;
  }
}

@media (max-width: 480px) {
  .metric-header {
    flex-direction: column;
    gap: 16px;
  }
  
  .metric-icon {
    align-self: flex-start;
  }
}
</style>