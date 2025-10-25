<template>
  <div class="time-tracking-section">
    <div class="time-header">
      <h3 class="section-title">
        <i class="fas fa-clock"></i>
        Rastreamento de Horas de Trabalho
      </h3>
      <p class="time-description">
        Acompanhe o tempo gasto neste projeto e gerencie sua taxa horária
      </p>
    </div>

    <!-- Mensagens de erro e sucesso -->
    <div v-if="hoursError" class="error-message hours-error">
      <i class="fas fa-exclamation-triangle"></i>
      {{ hoursError }}
    </div>

    <div v-if="hoursSuccess" class="success-message hours-success">
      <i class="fas fa-check-circle"></i>
      {{ hoursSuccess }}
    </div>

    <!-- Mensagens específicas para invoices -->
    <div v-if="invoiceError" class="error-message invoice-error">
      <i class="fas fa-exclamation-triangle"></i>
      {{ invoiceError }}
    </div>

    <div v-if="invoiceSuccess" class="success-message invoice-success">
      <i class="fas fa-check-circle"></i>
      {{ invoiceSuccess }}
    </div>

    <div class="time-content">
      <!-- Timer -->
      <div class="time-card timer-section">
        <h4 class="card-title">
          <i class="fas fa-stopwatch"></i>
          Timer de Trabalho
        </h4>
        <div class="timer-container">
          <div class="timer">
            <div class="timer-time">{{ formattedTime }}</div>
            <div class="timer-hours" v-if="!editingTime">
              <span>{{ totalHours }} horas</span>
              <button class="edit-time-button" @click="startEditingTime" title="Editar horas">
                <i class="fas fa-edit"></i>
              </button>
            </div>
            <div class="timer-edit" v-else>
            <input 
              v-model="tempHours" 
              type="number" 
              step="0.01" 
              class="time-input"
              @keyup.enter="saveTimeEdit"
              @keyup.escape="cancelTimeEdit"
              placeholder="0.00"
            />
            <span>horas</span>
            <div class="edit-actions">
              <button class="save-time-button" @click="saveTimeEdit" title="Salvar">
                <i class="fas fa-check"></i>
              </button>
              <button class="cancel-time-button" @click="cancelTimeEdit" title="Cancelar">
                <i class="fas fa-times"></i>
              </button>
            </div>
          </div>
          </div>

          <div class="timer-controls">
            <button 
              class="timer-button play-pause-button" 
              :class="{ 'paused': !isTimerRunning }"
              @click="toggleTimer"
              :disabled="editingTime"
            >
              <i :class="isTimerRunning ? 'fas fa-pause' : 'fas fa-play'"></i>
              {{ isTimerRunning ? 'Pausar' : 'Iniciar' }}
            </button>
            <button 
              class="timer-button reset-button" 
              @click="resetTimer"
              :disabled="totalSeconds === 0 || editingTime"
            >
              <i class="fas fa-redo"></i>
              Reiniciar
            </button>
            <button 
              class="timer-button save-button" 
              @click="saveTimerHours"
              :disabled="(totalSeconds === 0 && !editingTime) || isTimerRunning"
            >
              <i class="fas fa-save"></i>
              Salvar Horas
            </button>
          </div>
        </div>
      </div>

      <!-- Resumo e Taxa Horária -->
      <div class="time-card summary-section">
        <h4 class="card-title">
          <i class="fas fa-chart-bar"></i>
          Resumo do Projeto
        </h4>
        
        <div class="hourly-rate-section">
          <div class="rate-display" v-if="!editingHourlyRate">
            <div class="rate-info">
              <span class="rate-label">Taxa Horária:</span>
              <span class="rate-value">{{ formatCurrency(projectHourlyRate) }}/h</span>
            </div>
            <button class="edit-rate-button" @click="editHourlyRate" title="Editar taxa">
              <i class="fas fa-edit"></i>
            </button>
          </div>
          <div class="rate-edit" v-else>
            <div class="rate-input-group">
              <span class="currency-symbol">R$</span>
              <input
                v-model="tempHourlyRate"
                type="number"
                step="0.01"
                min="0"
                class="rate-input"
                @keyup.enter="updateHourlyRate"
                @keyup.escape="cancelEditHourlyRate"
              />
              <span class="rate-suffix">/h</span>
            </div>
            <div class="rate-actions">
              <button class="save-rate-button" @click="updateHourlyRate" title="Salvar">
                <i class="fas fa-check"></i>
              </button>
              <button class="cancel-rate-button" @click="cancelEditHourlyRate" title="Cancelar">
                <i class="fas fa-times"></i>
              </button>
            </div>
          </div>
        </div>

        <div class="summary-stats">
          <div class="stat-item">
            <div class="stat-value">{{ projectTotalHours.toFixed(2) }}h</div>
            <div class="stat-label">Total de Horas</div>
          </div>
          <div class="stat-item">
            <div class="stat-value">{{ formatCurrency(projectHourlyRate) }}/h</div>
            <div class="stat-label">Taxa Horária</div>
          </div>
          <div class="stat-item total-earned">
            <div class="stat-value">{{ formatCurrency(projectTotalEarned) }}</div>
            <div class="stat-label">Valor Total</div>
          </div>
        </div>

        <!-- Botão para gerar invoice -->
        <div class="invoice-section">
          <button 
            class="invoice-button" 
            @click="generateInvoiceWithPdf"
            :disabled="isGeneratingInvoice || projectTotalEarned <= 0"
          >
            <i class="fas fa-file-invoice-dollar"></i>
            {{ isGeneratingInvoice ? 'Gerando Invoice...' : 'Gerar Invoice com PDF' }}
          </button>
          <p class="invoice-help">
            Gere um invoice com o valor total de <strong>{{ formatCurrency(projectTotalEarned) }}</strong>
          </p>
        </div>
      </div>
    </div>

    <div v-if="isLoadingHours" class="loading-hours">
      <i class="fas fa-spinner fa-spin"></i>
      Carregando dados de horas...
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onUnmounted, watch } from 'vue'
import { formatCurrency } from '../services/projects'
import { createInvoice, updateInvoice } from '../services/invoices'
import { PdfService, type PdfInvoiceData } from '../services/pdf'

const props = defineProps<{
  project: any
  hoursSummary: any
  isLoadingHours: boolean
  hoursError: string
  hoursSuccess: string
}>()

const emit = defineEmits<{
  'save-timer-hours': []
  'add-manual-hours': [hours: number, description: string]
  'update-hourly-rate': [rate: number]
  'load-hours-summary': []
}>()

// Variáveis para controle de tempo
const isTimerRunning = ref(false)
const totalSeconds = ref(0)
const timerInterval = ref<NodeJS.Timeout | null>(null)
const editingTime = ref(false)
const tempHours = ref('')

// Variáveis para edição da taxa horária
const editingHourlyRate = ref(false)
const tempHourlyRate = ref('')

// Variáveis para geração de invoice
const isGeneratingInvoice = ref(false)
const invoiceError = ref('')
const invoiceSuccess = ref('')
const pdfTemplate = ref<HTMLElement | null>(null)

// Computed para formatar o tempo
const formattedTime = computed(() => {
  const hours = Math.floor(totalSeconds.value / 3600)
  const minutes = Math.floor((totalSeconds.value % 3600) / 60)
  const seconds = totalSeconds.value % 60
  
  return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`
})

const totalHours = computed(() => {
  return (totalSeconds.value / 3600).toFixed(2)
})

// Computed para dados do projeto
const projectTotalHours = computed(() => {
  return props.project?.totalHours || 0
})

const projectHourlyRate = computed(() => {
  return props.project?.hourlyRate || 0
})

const projectTotalEarned = computed(() => {
  return projectTotalHours.value * projectHourlyRate.value
})

// Computed para dados do cliente e projeto
const clientName = computed(() => {
  return props.project?.client?.name || 'Cliente'
})

const clientEmail = computed(() => {
  return props.project?.client?.email || 'cliente@email.com'
})

const projectName = computed(() => {
  return props.project?.name || 'Projeto'
})

const projectDescription = computed(() => {
  return props.project?.description || 'Serviços de desenvolvimento'
})

// Variáveis para o PDF
const currentInvoiceNumber = ref(`INV-${new Date().getFullYear()}-${Math.random().toString(36).substr(2, 9).toUpperCase()}`)
const invoiceIssueDate = ref(new Date().toLocaleDateString('pt-BR'))
const invoiceDueDate = ref(new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toLocaleDateString('pt-BR'))
const invoiceStatus = ref('Pendente')

// Watch para limpar mensagens de erro/sucesso quando mudar
watch(() => props.hoursError, (newError) => {
  if (!newError) return
  // Auto-hide error after 5 seconds
  setTimeout(() => {
    if (props.hoursError === newError) {
      // Você pode emitir um evento para limpar o erro ou deixar o pai controlar
    }
  }, 5000)
})

watch(() => props.hoursSuccess, (newSuccess) => {
  if (!newSuccess) return
  // Auto-hide success after 3 seconds
  setTimeout(() => {
    if (props.hoursSuccess === newSuccess) {
      // Você pode emitir um evento para limpar o sucesso ou deixar o pai controlar
    }
  }, 3000)
})

// Função para gerar PDF usando window.print() como alternativa
async function generateInvoiceWithPdf() {
  if (projectTotalEarned.value <= 0) {
    invoiceError.value = 'Não é possível gerar invoice com valor zero'
    return
  }

  isGeneratingInvoice.value = true
  invoiceError.value = ''
  invoiceSuccess.value = ''

  try {
    // Gerar dados para o PDF usando o serviço
    const invoiceData: PdfInvoiceData = {
      invoiceNumber: PdfService.generateInvoiceNumber(),
      clientName: clientName.value,
      clientEmail: clientEmail.value,
      invoiceIssueDate: PdfService.getCurrentDate(),
      invoiceDueDate: PdfService.calculateDueDate(),
      invoiceStatus: 'Pendente',
      projectName: projectName.value,
      projectDescription: projectDescription.value,
      projectTotalHours: projectTotalHours.value,
      projectHourlyRate: projectHourlyRate.value,
      projectTotalEarned: projectTotalEarned.value
    }

    // Gerar PDF usando o serviço
    const pdfFilename = await PdfService.generateInvoicePdf(invoiceData)
    
    const today = new Date().toISOString().split('T')[0]
    const dueDate = new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0]

    // Criar invoice no banco de dados
    const invoiceDataForDb = {
      userId: props.project?.userId || '',
      clientId: props.project?.clientId || '',
      projectId: props.project?.id || '',
      issueDate: today,
      dueDate: dueDate,
      amount: projectTotalEarned.value,
      status: 'pending',
      pdfUrl: pdfFilename || ''
    }

    await createInvoice(invoiceDataForDb)
    
    invoiceSuccess.value = 'Invoice gerado com sucesso! O PDF foi aberto para impressão.'
    
    // Auto-hide success message after 5 seconds
    setTimeout(() => {
      invoiceSuccess.value = ''
    }, 5000)
  } catch (error) {
    console.error('Erro ao gerar invoice:', error)
    invoiceError.value = 'Erro ao gerar invoice. Tente novamente.'
    
    // Auto-hide error message after 5 seconds
    setTimeout(() => {
      invoiceError.value = ''
    }, 5000)
  } finally {
    isGeneratingInvoice.value = false
  }
}

// Funções do timer
function toggleTimer() {
  if (isTimerRunning.value) {
    // Pausar timer
    if (timerInterval.value) {
      clearInterval(timerInterval.value)
      timerInterval.value = null
    }
    isTimerRunning.value = false
  } else {
    // Iniciar timer
    timerInterval.value = setInterval(() => {
      totalSeconds.value += 1
    }, 1000)
    isTimerRunning.value = true
  }
}

function resetTimer() {
  if (timerInterval.value) {
    clearInterval(timerInterval.value)
    timerInterval.value = null
  }
  isTimerRunning.value = false
  totalSeconds.value = 0
  editingTime.value = false
}

function saveTimerHours() {
  if (editingTime.value) {
    saveTimeEdit()
  }
  
  const hoursToSave = parseFloat(totalHours.value)
  if (!isNaN(hoursToSave)) {
    emit('add-manual-hours', hoursToSave, 'Horas trabalhadas via timer')
    resetTimer()
  }
}

// Funções de edição de tempo
function startEditingTime() {
  // Pausar o timer ao editar
  if (isTimerRunning.value) {
    toggleTimer()
  }
  editingTime.value = true
  tempHours.value = totalHours.value
}

function saveTimeEdit() {
  const hours = parseFloat(tempHours.value)
  if (!isNaN(hours)) {
    totalSeconds.value = Math.round(hours * 3600)
    editingTime.value = false
  }
}

function cancelTimeEdit() {
  editingTime.value = false
  tempHours.value = totalHours.value
}

function editHourlyRate() {
  editingHourlyRate.value = true
  tempHourlyRate.value = projectHourlyRate.value.toString()
}

function cancelEditHourlyRate() {
  editingHourlyRate.value = false
  tempHourlyRate.value = projectHourlyRate.value.toString()
}

function updateHourlyRate() {
  const newRate = parseFloat(tempHourlyRate.value)
  if (isNaN(newRate) || newRate < 0) {
    return
  }
  emit('update-hourly-rate', newRate)
  editingHourlyRate.value = false
}

// Cleanup do timer quando o componente for destruído
onUnmounted(() => {
  if (timerInterval.value) {
    clearInterval(timerInterval.value)
  }
})
</script>

<style scoped>
.time-tracking-section {
  padding: 2rem;
}

.time-header {
  margin-bottom: 2rem;
  text-align: center;
}

.section-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0 0 0.5rem 0;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
}

.time-description {
  color: #64748b;
  margin: 0;
  font-size: 1rem;
}

.time-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.time-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  transition: all 0.3s ease;
}

.time-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.card-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0 0 1rem 0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.card-title i {
  color: #3b82f6;
}

/* Timer Section */
.timer-section {
  grid-column: 1;
}

.timer-container {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 16px;
  padding: 2rem;
  color: white;
  text-align: center;
  box-shadow: 0 8px 25px rgba(102, 126, 234, 0.3);
}

.timer {
  margin-bottom: 1.5rem;
}

.timer-time {
  font-size: 3rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
  font-family: 'Courier New', monospace;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.timer-hours {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  font-size: 1.125rem;
  opacity: 0.9;
}

.edit-time-button {
  background: rgba(255, 255, 255, 0.2);
  border: none;
  border-radius: 6px;
  padding: 0.375rem;
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
}

.edit-time-button:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: scale(1.1);
}

.timer-edit {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
}

.time-input {
  background: rgba(255, 255, 255, 0.9);
  border: 2px solid rgba(255, 255, 255, 0.5);
  border-radius: 8px;
  padding: 0.5rem 0.75rem;
  font-size: 1rem;
  width: 100px;
  text-align: center;
  color: #1e293b;
}

.time-input:focus {
  outline: none;
  border-color: white;
  background: white;
}

.edit-actions {
  display: flex;
  gap: 0.25rem;
}

.save-time-button,
.cancel-time-button {
  background: rgba(255, 255, 255, 0.2);
  border: none;
  border-radius: 6px;
  padding: 0.5rem;
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
}

.save-time-button:hover {
  background: rgba(34, 197, 94, 0.7);
}

.cancel-time-button:hover {
  background: rgba(239, 68, 68, 0.7);
}

.timer-controls {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
}

.timer-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.2);
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 50px;
  padding: 0.75rem 1.5rem;
  color: white;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.timer-button:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.timer-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.play-pause-button {
  background: rgba(34, 197, 94, 0.7);
  border-color: rgba(34, 197, 94, 0.5);
}

.play-pause-button.paused {
  background: rgba(59, 130, 246, 0.7);
  border-color: rgba(59, 130, 246, 0.5);
}

.reset-button {
  background: rgba(239, 68, 68, 0.7);
  border-color: rgba(239, 68, 68, 0.5);
}

.save-button {
  background: rgba(168, 85, 247, 0.7);
  border-color: rgba(168, 85, 247, 0.5);
}

/* Summary Section */
.summary-section {
  grid-column: 2;
}

.hourly-rate-section {
  margin-bottom: 1.5rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #e5e7eb;
}

.rate-display {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.rate-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.rate-label {
  font-size: 0.875rem;
  color: #6b7280;
}

.rate-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: #059669;
}

.edit-rate-button {
  background: #f3f4f6;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  padding: 0.5rem;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s ease;
}

.edit-rate-button:hover {
  background: #e5e7eb;
  border-color: #9ca3af;
}

.rate-edit {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
}

.rate-input-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex: 1;
}

.currency-symbol {
  font-weight: 600;
  color: #374151;
}

.rate-input {
  padding: 0.5rem 0.75rem;
  border: 1px solid #d1d5db;
  border-radius: 8px;
  font-size: 1rem;
  width: 120px;
  text-align: center;
}

.rate-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.rate-suffix {
  color: #6b7280;
  font-size: 0.875rem;
}

.rate-actions {
  display: flex;
  gap: 0.25rem;
}

.save-rate-button,
.cancel-rate-button {
  background: #f3f4f6;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  padding: 0.5rem;
  color: #374151;
  cursor: pointer;
  transition: all 0.2s ease;
}

.save-rate-button:hover {
  background: #dcfce7;
  border-color: #16a34a;
  color: #16a34a;
}

.cancel-rate-button:hover {
  background: #fef2f2;
  border-color: #dc2626;
  color: #dc2626;
}

.summary-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.stat-item {
  text-align: center;
  padding: 1rem;
  background: #f8fafc;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
}

.stat-item.total-earned {
  background: linear-gradient(135deg, #dbeafe, #eff6ff);
  border-color: #3b82f6;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 0.25rem;
}

.stat-item.total-earned .stat-value {
  color: #1d4ed8;
}

.stat-label {
  font-size: 0.875rem;
  color: #64748b;
  font-weight: 600;
}

/* Invoice Section */
.invoice-section {
  text-align: center;
  padding-top: 1.5rem;
  border-top: 1px solid #e5e7eb;
}

.invoice-button {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  background: linear-gradient(135deg, #10b981, #059669);
  border: none;
  border-radius: 50px;
  padding: 0.75rem 1.5rem;
  color: white;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
}

.invoice-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(16, 185, 129, 0.4);
  background: linear-gradient(135deg, #059669, #047857);
}

.invoice-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.invoice-help {
  margin-top: 0.75rem;
  font-size: 0.875rem;
  color: #6b7280;
}

.invoice-help strong {
  color: #059669;
}

/* Template do PDF (sempre oculto) */
.pdf-template {
  display: none !important;
}

/* Mensagens */
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
  grid-column: 1 / -1;
  width: 100%;
}

.success-message {
  background: #f0fdf4;
  color: #16a34a;
  padding: 0.75rem 1rem;
  border-radius: 8px;
  margin-bottom: 1rem;
  border: 1px solid #bbf7d0;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.loading-hours {
  text-align: center;
  padding: 2rem;
  color: #64748b;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
}

.loading-hours i {
  color: #3b82f6;
}

@media (max-width: 768px) {
  .time-tracking-section {
    padding: 1rem;
  }
  
  .time-content {
    grid-template-columns: 1fr;
  }
  
  .timer-section,
  .summary-section {
    grid-column: 1;
  }
  
  .timer-controls {
    flex-direction: column;
  }
  
  .summary-stats {
    grid-template-columns: 1fr;
  }
  
  .timer-time {
    font-size: 2rem;
  }
}
</style>