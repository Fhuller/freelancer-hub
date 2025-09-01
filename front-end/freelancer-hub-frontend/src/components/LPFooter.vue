<template>
  <footer class="bg-white/90 py-12 border-t border-purple-500/20">
    <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="grid md:grid-cols-3 gap-8 text-center md:text-left">
        <!-- Logo e descrição -->
        <div>
          <h3 class="text-2xl font-bold text-purple-700 mb-4">FreelancerHub</h3>
          <p class="text-purple-500">Sistema de gestão completo para freelancers</p>
        </div>

        <!-- Links -->
        <div>
          <h4 class="text-lg font-semibold text-purple-700 mb-4">Links</h4>
          <div class="space-y-2">
            <router-link to="/about" class="text-purple-500 hover:text-purple-700 block transition">Sobre</router-link>
            <router-link to="/portfolio" class="text-purple-500 hover:text-purple-700 block transition">Portfólio</router-link>
            <a href="https://www.overleaf.com" target="_blank" class="text-purple-500 hover:text-purple-700 block transition">RFC</a>
          </div>
        </div>

        <!-- Projeto -->
        <div>
          <h4 class="text-lg font-semibold text-purple-700 mb-4">Projeto</h4>
          <p class="text-purple-500 text-sm">TCC - Engenharia de Software</p>
          <p class="text-purple-500 text-sm">Católica de Santa Catarina</p>
        </div>
      </div>

      <!-- Rodapé inferior -->
      <div class="mt-8 pt-8 border-t border-purple-500/20 text-center">
        <p class="text-purple-500">&copy; 2025 FreelancerHub. Projeto acadêmico.</p>
      </div>
    </div>
  </footer>
</template>