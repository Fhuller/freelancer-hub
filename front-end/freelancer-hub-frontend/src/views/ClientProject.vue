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

const taskTemplate = ref({
  title: '',
  description: '',
  status: 'Pendente',
  dueDate: new Date()
})

const taskTemplateDisplay = {
  title: 'Título da Tarefa',
  description: 'Descrição da tarefa',
  status: ['Pendente', 'Em Andamento', 'Concluída', 'Cancelada'],
  dueDate: new Date('2001-01-01')
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
    dueDate: new Date()
  }
  editingTask.value = null
  showModal.value = true
}

function editTask(task: any) {
  editingTask.value = task
  taskTemplate.value = {
    title: task.title,
    description: task.description || '',
    status: task.status || 'Pendente',
    dueDate: task.dueDate || undefined
  }
  showModal.value = true
}

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
      <BaseHeader
        v-if="project"
        :model="project"
        model-name="project"
      />

      <div class="kanban">
        <div
          v-for="status in statuses"
          :key="status.name"
          class="kanban-column"
          :class="status.color"
        >
          <h3>{{ status.name }}</h3>
          <AddCard
            :label="`Nova Tarefa (${status.name})`"
            :onClick="() => openNewTaskModal(status.name)"
          />

          <ContentCard
            v-for="task in tasks.filter(t => t.status === status.name)"
            :key="task.id"
            :label="task.title"
            :onMainClick="() => {}"
            :onEdit="() => editTask(task)"
            :onDelete="() => removeTask(task)"
          />
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
.kanban {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 1rem;
}

.kanban-column {
  border-radius: 8px;
  padding: 1rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  gap: 10px;
  display: flex;
  flex-direction: column;
  color: white;
}

.kanban-column h3 {
  margin-bottom: 0.75rem;
  font-weight: bold;
}

.kanban-column.pending {
  color: #facc15; /* amarelo */
  border: 2px solid #facc15; /* amarelo */
  background-color: #facc157a;
}

.kanban-column.in-progress {
  color: #3b82f6; /* azul */
  border: 2px solid #3b82f6; /* azul */
  background-color: #3b83f67a;
}

.kanban-column.done {
  color: #065f46; /* verde escuro */
  border: 2px solid #065f46; /* verde escuro */
  background-color: #065f467a; /* verde escuro */
}

.kanban-column.canceled {
  color: #dc2626; /* vermelho */
  border: 2px solid #dc2626; /* vermelho */
  background-color: #dc26267a;
}

.content-card {
  background: white;
  color: #111;
  border-radius: 6px;
  padding: 0.5rem;
  box-shadow: 0 1px 2px rgba(0,0,0,0.1);
  height: 50px;
}

.add-card {
  background: rgba(255, 255, 255, 0.8);
  border: 2px dashed rgba(0, 0, 0, 0.2);
  color: #111;
  height: 85px;
}
</style>