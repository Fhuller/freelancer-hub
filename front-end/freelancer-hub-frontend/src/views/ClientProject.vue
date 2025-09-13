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

// Modal
const showModal = ref(false)
const editingTask = ref<any | null>(null)
const taskTemplate = ref({
  title: '',
  description: '',
  status: 'Novo',
  dueDate: undefined
})

// Status do kanban
const statuses = ['Novo', 'Fazendo', 'Concluído', 'Cancelado']

// Carregar projeto
async function loadProject() {
  try {
    isLoading.value = true
    error.value = ''
    project.value = await fetchProjectById(route.params.id as string)
  } catch (err) {
    console.error(err)
    error.value = 'Erro ao carregar projeto'
  } finally {
    isLoading.value = false
  }
}

// Carregar tarefas
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

// Abrir modal para nova tarefa
function openNewTaskModal(status: string) {
  taskTemplate.value = {
    title: '',
    description: '',
    status: status,
    dueDate: undefined
  }
  editingTask.value = null
  showModal.value = true
}

// Editar tarefa existente
function editTask(task: any) {
  editingTask.value = task
  taskTemplate.value = {
    title: task.title,
    description: task.description || '',
    status: task.status || 'Novo',
    dueDate: task.dueDate || undefined
  }
  showModal.value = true
}

// Salvar tarefa
async function saveTask(data: Record<string, any>) {
  if (!project.value) return
  if (editingTask.value) {
    await updateTaskItem(editingTask.value.id, {
      title: data.title,
      description: data.description || '',
      status: data.status,
      dueDate: data.dueDate || undefined
    })
    editingTask.value = null
  } else {
    await createTaskItem({
      projectId: project.value.id,
      title: data.title,
      description: data.description || '',
      status: data.status,
      dueDate: data.dueDate || undefined
    })
  }
  await loadTasks()
  showModal.value = false
}

// Remover tarefa
async function removeTask(task: any) {
  try {
    await deleteTaskItem(task.id)
    await loadTasks()
  } catch (err) {
    console.error('Erro ao excluir tarefa:', err)
    error.value = 'Erro ao excluir tarefa'
  }
}

onMounted(async () => {
  await loadProject()
  await loadTasks()
})
</script>

<template>
  <AuthenticatedLayout :loading="isLoading || isLoadingTasks">
    <div v-if="error">{{ error }}</div>

    <template v-else>
      <!-- Cabeçalho do projeto -->
      <BaseHeader
        v-if="project"
        :model="project"
        model-name="project"
      />

      <!-- Kanban -->
      <div class="kanban">
        <div
          v-for="status in statuses"
          :key="status"
          class="kanban-column"
        >
          <h3>{{ status }}</h3>
          <AddCard :label="`Nova Tarefa (${status})`" :onClick="() => openNewTaskModal(status)" />

          <ContentCard
            v-for="task in tasks.filter(t => t.status === status)"
            :key="task.id"
            :label="task.title"
            :onMainClick="() => {}"
            :onEdit="() => editTask(task)"
            :onDelete="() => removeTask(task)"
          />
        </div>
      </div>

      <!-- Modal de tarefas -->
      <BaseModal
        :visible="showModal"
        :model="taskTemplate"
        :onSave="saveTask"
        :model-name="'task'"
        @close="showModal = false"
      />
    </template>
  </AuthenticatedLayout>
</template>

<style scoped>
.kanban {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.kanban-column {
  background: #f9f9f9;
  border-radius: 8px;
  padding: 1rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

.kanban-column h3 {
  margin-bottom: 0.75rem;
}
</style>
