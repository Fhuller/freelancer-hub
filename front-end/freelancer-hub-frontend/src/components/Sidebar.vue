<script setup lang="ts">
import { RouterLink } from 'vue-router';

// O componente recebe uma prop booleana para saber se deve se mostrar
defineProps<{
  isOpen: boolean;
}>();

// E emite um evento quando o usuário clica no botão de fechar
const emit = defineEmits(['close']);
</script>

<template>
  <div v-if="isOpen" class="sidebar-backdrop" @click="emit('close')"></div>

  <aside class="sidebar" :class="{ 'sidebar--open': isOpen }">
    <div class="sidebar-header">
      <h3>Navegação</h3>
      <button class="close-btn" @click="emit('close')">
        &times; </button>
    </div>
    <nav class="sidebar-nav">
      <ul>
        <li>
          <RouterLink to="/" class="nav-link" active-class="active" @click="emit('close')">Dashboard</RouterLink>
        </li>
        <li>
          <RouterLink to="/clients" class="nav-link" active-class="active" @click="emit('close')">Clientes</RouterLink>
        </li>
        <li>
          <RouterLink to="/finance" class="nav-link" active-class="active" @click="emit('close')">Financeiro</RouterLink>
        </li>
      </ul>
    </nav>
  </aside>
</template>

<style scoped>
/* Estilos para o menu lateral */
.sidebar-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 99;
  transition: opacity 0.3s ease;
}

.sidebar {
  position: fixed;
  top: 0;
  left: 0;
  height: 100%;
  width: 250px;
  background-color: white;
  z-index: 100;
  box-shadow: 2px 0 5px rgba(0, 0, 0, 0.2);
  transform: translateX(-100%); /* Esconde o menu inicialmente */
  transition: transform 0.3s ease-in-out;
}

.sidebar--open {
  transform: translateX(0); /* Mostra o menu */
}

.sidebar-header {
  padding: 20px;
  border-bottom: 1px solid #eee;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.sidebar-header h3 {
  margin: 0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
}

.sidebar-nav {
  padding: 20px;
}

.sidebar-nav ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

.nav-link {
  display: block;
  padding: 10px 15px;
  color: #333;
  text-decoration: none;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.nav-link:hover, .nav-link.active {
  background-color: #007bff;
  color: white;
}
</style>