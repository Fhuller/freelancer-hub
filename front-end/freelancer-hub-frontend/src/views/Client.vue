<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { fetchClientById, type ClientReadDto } from '../services/clients'
import { fetchProjects, createProject, updateProject, deleteProject } from '../services/projects'
import AuthenticatedLayout from '../layouts/AuthenticatedLayout.vue'
import ContentCard from '../components/ContentCard.vue'
import AddCard from '../components/AddCard.vue'
import BaseModal from '../components/BaseModal.vue'
import BaseHeader from '../components/BaseHeader.vue'

// Route e cliente
const route = useRoute()
const router = useRouter()
const client = ref<ClientReadDto | null>(null)

// Projetos
const projects = ref<any[]>([])
const editingProject = ref<any | null>(null)

// valores reais
const projectTemplate = ref({
  title: '',
  description: '',
  status: 'Pendente',
  dueDate: undefined
})

// mock/display
const projectTemplateDisplay = {
  title: 'Título do Projeto',
  description: 'Descrição breve',
  status: ['Pendente', 'Em Andamento', 'Concluído', 'Cancelado'], // select
  dueDate: new Date('2001-01-01')
}

// Controle de estado
const isLoading = ref(false)
const isLoadingProjects = ref(false)
const error = ref('')
const showModal = ref(false)

// Carregar cliente
async function loadClient() {
  try {
    isLoading.value = true
    error.value = ''
    client.value = await fetchClientById(route.params.id as string)
  } catch (err) {
    console.error(err)
    error.value = 'Erro ao carregar cliente'
  } finally {
    isLoading.value = false
  }
}

// Carregar projetos
async function loadProjects() {
  if (!client.value) return
  try {
    isLoadingProjects.value = true
    error.value = ''
    const allProjects = await fetchProjects()
    projects.value = allProjects.filter((p: any) => p.clientId === client.value?.id)
  } catch (err) {
    console.error(err)
    error.value = 'Erro ao carregar projetos'
  } finally {
    isLoadingProjects.value = false
  }
}

// Criar novo projeto
function openNewProjectModal() {
  projectTemplate.value = {
    title: '',
    description: '',
    status: 'Pendente',
    dueDate: undefined
  }
  editingProject.value = null
  showModal.value = true
}

// Editar projeto
function editProject(project: any) {
  editingProject.value = project
  projectTemplate.value = {
    title: project.title,
    description: project.description || '',
    status: project.status || 'Pendente',
    dueDate: project.dueDate || undefined
  }
  showModal.value = true
}

// Salvar projeto (novo ou edição)
async function saveProject(data: Record<string, any>) {
  if (!client.value) return
  if (editingProject.value) {
    await updateProject(editingProject.value.id, {
      clientId: client.value.id,
      title: data.title,
      description: data.description || '',
      status: data.status,
      dueDate: data.dueDate || undefined
    })
    editingProject.value = null
  } else {
    await createProject({
      userId: '1', // ajustar de acordo com seu contexto
      clientId: client.value.id,
      title: data.title,
      description: data.description || '',
      status: data.status,
      dueDate: data.dueDate || undefined
    })
  }
  await loadProjects()
  showModal.value = false
}

// Remover projeto
async function removeProject(project: any) {
  try {
    await deleteProject(project.id)
    await loadProjects()
  } catch (err) {
    console.error('Erro ao excluir projeto:', err)
    error.value = 'Erro ao excluir projeto'
  }
}

function openProjectDetails(project: any) {
  if (!client.value) return
  router.push({ 
    name: 'ClientProject', 
    params: { 
      id: client.value.id,       // id do cliente
      projectId: project.id      // id do projeto
    } 
  })
}

// Lifecycle
onMounted(async () => {
  await loadClient()
  await loadProjects()
})
</script>

<template>
  <AuthenticatedLayout :loading="isLoading || isLoadingProjects">
    <div v-if="error">{{ error }}</div>

    <template v-else>
      <BaseHeader
        v-if="client"
        :model="client"
        model-name="client"
      />

      <!-- Ações -->
      <AddCard label="Novo Projeto" :onClick="openNewProjectModal" />

      <!-- Projetos -->
      <ContentCard
        v-for="project in projects"
        :key="project.id"
        :label="project.title"
        :onMainClick="() => openProjectDetails(project)"
        :onEdit="() => editProject(project)"
        :onDelete="() => removeProject(project)"
      />

      <!-- Modal de projeto -->
      <BaseModal
        :visible="showModal"
        :model-values="projectTemplate"
        :model-display="projectTemplateDisplay"
        :onSave="saveProject"
        :model-name="'project'"
        @close="showModal = false"
      />
    </template>
  </AuthenticatedLayout>
</template>

<style scoped>
.client-header {
  grid-column: 1 / -1; /* ocupa todas as colunas do grid do layout */
  background: #f9f9f9;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  width: 100%;
}

.client-header h2 {
  margin: 0 0 0.75rem;
}

.client-info p {
  margin: 0.25rem 0;
}
</style>
