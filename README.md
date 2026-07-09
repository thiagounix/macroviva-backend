# MacroViva Backend

Backend do MacroViva, uma API para suporte a acompanhamento nutricional e fitness. O backend sera responsavel por receber dados do app mobile, manter a logica de negocio, calcular macronutrientes com base em dados nutricionais proprios e isolar qualquer integracao futura com IA atras de adapters internos.

Esta etapa estabiliza a fundacao tecnica, dominio inicial, Application Layer e Infrastructure Layer. Ainda nao ha endpoints REST de negocio, autenticacao real ou integracao real com IA.

## Stack

- .NET 10
- C#
- ASP.NET Core Web API
- EF Core 10
- SQL Server configuravel
- xUnit
- Clean Architecture
- DDD
- Modular Monolith
- REST API
- Swagger/OpenAPI
- Nullable enabled
- async/await como padrao para fluxos I/O futuros

## Estrutura

```text
/
  MacroViva.sln
  PROJECT_CONTEXT.md
  README.md
  docs/
  infra/
  src/
    MacroViva.Api/
    MacroViva.Domain/
    MacroViva.Application/
    MacroViva.Infrastructure/
    MacroViva.Worker/
  tests/
    MacroViva.Domain.Tests/
    MacroViva.Application.Tests/
```

## Regras de dependencia

- `MacroViva.Domain` nao depende de nenhum outro projeto.
- `MacroViva.Application` depende apenas de `MacroViva.Domain`.
- `MacroViva.Infrastructure` depende de `MacroViva.Application` e `MacroViva.Domain`.
- `MacroViva.Api` depende de `MacroViva.Application` e `MacroViva.Infrastructure`.
- `MacroViva.Worker` depende de `MacroViva.Application` e `MacroViva.Infrastructure`.

## Comandos

```powershell
dotnet restore MacroViva.sln
dotnet build MacroViva.sln
dotnet test MacroViva.sln
```

## Banco local para desenvolvimento

Copie `infra/.env.example` para `infra/.env`, ajuste a senha se necessario e execute:

```powershell
docker compose --env-file infra/.env -f infra/docker-compose.yml up -d
```

O SQL Server local existe apenas como dependencia de desenvolvimento. O backend ja possui `DbContext` e mappings na Infrastructure, mas a API ainda nao expoe endpoints de negocio nem aplica migrations automaticamente.

Connection string de desenvolvimento esperada:

```text
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=MacroViva;User Id=sa;Password=<sua-senha-local>;Encrypt=True;TrustServerCertificate=True
```

Nao versionar senha real. Use `infra/.env` local a partir de `infra/.env.example`.

## Infrastructure

`MacroViva.Infrastructure` contem:

- `MacroVivaDbContext` com EF Core 10 e SQL Server.
- Mappings por entidade em `Persistence/Configurations`.
- Repositorios concretos e `UnitOfWork`.
- `DatabaseSeeder` preparado para dados iniciais de desenvolvimento.
- `LocalFileStorageService`, que salva arquivo local e retorna referencia.
- `MockMealVisionAnalyzer`, sem chamada real a OpenAI ou outro provedor.

Seed preparado:

- alimentos iniciais como banana, arroz, feijao, frango, ovo e whey.
- suplementos iniciais como creatina e whey.
- planos Free, Plus e Pro.

Migration inicial ainda nao foi criada porque `dotnet-ef` nao esta disponivel no ambiente atual. Com a ferramenta instalada, usar:

```powershell
dotnet ef migrations add InitialCreate --project src/MacroViva.Infrastructure --startup-project src/MacroViva.Infrastructure --output-dir Persistence/Migrations
```

## Documentacao

- `docs/ARCHITECTURE.md`: camadas, dependencias e racional do Modular Monolith.
- `docs/DECISIONS.md`: decisoes tecnicas iniciais.
- `docs/BACKLOG.md`: backlog tecnico por etapas.
- `docs/API_CONTRACT.md`: endpoints planejados.
- `docs/SECURITY_PRIVACY.md`: premissas de seguranca, LGPD e dados sensiveis.
- `docs/AI_CONTRACT.md`: contrato futuro para analise de imagem.
- `docs/DOMAIN_DISCOVERY.md`: subdominios, bounded contexts e linguagem ubiqua.
