<script>
import Layout from '@/layouts/LandingPageLayout.vue'

export default {
  name: 'Portfolio',
  components: { Layout },
  mounted() {
    document.title = 'Portfólio TCC - FreelancerHub'
  }
}
</script>

<template>
  <Layout>
    <!-- Hero Section -->
    <section class="py-20 text-center bg-white/90">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <h1 class="text-5xl md:text-6xl font-bold text-purple-700 mb-6">
          Portfólio TCC
        </h1>
        <p class="text-xl text-purple-500 mb-8">
          Projeto Final - Engenharia de Software | Católica de Santa Catarina
        </p>
        <div class="inline-block rounded-lg border-2 border-purple-700 bg-transparent px-6 py-3">
          <span class="text-purple-700 font-semibold">
            FreelancerHub: Sistema de Controle para Freelancers
          </span>
        </div>
      </div>
    </section>

    <!-- Project Overview -->
    <section class="py-16 bg-white/90">
      <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid lg:grid-cols-2 gap-12 items-center mb-16">
          <div>
            <h2 class="text-4xl font-bold text-purple-700 mb-6">Visão Geral do Projeto</h2>
            <p class="text-purple-500 leading-relaxed mb-6">
              O FreelanceHub é uma solução integrada desenvolvida como Trabalho de Conclusão de Curso (TCC) 
              para o curso de <strong class="text-purple-700">Engenharia de Software</strong> na 
              <strong class="text-purple-700">Católica de Santa Catarina</strong>, também chamado de Portfólio.
            </p>
            <p class="text-purple-500 leading-relaxed mb-8">
              O sistema oferece ferramentas completas para freelancers manterem controle total sobre seus 
              negócios, incluindo gerenciamento de clientes, projetos, finanças e relatórios analíticos.
            </p>
            
            <div class="flex flex-wrap gap-4">
              <a 
                href="https://freelancer-hub-tau.vercel.app" 
                target="_blank"
                class="bg-gradient-to-r from-purple-600 to-indigo-600 hover:from-purple-700 hover:to-indigo-700 text-white font-bold py-3 px-6 rounded-lg transition-all duration-300 transform hover:scale-105 shadow-lg"
              >
                🚀 Sistema em Produção
              </a>
              <a 
                href="https://www.overleaf.com/read/xthqfscqnfrw#653fbd" 
                target="_blank"
                class="bg-white hover:bg-purple-50 text-purple-700 font-bold py-3 px-6 rounded-lg transition-all duration-300 transform hover:scale-105 border border-purple-300"
              >
                📄 RFC Completa
              </a>
              <a 
                href="https://github.com/Fhuller/freelancer-hub" 
                target="_blank"
                class="bg-white hover:bg-purple-50 text-purple-700 font-bold py-3 px-6 rounded-lg transition-all duration-300 transform hover:scale-105 border border-purple-300"
              >
                💻 Código Fonte (GitHub)
              </a>
              <a 
                href="https://freelancer-hub-backend-exe7f6cygggae0db.eastus2-01.azurewebsites.net/swagger/index.html" 
                target="_blank"
                class="bg-white hover:bg-purple-50 text-purple-700 font-bold py-3 px-6 rounded-lg transition-all duration-300 transform hover:scale-105 border border-purple-300"
              >
                🔗 API - Swagger Docs
              </a>
              <a 
                href="https://sonarcloud.io/summary/overall?id=Fhuller_freelancer-hub&branch=main" 
                target="_blank"
                class="bg-white hover:bg-purple-50 text-purple-700 font-bold py-3 px-6 rounded-lg transition-all duration-300 transform hover:scale-105 border border-purple-300"
              >
                🔗 SonarCloud
              </a>
            </div>
          </div>

          <div class="bg-white rounded-lg p-8 border border-purple-300 shadow">
            <h3 class="text-2xl font-bold text-purple-700 mb-6 text-center">Informações Acadêmicas</h3>
            <div class="space-y-4">
              <div class="flex justify-between items-center py-2 border-b border-purple-200">
                <span class="text-purple-500">Instituição:</span>
                <span class="text-purple-700 font-semibold">Católica SC</span>
              </div>
              <div class="flex justify-between items-center py-2 border-b border-purple-200">
                <span class="text-purple-500">Curso:</span>
                <span class="text-purple-700 font-semibold">Eng. de Software</span>
              </div>
              <div class="flex justify-between items-center py-2 border-b border-purple-200">
                <span class="text-purple-500">Tipo:</span>
                <span class="text-purple-700 font-semibold">TCC</span>
              </div>
              <div class="flex justify-between items-center py-2 border-b border-purple-200">
                <span class="text-purple-500">Status:</span>
                <span class="text-yellow-500 font-semibold">Em desenvolvimento</span>
              </div>
              <div class="flex justify-between items-center py-2">
                <span class="text-purple-500">Deploy:</span>
                <span class="text-green-500 font-semibold">✓ Em Produção</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Modules Section -->
    <section class="py-20 bg-white/90">
      <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <h2 class="text-4xl font-bold text-purple-700 text-center mb-16">Módulos do Sistema</h2>
        <div class="grid md:grid-cols-2 gap-8 mb-12">
          <!-- Módulos -->
          <div class="bg-white rounded-lg p-8 border border-purple-300 shadow transform hover:scale-105 transition-all duration-300">
            <div class="flex items-center mb-6">
              <div class="text-4xl mr-4 text-purple-700">👥</div>
              <h3 class="text-2xl font-bold text-purple-700">Gestão de Clientes</h3>
            </div>
            <ul class="text-purple-500 space-y-2">
              <li>• Cadastro e gerenciamento completo</li>
              <li>• Histórico de interações e projetos</li>
              <li>• Sistema de categorização e notas</li>
              <li>• Armazenamento de documentos</li>
            </ul>
          </div>

          <div class="bg-white rounded-lg p-8 border border-purple-300 shadow transform hover:scale-105 transition-all duration-300">
            <div class="flex items-center mb-6">
              <div class="text-4xl mr-4 text-purple-700">📋</div>
              <h3 class="text-2xl font-bold text-purple-700">Projetos e Entregas</h3>
            </div>
            <ul class="text-purple-500 space-y-2">
              <li>• Criação e acompanhamento de projetos</li>
              <li>• Definição de entregas (milestones)</li>
              <li>• Configuração de prazos e lembretes</li>
              <li>• Gestão de documentos por projeto</li>
            </ul>
          </div>

          <div class="bg-white rounded-lg p-8 border border-purple-300 shadow transform hover:scale-105 transition-all duration-300">
            <div class="flex items-center mb-6">
              <div class="text-4xl mr-4 text-purple-700">💰</div>
              <h3 class="text-2xl font-bold text-purple-700">Gestão Financeira</h3>
            </div>
            <ul class="text-purple-500 space-y-2">
              <li>• Geração de invoices/faturas</li>
              <li>• Controle de pagamentos recebidos</li>
              <li>• Registro de despesas por projeto</li>
              <li>• Exportação para contabilidade</li>
            </ul>
          </div>

          <div class="bg-white rounded-lg p-8 border border-purple-300 shadow transform hover:scale-105 transition-all duration-300">
            <div class="flex items-center mb-6">
              <div class="text-4xl mr-4 text-purple-700">📊</div>
              <h3 class="text-2xl font-bold text-purple-700">Relatórios e Analytics</h3>
            </div>
            <ul class="text-purple-500 space-y-2">
              <li>• Dashboard com métricas principais</li>
              <li>• Relatórios de produtividade</li>
              <li>• Análise financeira por período</li>
              <li>• Exportação em múltiplos formatos</li>
            </ul>
          </div>
        </div>
      </div>
    </section>
  </Layout>
</template>