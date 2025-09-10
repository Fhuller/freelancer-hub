<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue';

const props = defineProps<{
  label: string;
  onMainClick?: () => void;
  onEdit?: () => void;
  onDelete?: () => void;
}>();

const showMenu = ref(false);
const cardRef = ref<HTMLElement | null>(null);

function handleMainClick() {
  if (props.onMainClick) props.onMainClick();
}

function toggleMenu(event: MouseEvent) {
  event.stopPropagation();
  showMenu.value = !showMenu.value;
}

function edit() {
  showMenu.value = false;
  if (props.onEdit) props.onEdit();
}

function remove() {
  showMenu.value = false;
  if (props.onDelete) props.onDelete();
}

function handleClickOutside(event: MouseEvent) {
  if (cardRef.value && !cardRef.value.contains(event.target as Node)) {
    showMenu.value = false;
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside);
});

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside);
});
</script>

<template>
  <div ref="cardRef" class="content-card" @click="handleMainClick">
    <!-- Botão de três pontinhos -->
    <button class="menu-button" @click="toggleMenu" aria-label="Menu">⋮</button>

    <!-- Menu dropdown -->
    <div v-if="showMenu" class="menu">
      <div class="menu-item" @click.stop="edit">
        <i class="fa-solid fa-pen"></i>
        <span>Editar</span>
      </div>
      <div class="menu-item" @click.stop="remove">
        <i class="fa-solid fa-trash"></i>
        <span>Excluir</span>
      </div>
    </div>

    <!-- Texto principal -->
    <div class="card-label">{{ label }}</div>
  </div>
</template>

<style scoped>
.content-card {
  position: relative;
  width: 200px;
  height: 220px;
  background-color: var(--color-white);
  border: 1px solid var(--color-gray);
  border-radius: 8px;
  cursor: pointer;

  display: flex;
  justify-content: flex-start;
  align-items: flex-end;
  padding: 12px;

  transition: box-shadow 0.2s ease, transform 0.1s ease;
}
.content-card:hover {
  box-shadow: var(--shadow-default);
  transform: scale(1.01);
}

.card-label {
  font-size: 16px;
  font-weight: 500;
  color: var(--color-dark-gray);
}

/* Botão três pontinhos */
.menu-button {
  position: absolute;
  top: 8px;
  right: 8px;
  background: transparent;
  border: none;
  font-size: 20px;
  color: var(--color-dark-gray);
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  line-height: 1;
  height: 30px;
  width: 30px;
}
.menu-button:hover {
  background-color: #f1f3f5;
}

/* Menu */
.menu {
  position: absolute;
  top: 32px;
  right: 8px;
  background-color: var(--color-white);
  border: 1px solid var(--color-gray);
  border-radius: 4px;
  box-shadow: var(--shadow-default);
  display: flex;
  flex-direction: column;
  z-index: 10;
  min-width: 120px;
}
.menu-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  cursor: pointer;
  font-size: 14px;
  color: var(--color-dark-gray);
}
.menu-item:hover {
  background-color: #f1f3f5;
}

.menu-item i {
  font-size: 14px;
}
</style>