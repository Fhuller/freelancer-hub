<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import { fetchProjectById, type ProjectReadDto } from '../services/projects'
import {
  fetchTaskItemsByProject,
  createTaskItem,
  updateTaskItem,
  deleteTaskItem
} from '../services/taskItems'
import { 
  uploadProjectFile, 
  getProjectFiles, 
  deleteProjectFile, 
  type FileDto,
  getProjectHoursSummary,
  addHoursToProject,
  updateProjectHourlyRate,
  type ProjectHoursSummaryDto,
  formatCurrency
} from '../services/projects'
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue'
import BaseHeader from '../components/BaseHeader.vue'
import BaseModal from '../components/BaseModal.vue'
import ProjectKanban from '../components/ProjectKanban.vue'
import ProjectFiles from '../components/ProjectFiles.vue'
import ProjectHours from '../components/ProjectHours.vue'

const route = useRoute()

const project = ref<ProjectReadDto | null>(null)
const tasks = ref<any[]>([])
const files = ref<FileDto[]>([])
const hoursSummary = ref<ProjectHoursSummaryDto | null>(null)

const isLoading = ref(false)
const isLoadingTasks = ref(false)
const isLoadingFiles = ref(false)
const isLoadingHours = ref(false)
const error = ref('')
const fileError = ref('')
const hoursError = ref('')
const hoursSuccess = ref('')

const showModal = ref(false)
const editingTask = ref<any | null>(null)

// Variáveis para abas
const activeTab = ref('kanban')

// Variáveis para drag and drop
const draggedTask = ref<any>(null)
const dragOverColumn = ref<string | null>(null)

// Helper para formatar data no formato YYYY-MM-DD
const formatDateForInput = (date: Date) => {
  return date.toISOString().split('T')[0]
}

// Helper para criar data de amanhã (para evitar data anterior à atual)
const getTomorrowDate = () => {
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  return tomorrow
}

const taskTemplate = ref({
  title: '',
  description: '',
  status: 'Pendente',
  dueDate: formatDateForInput(getTomorrowDate())
})

const taskTemplateDisplay = {
  title: 'Título da Tarefa',
  description: 'Descrição da tarefa',
  status: ['Pendente', 'Em Andamento', 'Concluída', 'Cancelada'],
  dueDate: 'Data de Vencimento'
}

// Watcher para quando a aba de horas for ativada
watch(activeTab, (newTab) => {
  if (newTab === 'time' && project.value) {
    loadHoursSummary()
  }
})

// NOVAS FUNÇÕES PARA HORAS
async function loadHoursSummary() {
  if (!project.value) return
  try {
    isLoadingHours.value = true
    hoursError.value = ''
    hoursSummary.value = await getProjectHoursSummary(project.value.id)
    
    // Atualizar o projeto com os dados mais recentes
    if (project.value && hoursSummary.value) {
      project.value.totalHours = hoursSummary.value.totalHours
      project.value.hourlyRate = hoursSummary.value.hourlyRate
    }
  } catch (err: any) {
    console.error('Erro ao carregar resumo de horas:', err)
    hoursError.value = err.message || 'Erro ao carregar dados de horas'
  } finally {
    isLoadingHours.value = false
  }
}

async function saveTimerHours() {
  if (!project.value) return
  
  try {
    hoursError.value = ''
    hoursSuccess.value = ''
    
    await addHoursToProject(
      project.value.id, 
      0, // Será calculado no componente filho
      'Horas trabalhadas via timer'
    )
    
    hoursSuccess.value = 'Horas salvas com sucesso!'
    
    // Recarregar o resumo
    await loadHoursSummary()
    
    // Limpar mensagem de sucesso após 3 segundos
    setTimeout(() => {
      hoursSuccess.value = ''
    }, 3000)
  } catch (err: any) {
    console.error('Erro ao salvar horas do timer:', err)
    hoursError.value = err.message || 'Erro ao salvar horas'
  }
}

async function addManualHours(hours: number, description: string) {
  if (!project.value) return
  
  try {
    hoursError.value = ''
    hoursSuccess.value = ''
    
    await addHoursToProject(
      project.value.id, 
      hours, 
      description
    )
    
    hoursSuccess.value = `${hours} horas adicionadas com sucesso!`
    
    // Recarregar o resumo
    await loadHoursSummary()
    
    // Limpar mensagem de sucesso após 3 segundos
    setTimeout(() => {
      hoursSuccess.value = ''
    }, 3000)
  } catch (err: any) {
    console.error('Erro ao adicionar horas manuais:', err)
    hoursError.value = err.message || 'Erro ao adicionar horas'
  }
}

async function updateHourlyRate(rate: number) {
  if (!project.value) return

  try {
    hoursError.value = ''
    hoursSuccess.value = ''
    
    await updateProjectHourlyRate(project.value.id, rate)
    
    hoursSuccess.value = `Taxa horária atualizada para ${formatCurrency(rate)}!`
    
    // Recarregar o resumo
    await loadHoursSummary()
    
    // Limpar mensagem de sucesso após 3 segundos
    setTimeout(() => {
      hoursSuccess.value = ''
    }, 3000)
  } catch (err: any) {
    console.error('Erro ao atualizar taxa horária:', err)
    hoursError.value = err.message || 'Erro ao atualizar taxa horária'
  }
}

async function loadProject() {
  try {
    isLoading.value = true
    error.value = ''
    const data = await fetchProjectById(route.params.projectId as string)
    project.value = data
  } catch (err) {
    console.error(err)
    error.value = 'Erro ao carregar projeto'
  } finally {
    isLoading.value = false
  }
}

async function loadTasks() {
  if (!project.value) return
  try {
    isLoadingTasks.value = true
    error.value = ''
    tasks.value = await fetchTaskItemsByProject(project.value.id)
  } catch (err) {
    console.error(err)
    error.value = 'Erro ao carregar tarefas'
  } finally {
    isLoadingTasks.value = false
  }
}

// Funções para arquivos
async function loadFiles() {
  if (!project.value) return
  try {
    isLoadingFiles.value = true
    fileError.value = ''
    files.value = await getProjectFiles(project.value.id)
  } catch (err) {
    console.error('Erro ao carregar arquivos:', err)
    fileError.value = 'Erro ao carregar arquivos'
  } finally {
    isLoadingFiles.value = false
  }
}

async function handleFileUpload(event: Event) {
  if (!project.value) return
  
  const input = event.target as HTMLInputElement
  const file = input.files?.[0]
  
  if (!file) return

  // Validar tamanho do arquivo (10MB)
  if (file.size > 10 * 1024 * 1024) {
    fileError.value = 'Arquivo muito grande. Tamanho máximo: 10MB'
    return
  }

  try {
    fileError.value = ''
    await uploadProjectFile(project.value.id, file)
    await loadFiles() // Recarregar a lista
    
    // Limpar o input
    input.value = ''
  } catch (err: any) {
    console.error('Erro ao fazer upload:', err)
    fileError.value = err.message || 'Erro ao fazer upload do arquivo'
  }
}

async function handleDeleteFile(fileId: string) {
  if (!project.value) return
  
  if (!confirm('Tem certeza que deseja excluir este arquivo?')) {
    return
  }

  try {
    fileError.value = ''
    await deleteProjectFile(project.value.id, fileId)
    await loadFiles() // Recarregar a lista
  } catch (err: any) {
    console.error('Erro ao deletar arquivo:', err)
    fileError.value = err.message || 'Erro ao deletar arquivo'
  }
}

function handleDownloadFile(file: FileDto) {
  // Criar link temporário para download
  const link = document.createElement('a')
  link.href = file.fileUrl
  link.download = `${file.fileName}${file.fileExtension}`
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}

function openNewTaskModal(status: string) {
  taskTemplate.value = {
    title: '',
    description: '',
    status: status,
    dueDate: formatDateForInput(getTomorrowDate())
  }
  editingTask.value = null
  showModal.value = true
}

function editTask(task: any) {
  editingTask.value = task
  
  // Formatar a data corretamente para o input
  let dueDate = getTomorrowDate()
  if (task.dueDate) {
    dueDate = new Date(task.dueDate)
    // Se a data for anterior a hoje, usar amanhã
    if (dueDate < new Date()) {
      dueDate = getTomorrowDate()
    }
  }
  
  taskTemplate.value = {
    title: task.title,
    description: task.description || '',
    status: task.status || 'Pendente',
    dueDate: formatDateForInput(dueDate)
  }
  showModal.value = true
}

// Função para validar se a data é válida (não é passada)
function validateDueDate(dateString: string): boolean {
  const selectedDate = new Date(dateString)
  const today = new Date()
  today.setHours(0, 0, 0, 0) // Zerar horas para comparar apenas a data
  
  return selectedDate >= today
}

async function saveTask(data: Record<string, any>) {
  if (!project.value) return
  
  // Validar a data de vencimento
  if (data.dueDate && !validateDueDate(data.dueDate)) {
    error.value = 'A data de vencimento não pode ser anterior à data atual'
    return
  }
  
  try {
    // Formatar os dados para a API
    const taskData = {
      title: data.title,
      description: data.description || '',
      status: data.status,
      dueDate: data.dueDate || null
    }
    
    if (editingTask.value) {
      await updateTaskItem(editingTask.value.id, taskData)
      editingTask.value = null
    } else {
      await createTaskItem({
        projectId: project.value.id,
        ...taskData
      })
    }
    
    await loadTasks()
    showModal.value = false
    error.value = '' // Limpar erro se sucesso
  } catch (err: any) {
    console.error('Erro ao salvar tarefa:', err)
    
    // Verificar se o erro é específico sobre a data
    if (err.message && err.message.includes('data de vencimento')) {
      error.value = 'A data de vencimento não pode ser anterior à data atual'
    } else {
      error.value = 'Erro ao salvar tarefa'
    }
  }
}

async function removeTask(task: any) {
  try {
    await deleteTaskItem(task.id)
    await loadTasks()
  } catch (err) {
    console.error('Erro ao excluir tarefa:', err)
    error.value = 'Erro ao excluir tarefa'
  }
}

// Funções de Drag and Drop
function onDragStart(task: any) {
  draggedTask.value = task
}

function onDragOver(event: DragEvent, status: string) {
  event.preventDefault()
  dragOverColumn.value = status
}

function onDragLeave() {
  dragOverColumn.value = null
}

async function onDrop(status: string) {
  if (!draggedTask.value) return
  
  // Se a tarefa já está na mesma coluna, não faz nada
  if (draggedTask.value.status === status) {
    draggedTask.value = null
    dragOverColumn.value = null
    return
  }
  
  try {
    // Formatar os dados para a API
    const updateData = {
      title: draggedTask.value.title,
      description: draggedTask.value.description || '',
      status: status,
      dueDate: draggedTask.value.dueDate || null
    }
    
    // Atualiza o status da tarefa
    await updateTaskItem(draggedTask.value.id, updateData)
    
    // Recarrega as tarefas para refletir a mudança
    await loadTasks()
  } catch (err: any) {
    console.error('Erro ao atualizar status da tarefa:', err)
    
    // Verificar se o erro é sobre data
    if (err.message && err.message.includes('data de vencimento')) {
      error.value = 'Erro: A tarefa possui uma data de vencimento inválida'
    } else {
      error.value = 'Erro ao mover tarefa'
    }
  } finally {
    draggedTask.value = null
    dragOverColumn.value = null
  }
}

onMounted(async () => {
  await loadProject()
  await loadTasks()
  await loadFiles()
  // Não carregar horas summary aqui - será carregado quando a aba for ativada
})
</script>

<template>
  <AuthenticatedLayout :loading="isLoading || isLoadingTasks">
    <div v-if="error" class="error-message">{{ error }}</div>

    <template v-else>
      <BaseHeader
        v-if="project"
        :model="project"
        model-name="project"
      />

      <!-- ABA DE CONTEÚDO -->
      <div class="content-tabs" style="grid-column: 1 / -1;">
        <div class="tabs-header">
          <button 
            class="tab-button" 
            :class="{ active: activeTab === 'kanban' }"
            @click="activeTab = 'kanban'"
          >
            <i class="fas fa-columns"></i>
            Kanban
          </button>
          <button 
            class="tab-button" 
            :class="{ active: activeTab === 'files' }"
            @click="activeTab = 'files'"
          >
            <i class="fas fa-folder-open"></i>
            Arquivos
            <span class="tab-badge" v-if="files.length > 0">{{ files.length }}</span>
          </button>
          <button 
            class="tab-button" 
            :class="{ active: activeTab === 'time' }"
            @click="activeTab = 'time'"
          >
            <i class="fas fa-clock"></i>
            Rastreamento de Horas
          </button>
        </div>

        <div class="tab-content">
          <!-- ABA KANBAN -->
          <div v-if="activeTab === 'kanban'" class="tab-pane">
            <ProjectKanban
              :tasks="tasks"
              @drag-start="onDragStart"
              @drag-over="onDragOver"
              @drag-leave="onDragLeave"
              @drop="onDrop"
              @open-new-task-modal="openNewTaskModal"
              @edit-task="editTask"
              @remove-task="removeTask"
            />
          </div>

          <!-- ABA ARQUIVOS -->
          <div v-if="activeTab === 'files'" class="tab-pane">
            <ProjectFiles
              :files="files"
              :isLoadingFiles="isLoadingFiles"
              :fileError="fileError"
              @file-upload="handleFileUpload"
              @download-file="handleDownloadFile"
              @delete-file="handleDeleteFile"
            />
          </div>

          <!-- ABA RASTREAMENTO DE HORAS - ATUALIZADA -->
          <div v-if="activeTab === 'time'" class="tab-pane">
            <ProjectHours
              :project="project"
              :hoursSummary="hoursSummary"
              :isLoadingHours="isLoadingHours"
              :hoursError="hoursError"
              :hoursSuccess="hoursSuccess"
              @save-timer-hours="saveTimerHours"
              @add-manual-hours="addManualHours"
              @update-hourly-rate="updateHourlyRate"
              @load-hours-summary="loadHoursSummary"
            />
          </div>
        </div>
      </div>

      <BaseModal
        :visible="showModal"
        :model-values="taskTemplate"
        :model-display="taskTemplateDisplay"
        :onSave="saveTask"
        :model-name="'task'"
        @close="showModal = false"
      />
    </template>
  </AuthenticatedLayout>
</template>

<style scoped>
.content-tabs {
  background: white;
  border-radius: 12px;
  border: 1px solid #e1e5e9;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  width: 100%;
}

.tabs-header {
  display: flex;
  background: #f8fafc;
  border-bottom: 1px solid #e1e5e9;
  padding: 0 1.5rem;
}

.tab-button {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  background: none;
  border: none;
  padding: 1rem 1.5rem;
  font-size: 0.95rem;
  font-weight: 600;
  color: #64748b;
  cursor: pointer;
  transition: all 0.3s ease;
  border-bottom: 3px solid transparent;
  position: relative;
}

.tab-button:hover {
  color: #3b82f6;
  background: rgba(59, 130, 246, 0.05);
}

.tab-button.active {
  color: #3b82f6;
  border-bottom-color: #3b82f6;
  background: white;
}

.tab-badge {
  background: #3b82f6;
  color: white;
  border-radius: 10px;
  padding: 0.2rem 0.6rem;
  font-size: 0.75rem;
  font-weight: 600;
  margin-left: 0.25rem;
}

.tab-content {
  padding: 0;
}

.tab-pane {
  min-height: 500px;
}

.error-message {
  background: #fed7d7;
  color: #c53030;
  padding: 0.75rem;
  border-radius: 6px;
  margin-bottom: 1rem;
  text-align: center;
}

/* Responsive */
@media (max-width: 768px) {
  .tabs-header {
    flex-direction: column;
    padding: 0;
  }
  
  .tab-button {
    justify-content: center;
    border-bottom: 1px solid #e1e5e9;
    border-left: 3px solid transparent;
  }
  
  .tab-button.active {
    border-bottom: 1px solid #e1e5e9;
    border-left-color: #3b82f6;
  }
}
</style>