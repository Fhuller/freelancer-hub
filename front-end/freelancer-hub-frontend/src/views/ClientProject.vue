<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { fetchProjectById, type ProjectReadDto } from '../services/projects'
import {
  fetchTaskItemsByProject,
  createTaskItem,
  updateTaskItem,
  deleteTaskItem
} from '../services/taskItems'
// Importar os serviços de arquivos
import { 
  uploadProjectFile, 
  getProjectFiles, 
  deleteProjectFile, 
  type FileDto 
} from '../services/projects'
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue'
import BaseHeader from '../components/BaseHeader.vue'
import ContentCard from '../components/ContentCard.vue'
import AddCard from '../components/AddCard.vue'
import BaseModal from '../components/BaseModal.vue'

const route = useRoute()

const project = ref<ProjectReadDto | null>(null)
const tasks = ref<any[]>([])
const files = ref<FileDto[]>([])

const isLoading = ref(false)
const isLoadingTasks = ref(false)
const isLoadingFiles = ref(false)
const error = ref('')
const fileError = ref('')

const showModal = ref(false)
const editingTask = ref<any | null>(null)

// Variáveis para abas
const activeTab = ref('kanban')

// Variáveis para controle de tempo
const isTimerRunning = ref(false)
const totalSeconds = ref(0)
const timerInterval = ref<NodeJS.Timeout | null>(null)
const editingTime = ref(false)
const tempHours = ref('0')

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

const statuses = [
  { name: 'Pendente', color: 'pending' },
  { name: 'Em Andamento', color: 'in-progress' },
  { name: 'Concluída', color: 'done' },
  { name: 'Cancelada', color: 'canceled' }
]

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

// Funções do timer
function toggleTimer() {
  if (isTimerRunning.value) {
    // Pausar
    if (timerInterval.value) {
      clearInterval(timerInterval.value)
      timerInterval.value = null
    }
  } else {
    // Iniciar
    timerInterval.value = setInterval(() => {
      totalSeconds.value += 1
    }, 1000)
  }
  isTimerRunning.value = !isTimerRunning.value
}

function resetTimer() {
  if (timerInterval.value) {
    clearInterval(timerInterval.value)
    timerInterval.value = null
  }
  isTimerRunning.value = false
  totalSeconds.value = 0
}

function editTime() {
  editingTime.value = true
  tempHours.value = totalHours.value
}

function saveTime() {
  const hours = parseFloat(tempHours.value)
  if (!isNaN(hours) && hours >= 0) {
    totalSeconds.value = Math.round(hours * 3600)
  }
  editingTime.value = false
}

function cancelEditTime() {
  editingTime.value = false
  tempHours.value = totalHours.value
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

// NOVAS FUNÇÕES PARA ARQUIVOS
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

// Helper para formatar tamanho do arquivo
function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 Bytes'
  
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// Helper para obter ícone baseado na extensão do arquivo
function getFileIcon(extension: string): string {
  const iconMap: { [key: string]: string } = {
    '.pdf': 'fa-file-pdf',
    '.doc': 'fa-file-word',
    '.docx': 'fa-file-word',
    '.xls': 'fa-file-excel',
    '.xlsx': 'fa-file-excel',
    '.ppt': 'fa-file-powerpoint',
    '.pptx': 'fa-file-powerpoint',
    '.jpg': 'fa-file-image',
    '.jpeg': 'fa-file-image',
    '.png': 'fa-file-image',
    '.gif': 'fa-file-image',
    '.zip': 'fa-file-archive',
    '.rar': 'fa-file-archive',
    '.txt': 'fa-file-alt',
    '.js': 'fa-file-code',
    '.ts': 'fa-file-code',
    '.html': 'fa-file-code',
    '.css': 'fa-file-code',
    '.json': 'fa-file-code',
  }
  
  return iconMap[extension.toLowerCase()] || 'fa-file'
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
  await loadFiles() // Carregar arquivos ao inicializar
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
            <div class="kanban-board">
              <div class="kanban-grid">
                <div
                  v-for="status in statuses"
                  :key="status.name"
                  class="kanban-column"
                  :class="{ 'drag-over': dragOverColumn === status.name }"
                  @dragover="(e) => onDragOver(e, status.name)"
                  @dragleave="onDragLeave"
                  @drop="() => onDrop(status.name)"
                >
                  <div class="column-header">
                    <span class="status-dot" :class="status.color"></span>
                    <h3 class="column-title">{{ status.name }}</h3>
                    <span class="task-count">{{ tasks.filter(t => t.status === status.name).length }}</span>
                  </div>
                  
                  <div class="column-content">
                    <AddCard
                      :label="`Nova Tarefa`"
                      :onClick="() => openNewTaskModal(status.name)"
                      class="add-card"
                    />

                    <div class="tasks-list">
                      <ContentCard
                        v-for="task in tasks.filter(t => t.status === status.name)"
                        :key="task.id"
                        :label="task.title"
                        :onMainClick="() => {}"
                        :onEdit="() => editTask(task)"
                        :onDelete="() => removeTask(task)"
                        class="task-card"
                        draggable="true"
                        @dragstart="() => onDragStart(task)"
                        @dragend="draggedTask = null"
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- ABA ARQUIVOS -->
          <div v-if="activeTab === 'files'" class="tab-pane">
            <div class="files-section">
              <div class="files-header">
                <h3 class="section-title">
                  <i class="fas fa-folder-open"></i>
                  Arquivos do Projeto
                  <span class="files-count" v-if="files.length > 0">({{ files.length }})</span>
                </h3>
                <div class="file-actions">
                  <input
                    type="file"
                    id="file-upload"
                    class="file-input"
                    @change="handleFileUpload"
                    :disabled="isLoadingFiles"
                  />
                  <label for="file-upload" class="upload-button">
                    <i class="fas fa-plus"></i>
                    <span v-if="isLoadingFiles">Enviando...</span>
                    <span v-else>Adicionar Arquivo</span>
                  </label>
                </div>
              </div>

              <div v-if="fileError" class="error-message file-error">
                <i class="fas fa-exclamation-triangle"></i>
                {{ fileError }}
              </div>

              <div v-if="isLoadingFiles" class="loading-files">
                <i class="fas fa-spinner fa-spin"></i>
                Carregando arquivos...
              </div>

              <div v-else-if="files.length === 0" class="no-files">
                <i class="fas fa-folder-open"></i>
                <p>Nenhum arquivo adicionado ao projeto</p>
              </div>

              <div v-else class="files-container">
                <div class="files-list">
                  <div
                    v-for="file in files"
                    :key="file.id"
                    class="file-card"
                  >
                    <div class="file-icon">
                      <i :class="['fas', getFileIcon(file.fileExtension)]"></i>
                    </div>
                    <div class="file-content">
                      <div class="file-info">
                        <h4 class="file-name">{{ file.fileName }}{{ file.fileExtension }}</h4>
                        <p class="file-meta">
                          <span class="file-size">
                            <i class="fas fa-hdd"></i>
                            {{ formatFileSize(file.fileSize) }}
                          </span>
                          <span class="file-date">
                            <i class="fas fa-calendar"></i>
                            {{ new Date(file.createdAt).toLocaleDateString('pt-BR') }}
                          </span>
                        </p>
                      </div>
                      <div class="file-actions">
                        <button
                          class="action-button download-button"
                          @click="handleDownloadFile(file)"
                          title="Download"
                        >
                          <i class="fas fa-download"></i>
                        </button>
                        <button
                          class="action-button delete-button"
                          @click="handleDeleteFile(file.id)"
                          title="Excluir"
                        >
                          <i class="fas fa-trash"></i>
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- ABA RASTREAMENTO DE HORAS -->
          <div v-if="activeTab === 'time'" class="tab-pane">
            <div class="time-tracking-section">
              <div class="time-header">
                <h3 class="section-title">
                  <i class="fas fa-clock"></i>
                  Rastreamento de Horas de Trabalho
                </h3>
                <p class="time-description">
                  Acompanhe o tempo gasto neste projeto
                </p>
              </div>

              <div class="time-display">
                <div class="timer-container">
                  <div class="timer">
                    <div class="timer-time">{{ formattedTime }}</div>
                    <div class="timer-hours" v-if="!editingTime">
                      <span>{{ totalHours }} horas</span>
                      <button class="edit-time-button" @click="editTime" title="Editar horas">
                        <i class="fas fa-edit"></i>
                      </button>
                    </div>
                    <div class="timer-edit" v-else>
                      <input 
                        v-model="tempHours" 
                        type="number" 
                        step="0.01" 
                        min="0"
                        class="time-input"
                        @keyup.enter="saveTime"
                        @keyup.escape="cancelEditTime"
                      />
                      <span>horas</span>
                      <div class="edit-actions">
                        <button class="save-time-button" @click="saveTime" title="Salvar">
                          <i class="fas fa-check"></i>
                        </button>
                        <button class="cancel-time-button" @click="cancelEditTime" title="Cancelar">
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
                    >
                      <i :class="isTimerRunning ? 'fas fa-pause' : 'fas fa-play'"></i>
                      {{ isTimerRunning ? 'Pausar' : 'Iniciar' }}
                    </button>
                    <button 
                      class="timer-button reset-button" 
                      @click="resetTimer"
                      :disabled="totalSeconds === 0"
                    >
                      <i class="fas fa-redo"></i>
                      Reiniciar
                    </button>
                  </div>
                </div>
              </div>
            </div>
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
.add-card{
  height: 100px;
}

.content-card{
  height: 100px;
}

/* ESTILOS DAS ABAS */
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

/* ESTILOS DA ABA DE TEMPO */
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

.time-display {
  display: grid;
  gap: 2rem;
  margin-bottom: 2rem;
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

.time-stats {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.stat-card {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 1.5rem;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  transition: all 0.3s ease;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.stat-value {
  font-size: 2rem;
  font-weight: 700;
  color: #1e293b;
  margin-bottom: 0.5rem;
}

.stat-label {
  color: #64748b;
  font-size: 0.9rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.time-notes {
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 1.5rem;
}

.time-notes h4 {
  color: #1e293b;
  margin: 0 0 1rem 0;
  font-size: 1.125rem;
}

.time-notes p {
  color: #64748b;
  margin: 0 0 1rem 0;
}

.time-notes ul {
  color: #64748b;
  margin: 0;
  padding-left: 1.25rem;
}

.time-notes li {
  margin-bottom: 0.25rem;
}

/* ESTILOS ORIGINAIS PARA ARQUIVOS RESTAURADOS */
.files-section {
  background: white;
  border-radius: 12px;
  border: 1px solid #e1e5e9;
  padding: 1.5rem;
  margin: 0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  width: 100%;
}

.files-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #f1f5f9;
  padding: 0 0 1rem 0;
}

.section-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1e293b;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.section-title i {
  color: #3b82f6;
  font-size: 1.375rem;
}

.files-count {
  background: #3b82f6;
  color: white;
  border-radius: 20px;
  padding: 0.25rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 600;
  margin-left: 0.5rem;
}

.file-actions {
  display: flex;
  gap: 0.75rem;
}

.file-input {
  display: none;
}

.upload-button {
  background: linear-gradient(135deg, #3b82f6, #1d4ed8);
  color: white;
  border: none;
  border-radius: 8px;
  padding: 0.75rem 1.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  box-shadow: 0 2px 4px rgba(59, 130, 246, 0.3);
}

.upload-button:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(59, 130, 246, 0.4);
  background: linear-gradient(135deg, #1d4ed8, #1e40af);
}

.upload-button:disabled {
  background: #94a3b8;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.file-error {
  margin-bottom: 1rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.file-error i {
  color: #dc2626;
}

.loading-files {
  text-align: center;
  padding: 3rem;
  color: #64748b;
  font-size: 1.125rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
}

.loading-files i {
  color: #3b82f6;
}

.no-files {
  text-align: center;
  padding: 3rem;
  color: #94a3b8;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
}

.no-files i {
  font-size: 3rem;
  color: #cbd5e1;
}

.no-files p {
  font-size: 1.125rem;
  margin: 0;
}

.files-container {
  width: 100%;
}

.files-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.file-card {
  display: flex;
  align-items: center;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 1.25rem;
  transition: all 0.3s ease;
  gap: 1rem;
}

.file-card:hover {
  border-color: #3b82f6;
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.15);
  transform: translateY(-2px);
  background: white;
}

.file-icon {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #3b82f6, #60a5fa);
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 1.5rem;
  flex-shrink: 0;
}

.file-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex: 1;
  gap: 1rem;
}

.file-info {
  flex: 1;
  min-width: 0;
}

.file-name {
  font-size: 1.125rem;
  font-weight: 600;
  color: #1e293b;
  margin: 0 0 0.5rem 0;
  word-break: break-word;
}

.file-meta {
  display: flex;
  gap: 1.5rem;
  margin: 0;
  font-size: 0.875rem;
  color: #64748b;
}

.file-size,
.file-date {
  display: flex;
  align-items: center;
  gap: 0.375rem;
}

.file-size i,
.file-date i {
  color: #94a3b8;
  font-size: 0.75rem;
}

.file-actions {
  display: flex;
  gap: 0.5rem;
  flex-shrink: 0;
}

.action-button {
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 0.75rem;
  cursor: pointer;
  font-size: 1rem;
  transition: all 0.2s ease;
  color: #64748b;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 44px;
  height: 44px;
}

.action-button:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.download-button:hover {
  background: #dcfce7;
  border-color: #22c55e;
  color: #16a34a;
}

.delete-button:hover {
  background: #fef2f2;
  border-color: #ef4444;
  color: #dc2626;
}

/* ESTILOS EXISTENTES DO KANBAN (AJUSTADOS) */
.kanban-board {
  width: 100%;
  padding: 1.5rem;
}

.kanban-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1.5rem;
  width: 100%;
  min-height: 600px;
}

.kanban-column {
  display: flex;
  flex-direction: column;
  background: #f8f9fa;
  border-radius: 8px;
  border: 1px solid #e1e5e9;
  min-height: 500px;
  transition: all 0.2s ease;
}

.kanban-column.drag-over {
  background: #e3f2fd;
  border: 2px dashed #2196f3;
  transform: scale(1.02);
}

.column-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 1rem;
  background: white;
  border-bottom: 1px solid #e1e5e9;
  border-radius: 8px 8px 0 0;
}

.column-title {
  font-size: 0.9rem;
  font-weight: 600;
  color: #2d3748;
  margin: 0;
  flex: 1;
}

.task-count {
  background: #e2e8f0;
  color: #4a5568;
  border-radius: 10px;
  padding: 0.2rem 0.5rem;
  font-size: 0.75rem;
  font-weight: 600;
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  display: inline-block;
  flex-shrink: 0;
}

.status-dot.pending {
  background-color: #ed8936;
}

.status-dot.in-progress {
  background-color: #3182ce;
}

.status-dot.done {
  background-color: #38a169;
}

.status-dot.canceled {
  background-color: #e53e3e;
}

.column-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  padding: 1rem;
  gap: 0.75rem;
  overflow-y: auto;
}

.tasks-list {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

/* Ajustes para os cards ocuparem toda a largura */
:deep(.task-card) {
  width: 100%;
  box-sizing: border-box;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  padding: 0.75rem;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
  transition: all 0.2s ease;
  cursor: grab;
  margin: 0;
}

:deep(.task-card:hover) {
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  border-color: #cbd5e0;
}

:deep(.task-card:active) {
  cursor: grabbing;
  transform: rotate(2deg);
  opacity: 0.8;
}

:deep(.task-card.dragging) {
  opacity: 0.5;
}

:deep(.add-card) {
  width: 100%;
  box-sizing: border-box;
  background: rgba(255, 255, 255, 0.9);
  border: 2px dashed #cbd5e0;
  border-radius: 6px;
  color: #718096;
  min-height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  cursor: pointer;
  font-size: 0.875rem;
  margin: 0;
}

:deep(.add-card:hover) {
  background: rgba(247, 250, 252, 0.9);
  border-color: #3182ce;
  color: #3182ce;
}

.error-message {
  background: #fed7d7;
  color: #c53030;
  padding: 0.75rem;
  border-radius: 6px;
  margin-bottom: 1rem;
  text-align: center;
}

/* Garantir que o conteúdo interno dos cards também ocupe toda a largura */
:deep(.task-card .card-content) {
  width: 100%;
}

:deep(.add-card .card-content) {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Responsive */
@media (max-width: 1024px) {
  .kanban-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .time-display {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
}

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
  
  .files-header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }
  
  .file-actions {
    width: 100%;
  }
  
  .upload-button {
    width: 100%;
    justify-content: center;
  }

  .file-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }

  .file-actions {
    align-self: flex-end;
  }

  .file-meta {
    flex-direction: column;
    gap: 0.5rem;
  }
  
  .timer-controls {
    flex-direction: column;
  }
  
  .time-tracking-section {
    padding: 1rem;
  }
}

@media (max-width: 640px) {
  .kanban-grid {
    grid-template-columns: 1fr;
  }
  
  .file-card {
    flex-direction: column;
    text-align: center;
    gap: 1rem;
  }

  .file-content {
    width: 100%;
    align-items: center;
  }

  .file-actions {
    align-self: center;
  }
  
  .timer-time {
    font-size: 2rem;
  }
}
</style>