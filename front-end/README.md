# Freelancer Hub - Frontend

## 🌐 Demo
**Aplicação em produção**: [https://freelancer-hub-tau.vercel.app/](https://freelancer-hub-tau.vercel.app/)

## 📋 Descrição
Frontend da aplicação Freelancer Hub desenvolvido em **Vue 3** com **TypeScript**, utilizando **Vite** como bundler e **Vercel** para deploy automatizado.

## 🛠️ Stack Tecnológica

- **Vue 3** - Framework JavaScript progressivo
- **TypeScript** - Superset tipado do JavaScript
- **Vite** - Build tool e dev server ultra-rápido
- **Pinia** - Gerenciamento de estado para Vue
- **Vue Router** - Roteamento SPA
- **Vercel** - Plataforma de deploy

## ⚡ Como executar o projeto

### 1. Clonando o repositório
```bash
git clone https://github.com/Fhuller/freelancer-hub
cd front-end/freelancer-hub-frontend
```

### 2. Instalação das dependências
```bash
# Usando npm
npm install
```

### 3. Executando em modo desenvolvimento
```bash
# Usando npm
npm run dev
```

A aplicação estará disponível em: **http://localhost:5173/**

### 4. Build para produção
```bash
# Build otimizado
npm run build

# Preview do build
npm run preview
```

A aplicação estará disponível em: **http://localhost:4173/**

## 📁 Estrutura do Projeto

```
📁 front-end/
├──📁 freelancer-hub-frontend/
│   ├── 📁 src/
│   │   ├── 📁 assets/       # Imagens, estilos, etc.
│   │   ├── 📁 components/   # Componentes Vue reutilizáveis
│   │   ├── 📁 layouts/      # Layout base das páginas
│   │   ├── 📁 locales/      # Arquivos json de tradução
│   │   ├── 📁 router/       # Configuração do Vue Router
│   │   ├── 📁 services/     # Para request das api's
│   │   ├── 📁 stores/       # Stores do Pinia
│   │   ├── 📁 views/        # Páginas/Views da aplicação
│   │   └── 📄 App.vue       # Componente raiz
│   ├── 📄 .env              # Variáveis de ambiente
│   ├── 📄 index.html        # Template HTML
│   ├── 📄 vite.config.ts    # Configuração do Vite
│   ├── 📄 tsconfig.json     # Configuração TypeScript
│   ├── 📄 package.json      # Dependências e scripts
│   ├── 📄.gitignore         # Config de arquivos que não vão subir pro git
└── 📄 README.md             # Este arquivo
```
