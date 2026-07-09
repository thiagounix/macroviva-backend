# MacroViva Backend

Backend do MacroViva, uma API para suporte a acompanhamento nutricional e fitness. O backend sera responsavel por receber dados do app mobile, manter a logica de negocio, calcular macronutrientes com base em dados nutricionais proprios e isolar qualquer integracao futura com IA atras de adapters internos.

Esta etapa estabiliza a fundacao tecnica, dominio inicial, Application Layer, Infrastructure Layer, API REST inicial e persistencia executavel para desenvolvimento. Nao ha autenticacao real, pagamentos ou integracao real com IA.

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

Rodar API local:

```powershell
dotnet run --project src/MacroViva.Api --launch-profile http
```

URLs locais:

```text
http://localhost:5169/health
http://localhost:5169/swagger
http://localhost:5169/swagger/v1/swagger.json
```

## Banco local para desenvolvimento

Para subir o SQL Server local com os valores de desenvolvimento versionados em exemplo:

```powershell
docker compose --env-file infra/.env.example -f infra/docker-compose.yml up -d
```

Verificar container:

```powershell
docker ps
docker logs macroviva-sqlserver
```

Se o nome `macroviva-sqlserver` ja existir em outro container local, pare ou remova o container antigo antes de subir novamente:

```powershell
docker stop macroviva-sqlserver
docker rm macroviva-sqlserver
```

Opcionalmente copie `infra/.env.example` para `infra/.env`, ajuste a senha local e use:

```powershell
docker compose --env-file infra/.env -f infra/docker-compose.yml up -d
```

O SQL Server local existe apenas como dependencia de desenvolvimento.

Connection string de desenvolvimento esperada:

```text
ConnectionStrings__DefaultConnection=Server=localhost,1433;Database=MacroViva;User Id=sa;Password=<sua-senha-local>;Encrypt=True;TrustServerCertificate=True
```

Nao versionar senha real. Use `infra/.env` local a partir de `infra/.env.example`.

## Migrations

Se `dotnet-ef` nao estiver instalado:

```powershell
dotnet tool update --global dotnet-ef
```

Criar migration:

```powershell
dotnet ef migrations add InitialCreate --project src/MacroViva.Infrastructure --startup-project src/MacroViva.Api --output-dir Persistence/Migrations
```

Este comando so deve ser executado se a migration inicial ainda nao existir no repositorio.

Aplicar migration:

```powershell
dotnet ef database update --project src/MacroViva.Infrastructure --startup-project src/MacroViva.Api
```

`DesignTimeDbContextFactory` fica na Infrastructure e usa a connection string de desenvolvimento quando necessario.

## Infrastructure

`MacroViva.Infrastructure` contem:

- `MacroVivaDbContext` com EF Core 10 e SQL Server.
- Mappings por entidade em `Persistence/Configurations`.
- Repositorios concretos e `UnitOfWork`.
- `DatabaseSeeder` preparado para dados iniciais de desenvolvimento.
- `LocalFileStorageService`, que salva arquivo local e retorna referencia.
- `MockMealVisionAnalyzer`, sem chamada real a OpenAI ou outro provedor.

Seed de desenvolvimento:

- alimentos iniciais como banana, arroz, feijao, frango, ovo e whey.
- suplementos iniciais como creatina e whey.
- planos Free, Plus e Pro.

O seed e idempotente por IDs deterministicos e pode rodar no startup apenas em Development quando `Seed:RunOnStartup=true` em `appsettings.Development.json`.
Ele tambem verifica chave natural simples antes de inserir, evitando duplicidade se dados equivalentes ja existirem.

## Endpoints iniciais e smoke test

```text
GET  /health
GET  /api/foods
GET  /api/foods/{id}
POST /api/meals
GET  /api/meals/today
POST /api/ai/meal-photo/analyze
POST /api/ai/meal-photo/{analysisId}/confirm
GET  /api/supplements
POST /api/user-supplements/check-in
```

Sequencia basica para smoke test local:

1. Subir SQL Server local.
2. Aplicar migration.
3. Rodar a API em Development.
4. Abrir `http://localhost:5169/swagger`.
5. Executar `GET /api/foods` e usar um `foodId` real retornado pelo seed.
6. Executar `POST /api/meals`.
7. Executar `GET /api/meals/today`.
8. Executar `GET /api/supplements` e usar um `supplementId` real retornado pelo seed.
9. Executar `POST /api/user-supplements/check-in`.
10. Executar `POST /api/ai/meal-photo/analyze` com `multipart/form-data`, campo `file`.
11. Executar `POST /api/ai/meal-photo/{analysisId}/confirm` usando `selectedFoodId` real do catalogo.

Exemplo de corpo para `POST /api/meals`:

```json
{
  "mealType": 2,
  "occurredAt": "2026-07-09T12:00:00Z",
  "items": [
    {
      "foodId": "10000000-0000-0000-0000-000000000004",
      "grams": 120
    }
  ]
}
```

Exemplo de corpo para confirmacao de analise:

```json
{
  "mealType": 2,
  "occurredAt": "2026-07-09T12:00:00Z",
  "items": [
    {
      "analysisItemId": "11111111-1111-1111-1111-111111111111",
      "selectedFoodId": "10000000-0000-0000-0000-000000000004",
      "grams": 100
    }
  ]
}
```

## Documentacao

- `docs/ARCHITECTURE.md`: camadas, dependencias e racional do Modular Monolith.
- `docs/DECISIONS.md`: decisoes tecnicas iniciais.
- `docs/BACKLOG.md`: backlog tecnico por etapas.
- `docs/API_CONTRACT.md`: endpoints planejados.
- `docs/SECURITY_PRIVACY.md`: premissas de seguranca, LGPD e dados sensiveis.
- `docs/AI_CONTRACT.md`: contrato futuro para analise de imagem.
- `docs/DOMAIN_DISCOVERY.md`: subdominios, bounded contexts e linguagem ubiqua.
