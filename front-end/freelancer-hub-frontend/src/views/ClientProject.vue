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
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue'
import BaseHeader from '../components/BaseHeader.vue'
import ContentCard from '../components/ContentCard.vue'
import AddCard from '../components/AddCard.vue'
import BaseModal from '../components/BaseModal.vue'

const route = useRoute()

const project = ref<ProjectReadDto | null>(null)
const tasks = ref<any[]>([])

const isLoading = ref(false)
const isLoadingTasks = ref(false)
const error = ref('')

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
  dueDate: formatDateForInput(getTomorrowDate()) // Data padrão é amanhã
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

function openNewTaskModal(status: string) {
  taskTemplate.value = {
    title: '',
    description: '',
    status: status,
    dueDate: formatDateForInput(getTomorrowDate()) // Data padrão é amanhã
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

@media (max-width: 640px) {
  .kanban-grid {
    grid-template-columns: 1fr;
  }
}
</style>