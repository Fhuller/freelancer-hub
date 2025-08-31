# Projeto .NET 8

## 📋 Descrição
Este é um projeto desenvolvido em .NET 8 que utiliza SQL Server hospedado no Azure como banco de dados e implementa CI/CD para deploy automatizado.
Link API: https://freelancer-hub-backend-exe7f6cygggae0db.eastus2-01.azurewebsites.net/swagger/index.html

## 🛠️ Pré-requisitos

### Software necessário:
- **Visual Studio Community** (versão mais recente recomendada)
- **.NET 8 SDK**
- **SQL Server Management Studio** (opcional, para gerenciamento do banco)

### Serviços Azure:
- **Azure SQL Database**
- **Azure App Service**

## 🚀 Como executar o projeto

### 1. Clonando o repositório
```bash
git clone https://github.com/Fhuller/freelancer-hub
cd ./back-end/freelancer-hub-backend/
```

### 2. Configuração no Visual Studio Community
1. Abra o **Visual Studio Community**
2. Selecione **"Abrir um projeto ou solução"**
3. Navegue até a pasta do projeto e selecione o arquivo `freelancer-hub-backend.sln`
4. Aguarde o carregamento completo da solução

### 3. Configuração do banco de dados

#### Connection String
As informações de conexão com o SQL Server do Azure estão armazenadas de forma segura utilizando **User Secrets**.

Para configurar localmente:

1. No Visual Studio, clique com o botão direito no projeto
2. Selecione **"Manage User Secrets"**
3. Adicione a connection string no arquivo `secrets.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:[SERVIDOR].database.windows.net,1433;Initial Catalog=[DATABASE];Persist Security Info=False;User ID=[USUARIO];Password=[SENHA];MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

> ⚠️ **Importante**: Nunca commite informações sensíveis como connection strings diretamente no código!

### 4. Executando o projeto
1. No Visual Studio, pressione **F5** ou clique em **"Iniciar Depuração"**
2. O projeto será compilado e executado automaticamente
3. Uma janela do navegador será aberta com a aplicação

## 🔧 Estrutura do projeto

```
📁 back-end/
├── 📁 freelancer-hub-backend/
|    ├── 📁 Controllers/         # Controladores da API
|    ├── 📁 DTO's/               # Data transfer objects pra API
|    ├── 📁 Migrations/          # Historico de criação do banco
|    ├── 📁 Models/              # Modelos de dados
|    ├── 📁 Repository/          # CRUD direto no context
|    ├── 📁 Utils/               # Funções úteis em toda aplicação
|    ├── 📁 Services/            # Serviços e lógica de negócio
|    ├── 📄 Program.cs           # Ponto de entrada da aplicação
|    ├── 📄 Context.cs           # Definiçao do banco no EF
|    └── 📄 appsettings.json     # Configurações da aplicação
└── 📄 README.md                 # Este arquivo 
```

## 🚀 CI/CD - Deploy Automatizado

### GitHub Actions
O projeto possui pipeline de CI/CD configurado com **GitHub Actions** para deploy automatizado no **Azure App Service**.

#### Workflow automático:
- **Trigger**: Push na branch `main`
- **Build**: Compila o projeto .NET 8
- **Deploy**: Publica automaticamente no Azure App Service

#### Arquivos de configuração:
```
📁 .github/
└── 📁 workflows/
    └── 📄 azure-deploy.yml    # Pipeline de deploy
```
