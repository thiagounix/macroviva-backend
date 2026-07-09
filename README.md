# MacroViva Backend

Backend do MacroViva, uma API para suporte a acompanhamento nutricional e fitness. O backend sera responsavel por receber dados do app mobile, manter a logica de negocio, calcular macronutrientes com base em dados nutricionais proprios e isolar qualquer integracao futura com IA atras de adapters internos.

Esta etapa estabiliza apenas a fundacao tecnica. Nao ha dominio completo, persistencia configurada em runtime, endpoints de negocio, autenticacao real ou integracao real com IA.

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

O SQL Server local existe apenas como dependencia de desenvolvimento. O backend ainda nao registra `DbContext`, migrations ou schema nesta etapa.

## Documentacao

- `docs/ARCHITECTURE.md`: camadas, dependencias e racional do Modular Monolith.
- `docs/DECISIONS.md`: decisoes tecnicas iniciais.
- `docs/BACKLOG.md`: backlog tecnico por etapas.
- `docs/API_CONTRACT.md`: endpoints planejados.
- `docs/SECURITY_PRIVACY.md`: premissas de seguranca, LGPD e dados sensiveis.
- `docs/AI_CONTRACT.md`: contrato futuro para analise de imagem.
- `docs/DOMAIN_DISCOVERY.md`: subdominios, bounded contexts e linguagem ubiqua.
