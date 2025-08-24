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
git clone [URL_DO_REPOSITORIO]
cd [NOME_DO_PROJETO]
```

### 2. Configuração no Visual Studio Community
1. Abra o **Visual Studio Community**
2. Selecione **"Abrir um projeto ou solução"**
3. Navegue até a pasta do projeto e selecione o arquivo `.sln`
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
📁 [NOME_DO_PROJETO]/
├── 📁 Controllers/          # Controladores da API
├── 📁 Models/              # Modelos de dados
├── 📁 Services/            # Serviços e lógica de negócio
├── 📁 Data/                # Contexto do Entity Framework
├── 📄 Program.cs           # Ponto de entrada da aplicação
├── 📄 appsettings.json     # Configurações da aplicação
└── 📄 [PROJETO].csproj     # Arquivo de projeto
```

## 🚀 CI/CD - Deploy Automatizado

### GitHub Actions
O projeto possui pipeline de CI/CD configurado com **GitHub Actions** para deploy automatizado no **Azure App Service**.

#### Workflow automático:
- **Trigger**: Push na branch `main` ou `master`
- **Build**: Compila o projeto .NET 8
- **Testes**: Executa testes unitários (se configurados)
- **Deploy**: Publica automaticamente no Azure App Service

#### Arquivos de configuração:
```
📁 .github/
└── 📁 workflows/
    └── 📄 azure-deploy.yml    # Pipeline de deploy
```

### Variáveis de ambiente (GitHub Secrets)
Configure as seguintes secrets no repositório GitHub:

- `AZURE_WEBAPP_PUBLISH_PROFILE`: Profile de publicação do App Service
- `AZURE_WEBAPP_NAME`: Nome do App Service no Azure
- `SQL_CONNECTION_STRING`: Connection string do banco (para produção)

## 🔐 Segurança

### Secrets Management
- **Desenvolvimento**: User Secrets do Visual Studio
- **Produção**: Azure Key Vault ou App Service Configuration

### Boas práticas implementadas:
- Connection strings não expostas no código
- Configurações sensíveis em variáveis de ambiente
- Deploy seguro via GitHub Actions

## 📦 Dependências principais

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.x" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.x" />
<PackageReference Include="Microsoft.AspNetCore.Authentication" Version="8.0.x" />
```

## 🐛 Troubleshooting

### Problemas comuns:

1. **Erro de conexão com o banco**
   - Verifique se a connection string está correta nos User Secrets
   - Confirme se o firewall do Azure SQL permite conexões

2. **Falha no deploy**
   - Verifique se as GitHub Secrets estão configuradas
   - Confirme se o publish profile está válido

3. **Projeto não carrega no Visual Studio**
   - Verifique se o .NET 8 SDK está instalado
   - Execute `dotnet restore` no terminal

## 📞 Suporte

Para dúvidas ou problemas:
1. Verifique a documentação do projeto
2. Consulte os logs de build no GitHub Actions
3. Entre em contato com a equipe de desenvolvimento

---

**Desenvolvido com .NET 8 + Azure + GitHub Actions** 🚀
