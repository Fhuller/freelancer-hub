<script setup lang="ts">
import { ref } from 'vue'

const isOpen = ref(false)

const toggleMenu = () => {
  isOpen.value = !isOpen.value
}
</script>

<template>
  <header class="fixed w-full top-0 z-50 bg-white backdrop-blur-md border-b border-purple-500/20">
    <nav class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center h-16">
        <!-- Logo -->
        <router-link to="/" class="text-2xl font-bold text-purple-700">
          <img alt="Freelancer Hub" src="@/assets/logo_grd.png" width="120" />
        </router-link>

        <!-- Botão mobile -->
        <button
          @click="toggleMenu"
          class="md:hidden inline-flex items-center justify-center p-2 rounded-md text-purple-700 hover:text-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-500"
          aria-label="Abrir menu"
        >
          <svg
            class="h-6 w-6"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              v-if="!isOpen"
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M4 6h16M4 12h16M4 18h16"
            />
            <path
              v-else
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>

        <!-- Menu desktop -->
        <div class="hidden md:flex items-center space-x-4">
          <router-link
            to="/"
            class="text-purple-700 hover:text-purple-500 transition font-medium"
          >
            Início
          </router-link>
          <router-link
            to="/about"
            class="text-purple-700 hover:text-purple-500 transition font-medium"
          >
            Sobre
          </router-link>
          <router-link
            to="/portfolio"
            class="text-purple-700 hover:text-purple-500 transition font-medium"
          >
            Portfólio
          </router-link>
          <router-link
            to="/login"
            class="bg-purple-700 hover:bg-purple-800 text-white px-4 py-2 rounded-lg transition"
          >
            Login
          </router-link>
        </div>
      </div>

      <!-- Menu mobile -->
      <div
        v-show="isOpen"
        class="md:hidden flex flex-col space-y-2 mt-2 pb-3 border-t border-purple-100"
      >
        <router-link
          to="/"
          class="block text-purple-700 hover:text-purple-500 font-medium px-2 py-2"
          @click="isOpen = false"
        >
          Início
        </router-link>
        <router-link
          to="/about"
          class="block text-purple-700 hover:text-purple-500 font-medium px-2 py-2"
          @click="isOpen = false"
        >
          Sobre
        </router-link>
        <router-link
          to="/portfolio"
          class="block text-purple-700 hover:text-purple-500 font-medium px-2 py-2"
          @click="isOpen = false"
        >
          Portfólio
        </router-link>
        <router-link
          to="/login"
          class="block bg-purple-700 hover:bg-purple-800 text-white px-4 py-2 rounded-lg mx-2 transition"
          @click="isOpen = false"
        >
          Login
        </router-link>
      </div>
    </nav>
  </header>
</template>
