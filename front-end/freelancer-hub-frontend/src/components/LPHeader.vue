<template>
  <header class="fixed w-full top-0 z-50 bg-white/90 backdrop-blur-md border-b border-purple-500/20">
    <nav class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center h-16">
        <router-link to="/" class="text-2xl font-bold text-purple-700">
            <img alt="Freelancer Hub" src="@/assets/logo_grd.png" width="120" height="auto" />
        </router-link >
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
    </nav>
  </header>
</template>
