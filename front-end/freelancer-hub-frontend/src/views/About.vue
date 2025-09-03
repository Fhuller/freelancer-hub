<script>
import Layout from '@/layouts/LandingPageLayout.vue'

export default {
  name: 'About',
  components: { Layout },
  mounted() {
    document.title = 'Sobre - FreelancerHub'
  }
}
</script>

<template>
  <Layout>
    <!-- Hero Section -->
    <section class="py-20 text-center bg-white/90">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <h1 class="text-5xl md:text-6xl font-bold text-purple-700 mb-6">
          Sobre Mim
        </h1>
        <p class="text-xl text-purple-500 mb-8">
          Felipe Beppler Huller
        </p>
        <p class="text-xl text-purple-500 mb-8">
          Desenvolvedor full stack, estudante de Engenharia de Software e apaixonado por música
        </p>
      </div>
    </section>

    <!-- Main Content -->
    <section class="py-16 bg-white/90">
      <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="grid lg:grid-cols-2 gap-12 items-center">
          <!-- Profile -->
          <div class="text-center lg:text-left">
            <div class="inline-block p-8 bg-purple-200/30 rounded-full mb-8">
              <div class="text-8xl text-purple-700">👨‍💻</div>
            </div>
            <div class="space-y-6">
              <div class="flex items-center justify-center lg:justify-start space-x-4">
                <div class="text-3xl text-purple-700">🎸</div>
                <span class="text-purple-700 text-lg">Músico apaixonado por Rock</span>
              </div>
              <div class="flex items-center justify-center lg:justify-start space-x-4">
                <div class="text-3xl text-purple-700">🌍</div>
                <span class="text-purple-700 text-lg">Fluente em Inglês</span>
              </div>
              <div class="flex items-center justify-center lg:justify-start space-x-4">
                <div class="text-3xl text-purple-700">⚙️</div>
                <span class="text-purple-700 text-lg">Especialista em Back-end</span>
              </div>
            </div>
          </div>

          <!-- Description -->
          <div class="space-y-8">
            <div class="bg-white backdrop-blur-md rounded-lg p-8 border border-purple-300/20">
              <h2 class="text-3xl font-bold text-purple-700 mb-6 flex items-center">
                <span class="mr-3">🚀</span>
                Desenvolvedor em Formação
              </h2>
              <p class="text-purple-500 leading-relaxed mb-4">
                Olá! Sou um entusiasta da programação com foco em desenvolvimento fullstack, atualmente cursando 
                <strong class="text-purple-700">Engenharia de Software</strong>. Minha jornada profissional já me levou por três 
                empolgantes experiências como desenvolvedor, onde pude aprimorar minhas habilidades e paixão pela tecnologia.
              </p>
              <p class="text-purple-500 leading-relaxed">
                Além do meu amor pela programação, minha outra grande paixão é a música, especialmente o gênero de 
                <strong class="text-purple-700">rock</strong>. Quando não estou imerso no código, estou frequentemente tocando minha 
                guitarra e explorando os sons e ritmos que definem esse gênero.
              </p>
            </div>

            <div class="bg-white backdrop-blur-md rounded-lg p-8 border border-purple-300/20">
              <h3 class="text-2xl font-bold text-purple-700 mb-4 flex items-center">
                <span class="mr-3">💡</span>
                Filosofia & Abordagem
              </h3>
              <p class="text-purple-500 leading-relaxed mb-4">
                Acredito que essa paixão pela música se alinha bem com a criatividade e a disciplina necessárias no mundo 
                da programação. Minha fluência em inglês permite colaborar de maneira eficaz em equipes internacionais e acessar recursos globais para manter-me atualizado.
              </p>
              <p class="text-purple-500 leading-relaxed">
                Estou animado para continuar aprendendo, crescendo e contribuindo para projetos empolgantes no mundo do 
                desenvolvimento.
              </p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Skills Section -->
    <section class="py-20 bg-white/90">
      <div class="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <h2 class="text-4xl font-bold text-purple-700 text-center mb-16">Habilidades & Interesses</h2>
        
        <div class="grid md:grid-cols-3 gap-8">
          <div class="bg-white backdrop-blur-md rounded-lg p-8 border border-purple-300/30 text-center transform hover:scale-105 transition-all duration-300">
            <div class="text-6xl mb-4 text-purple-700">💻</div>
            <h3 class="text-2xl font-bold text-purple-700 mb-4">Desenvolvimento</h3>
            <ul class="text-purple-500 space-y-2">
              <li>.NET Core</li>
              <li>C#</li>
              <li>APIs RESTful</li>
              <li>SQL Server</li>
              <li>Vue.js</li>
            </ul>
          </div>

          <div class="bg-white backdrop-blur-md rounded-lg p-8 border border-purple-300/30 text-center transform hover:scale-105 transition-all duration-300">
            <div class="text-6xl mb-4 text-purple-700">🎸</div>
            <h3 class="text-2xl font-bold text-purple-700 mb-4">Música</h3>
            <ul class="text-purple-500 space-y-2">
              <li>Guitarra</li>
              <li>Rock Music</li>
            </ul>
          </div>

          <div class="bg-white backdrop-blur-md rounded-lg p-8 border border-purple-300/30 text-center transform hover:scale-105 transition-all duration-300">
            <div class="text-6xl mb-4 text-purple-700">🌐</div>
            <h3 class="text-2xl font-bold text-purple-700 mb-4">Soft Skills</h3>
            <ul class="text-purple-500 space-y-2">
              <li>Inglês Fluente</li>
              <li>Trabalho em Equipe</li>
              <li>Colaboração Internacional</li>
              <li>Aprendizado Contínuo</li>
              <li>Resolução de Problemas</li>
            </ul>
          </div>
        </div>
      </div>
    </section>

    <!-- Experience Section -->
    <section class="py-20 bg-white/90 text-center">
      <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <h2 class="text-4xl font-bold text-purple-700 mb-8">Experiência Profissional</h2>
        <div class="bg-white backdrop-blur-md rounded-lg p-8 border border-purple-300/20 mb-8">
          <h3 class="text-2xl font-bold text-purple-700 mb-4">Desenvolvedor FullStack</h3>
          <p class="text-purple-500 text-lg mb-4">3 experiências profissionais enriquecedoras</p>
          <p class="text-purple-500 leading-relaxed">
            Durante minha jornada profissional, trabalhei em 3 experiências como desenvolvedor, aprimorando habilidades técnicas, colaborando em equipes multidisciplinares e contribuindo para soluções robustas e escaláveis.
          </p>
        </div>
      </div>
    </section>
  </Layout>
</template>