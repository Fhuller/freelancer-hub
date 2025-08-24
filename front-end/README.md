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

## 🚀 Pré-requisitos

### Software necessário:
- **Node.js** (versão 18+ recomendada)
- **npm** ou **yarn** ou **pnpm**
- **Git**

### Verificar instalação:
```bash
node --version
npm --version
```

## ⚡ Como executar o projeto

### 1. Clonando o repositório
```bash
git clone [URL_DO_REPOSITORIO]
cd freelancer-hub-frontend
```

### 2. Instalação das dependências
```bash
# Usando npm
npm install

# Usando yarn
yarn install

# Usando pnpm
pnpm install
```

### 3. Executando em modo desenvolvimento
```bash
# Usando npm
npm run dev

# Usando yarn
yarn dev

# Usando pnpm
pnpm dev
```

A aplicação estará disponível em: **http://localhost:5173/**

### 4. Build para produção
```bash
# Build otimizado
npm run build

# Preview do build
npm run preview
```

## 📦 Scripts Disponíveis

```bash
# 🔥 Desenvolvimento
npm run dev              # Inicia servidor de desenvolvimento

# 🏗️ Build
npm run build            # Build completo com verificação de tipos
npm run build-only       # Build sem verificação de tipos
npm run preview          # Preview do build de produção

# ✅ Qualidade de código
npm run type-check       # Verificação de tipos TypeScript
npm run lint             # Linting com ESLint (auto-fix)
npm run format           # Formatação com Prettier
```

## 📁 Estrutura do Projeto

```
📁 freelancer-hub-frontend/
├── 📁 public/              # Arquivos estáticos
├── 📁 src/
│   ├── 📁 assets/          # Imagens, estilos, etc.
│   ├── 📁 components/      # Componentes Vue reutilizáveis
│   ├── 📁 views/          # Páginas/Views da aplicação
│   ├── 📁 router/         # Configuração do Vue Router
│   ├── 📁 stores/         # Stores do Pinia
│   ├── 📁 types/          # Definições de tipos TypeScript
│   ├── 📄 App.vue         # Componente raiz
│   └── 📄 main.ts         # Ponto de entrada
├── 📄 index.html          # Template HTML
├── 📄 vite.config.ts      # Configuração do Vite
├── 📄 tsconfig.json       # Configuração TypeScript
├── 📄 package.json        # Dependências e scripts
└── 📄 README.md           # Este arquivo
```

## 🔧 Dependências Principais

### **Runtime Dependencies:**
- `vue@^3.5.13` - Framework principal
- `vue-router@^4.5.0` - Roteamento SPA
- `pinia@^3.0.1` - Gerenciamento de estado

### **Development Dependencies:**
- `vite@^6.2.4` - Build tool e dev server
- `typescript@~5.8.0` - Suporte ao TypeScript
- `@vitejs/plugin-vue@^5.2.3` - Plugin Vue para Vite
- `eslint@^9.22.0` - Linting
- `prettier@3.5.3` - Formatação de código

## 🚀 Deploy Automático - Vercel

### Configuração atual:
- **Platform**: Vercel
- **URL**: https://freelancer-hub-tau.vercel.app/
- **Deploy automático**: Push na branch principal

### Como configurar deploy:

1. **Conecte o repositório ao Vercel:**
   ```bash
   # Install Vercel CLI (opcional)
   npm i -g vercel
   
   # Deploy manual
   vercel --prod
   ```

2. **Configuração automática:**
   - Vercel detecta automaticamente projetos Vite
   - Build command: `npm run build`
   - Output directory: `dist`

### Variáveis de ambiente (se necessário):
```bash
# Criar arquivo .env.local
VITE_API_URL=https://sua-api.com
VITE_APP_TITLE=Freelancer Hub
```

## 🎯 Funcionalidades do Vite

### **Hot Module Replacement (HMR)**
- Atualizações instantâneas durante desenvolvimento
- Preserva estado da aplicação

### **Build otimizado**
- Tree-shaking automático
- Code splitting
- Asset optimization

### **TypeScript nativo**
- Suporte completo ao TypeScript
- Verificação de tipos em tempo real

## 🧹 Qualidade de Código

### **ESLint + Prettier**
```bash
# Verificar problemas
npm run lint

# Formatar código
npm run format

# Verificar tipos
npm run type-check
```

### **Configuração recomendada VSCode:**
```json
{
  "editor.formatOnSave": true,
  "editor.defaultFormatter": "esbenp.prettier-vscode",
  "editor.codeActionsOnSave": {
    "source.fixAll.eslint": true
  }
}
```

## 🐛 Troubleshooting

### Problemas comuns:

1. **Erro de dependências**
   ```bash
   # Limpar cache e reinstalar
   rm -rf node_modules package-lock.json
   npm install
   ```

2. **Porta em uso**
   ```bash
   # Vite usa porta 5173 por padrão
   # Para mudar: vite --port 3000
   ```

3. **Erro de tipos TypeScript**
   ```bash
   # Verificar configuração
   npm run type-check
   ```

4. **Build falha**
   ```bash
   # Verificar se há erros de linting
   npm run lint
   npm run type-check
   npm run build
   ```

## 🌟 Comandos Úteis

```bash
# Análise do bundle
npm run build
npx vite-bundle-analyzer dist

# Atualizar dependências
npm update

# Verificar dependências desatualizadas
npm outdated
```

## 📚 Recursos Adicionais

- [Vue 3 Documentation](https://vuejs.org/)
- [Vite Documentation](https://vitejs.dev/)
- [Pinia Documentation](https://pinia.vuejs.org/)
- [Vue Router Documentation](https://router.vuejs.org/)
- [TypeScript Documentation](https://www.typescriptlang.org/)

## 📞 Suporte

Para dúvidas ou problemas:
1. Verifique os logs no terminal
2. Consulte a documentação das tecnologias
3. Verifique o status do deploy no Vercel
4. Entre em contato com a equipe de desenvolvimento

---

**Desenvolvido com ❤️ usando Vue 3 + Vite + TypeScript** 🚀
