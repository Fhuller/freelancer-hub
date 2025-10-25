<script setup lang="ts">
import { RouterLink } from 'vue-router';

defineProps<{
  isOpen: boolean;
}>();

const emit = defineEmits(['close']);
</script>

<template>
  <div v-if="isOpen" class="sidebar-backdrop" @click="emit('close')"></div>

  <aside class="sidebar" :class="{ 'sidebar--open': isOpen }">
    <div class="sidebar-header">
      <h3>Navegação</h3>
      <button class="close-btn" @click="emit('close')">&times;</button>
    </div>
    <nav class="sidebar-nav">
      <ul>
        <li>
          <RouterLink to="/app/dashboard" class="nav-link" active-class="active" @click="emit('close')">Dashboard</RouterLink>
        </li>
        <li>
          <RouterLink to="/app/clients" class="nav-link" active-class="active" @click="emit('close')">Clientes</RouterLink>
        </li>
        <li>
          <RouterLink to="/app/finance" class="nav-link" active-class="active" @click="emit('close')">Financeiro</RouterLink>
        </li>
      </ul>
    </nav>
  </aside>
</template>

<style scoped>
.sidebar-backdrop {
  position: fixed;
  inset: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(50, 50, 50, 0.4); 
  z-index: 1000;
  transition: opacity 0.3s ease;
}

.sidebar {
  position: fixed;
  top: 0;
  left: 0;
  height: 100%;
  width: 260px;
  background-color: #ffffff;
  z-index: 1000;
  box-shadow: var(--shadow-default);
  transform: translateX(-100%);
  transition: transform 0.3s ease-in-out;
  display: flex;
  flex-direction: column;
}

.sidebar--open {
  transform: translateX(0);
}

.sidebar-header {
  padding: 20px;
  border-bottom: 1px solid #e5e5e5;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.sidebar-header h3 {
  margin: 0;
  font-weight: 600;
  color: var(--color-purple);
}

.close-btn {
  background: none;
  border: none;
  font-size: 28px;
  line-height: 1;
  color: var(--color-purple);
  cursor: pointer;
  transition: color 0.2s;
}

.sidebar-nav {
  padding: 15px 0;
  flex: 1;
}

.sidebar-nav ul {
  list-style: none;
  margin: 0;
  padding: 0;
}

.nav-link {
  display: block;
  padding: 10px 20px;
  color: #4b4b4b;
  text-decoration: none;
  font-weight: 500;
  transition: background-color 0.2s, color 0.2s;
  border-left: 4px solid transparent;
}

.nav-link:hover {
  background-color: #f3f0fa;
  border-left-color: var(--color-purple);
  color: var(--color-purple);
}

.nav-link.active {
  background-color: #f3f0fa;
  color: var(--color-purple);
  border-left-color: var(--color-purple);
}
</style>