<script setup lang="ts">
import { ref, onMounted } from 'vue'
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

      <!-- SEÇÃO DE ARQUIVOS - NOVO ESTILO -->
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

      <!-- KANBAN -->
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
/* NOVOS ESTILOS PARA A SEÇÃO DE ARQUIVOS */
.files-section {
  background: white;
  border-radius: 12px;
  border: 1px solid #e1e5e9;
  padding: 1.5rem;
  margin-bottom: 2rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  width: 100%;
  grid-column: 1 / -1;
}

.files-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #f1f5f9;
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

/* Estilos existentes do Kanban (mantidos) */
.kanban-board {
  grid-column: 1 / -1;
  width: 100%;
  margin-top: 1rem;
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
}

@media (max-width: 768px) {
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
}
</style>