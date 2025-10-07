<script setup lang="ts">
import { ref } from 'vue'
import AddCard from '../components/AddCard.vue'
import ContentCard from '../components/ContentCard.vue'

defineProps<{
  tasks: any[]
}>()

const statuses = [
  { name: 'Pendente', color: 'pending' },
  { name: 'Em Andamento', color: 'in-progress' },
  { name: 'Concluída', color: 'done' },
  { name: 'Cancelada', color: 'canceled' }
]

const dragOverColumn = ref<string | null>(null)

defineEmits<{
  'drag-start': [task: any]
  'drag-over': [event: DragEvent, status: string]
  'drag-end': []
  'drag-leave': []
  'drop': [status: string]
  'open-new-task-modal': [status: string]
  'edit-task': [task: any]
  'remove-task': [task: any]
}>()
</script>

<template>
  <div class="kanban-board">
    <div class="kanban-grid">
      <div
        v-for="status in statuses"
        :key="status.name"
        class="kanban-column"
        :class="{ 'drag-over': dragOverColumn === status.name }"
        @dragover="(e) => $emit('drag-over', e, status.name)"
        @dragleave="$emit('drag-leave')"
        @drop="() => $emit('drop', status.name)"
      >
        <div class="column-header">
          <span class="status-dot" :class="status.color"></span>
          <h3 class="column-title">{{ status.name }}</h3>
          <span class="task-count">{{ tasks.filter(t => t.status === status.name).length }}</span>
        </div>
        
        <div class="column-content">
          <AddCard
            :label="`Nova Tarefa`"
            :onClick="() => $emit('open-new-task-modal', status.name)"
            class="add-card"
          />

          <div class="tasks-list">
            <ContentCard
              v-for="task in tasks.filter(t => t.status === status.name)"
              :key="task.id"
              :label="task.title"
              :onMainClick="() => {}"
              :onEdit="() => $emit('edit-task', task)"
              :onDelete="() => $emit('remove-task', task)"
              class="task-card"
              draggable="true"
              @dragstart="() => $emit('drag-start', task)"
              @dragend="$emit('drag-end')"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>


<style scoped>
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

.add-card{
  height: 100px;
}

.task-card{
  height: 100px;
}

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

@media (max-width: 1024px) {
  .kanban-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 768px) {
  .kanban-grid {
    grid-template-columns: 1fr;
  }
}
</style>