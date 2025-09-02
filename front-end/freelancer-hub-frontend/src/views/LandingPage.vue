<script>
import Layout from '@/layouts/LandingPageLayout.vue'

export default {
  name: 'LandingPage',
  components: { Layout },
  mounted() {
    document.title = 'FreelancerHub - Sistema de Gestão para Freelancers'
  }
}
</script>

<template>
  <Layout>
    <!-- Hero Section -->
    <section class="relative min-h-screen flex items-center justify-center px-4 sm:px-6 lg:px-8 bg-white/90">
      <div class="max-w-4xl mx-auto text-center">
        <!-- Logo -->
        <!-- <router-link to="/" class="inline-block mb-6">
          <img alt="Freelancer Hub" src="@/assets/logo_grd.png" width="240" height="auto" />
        </router-link> -->

        <!-- Headline -->
        <h1 class="text-5xl md:text-7xl font-bold text-purple-700 mb-6 leading-tight">
          FreelancerHub
        </h1>
        <p class="text-xl md:text-2xl text-purple-500 mb-8 leading-relaxed">
          Sistema completo para gerenciamento de clientes, projetos e finanças desenvolvido especialmente para freelancers
        </p>

        <!-- Feature Cards -->
        <div class="grid md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
          <div class="bg-white backdrop-blur-md rounded-lg p-6 border border-purple-300/20 hover:bg-white transition-all duration-300 transform hover:scale-105">
            <div class="text-purple-700 text-3xl mb-4">👥</div>
            <h3 class="text-purple-700 font-semibold mb-2">Gestão de Clientes</h3>
            <p class="text-purple-500 text-sm">Organize e mantenha histórico completo de seus clientes</p>
          </div>

          <div class="bg-white backdrop-blur-md rounded-lg p-6 border border-purple-300/20 hover:bg-white transition-all duration-300 transform hover:scale-105">
            <div class="text-purple-700 text-3xl mb-4">📋</div>
            <h3 class="text-purple-700 font-semibold mb-2">Projetos</h3>
            <p class="text-purple-500 text-sm">Acompanhe entregas, prazos e progresso dos projetos</p>
          </div>

          <div class="bg-white backdrop-blur-md rounded-lg p-6 border border-purple-300/20 hover:bg-white transition-all duration-300 transform hover:scale-105">
            <div class="text-purple-700 text-3xl mb-4">💰</div>
            <h3 class="text-purple-700 font-semibold mb-2">Finanças</h3>
            <p class="text-purple-500 text-sm">Controle faturas, pagamentos e despesas</p>
          </div>

          <div class="bg-white backdrop-blur-md rounded-lg p-6 border border-purple-300/20 hover:bg-white transition-all duration-300 transform hover:scale-105">
            <div class="text-purple-700 text-3xl mb-4">📊</div>
            <h3 class="text-purple-700 font-semibold mb-2">Relatórios</h3>
            <p class="text-purple-500 text-sm">Dashboard e análises para acompanhar performance</p>
          </div>
        </div>

        <!-- CTA Buttons -->
        <div class="space-y-4 sm:space-y-0 sm:space-x-4 sm:flex sm:justify-center">
          <router-link 
            to="/portfolio"
            class="w-full sm:w-auto bg-white hover:bg-purple-200 text-purple-700 font-bold py-4 px-8 rounded-lg text-lg transition-all duration-300 transform hover:scale-105 border border-purple-300 inline-block"
          >
            Saiba Mais
          </router-link>
        </div>
      </div>
    </section>

    <!-- Technology Section -->
    <section class="py-20 bg-white/90">
      <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-4xl font-bold text-purple-700 mb-4">Tecnologias Utilizadas</h2>
          <p class="text-xl text-purple-500">Desenvolvido com as melhores tecnologias do mercado</p>
        </div>

        <div class="grid md:grid-cols-3 gap-8">
          <div class="text-center p-8 bg-white rounded-lg border border-purple-300">
            <div class="text-6xl mb-4 text-purple-700">🖥️</div>
            <h3 class="text-2xl font-bold text-purple-700 mb-4">Frontend</h3>
            <p class="text-purple-500 text-lg">Vue.js</p>
          </div>

          <div class="text-center p-8 bg-white rounded-lg border border-purple-300">
            <div class="text-6xl mb-4 text-purple-700">⚙️</div>
            <h3 class="text-2xl font-bold text-purple-700 mb-4">Backend</h3>
            <p class="text-purple-500 text-lg">.NET Core</p>
          </div>

          <div class="text-center p-8 bg-white rounded-lg border border-purple-300">
            <div class="text-6xl mb-4 text-purple-700">🗄️</div>
            <h3 class="text-2xl font-bold text-purple-700 mb-4">Banco de Dados</h3>
            <p class="text-purple-500 text-lg">SQL Server</p>
          </div>
        </div>
      </div>
    </section>
  </Layout>
</template>