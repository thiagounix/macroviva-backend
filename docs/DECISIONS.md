# Decisions

Registro de decisoes iniciais do MacroViva Backend.

## MV-001: Backend separado do app mobile

Backend e mobile permanecem como solucoes separadas. Nao sera criado monorepo com Flutter nesta etapa.

## MV-002: Modular Monolith

O backend sera um Modular Monolith, nao microservicos. A prioridade e clareza de dominio, baixo custo operacional e evolucao incremental.

## MV-003: Clean Architecture

As dependencias apontam para dentro. `Domain` nao referencia ninguem, `Application` referencia apenas `Domain`, e detalhes externos ficam em `Infrastructure`.

## MV-004: REST API

A interface publica inicial sera REST sobre ASP.NET Core Web API. Swagger/OpenAPI sera usado para documentar contratos.

## MV-005: EF Core e SQL Server

EF Core 10 e SQL Server serao a base de persistencia. Nesta etapa os pacotes e ambiente local sao preparados, mas nenhum schema, migration ou `DbContext` real e criado.

## MV-006: Autenticacao preparada, nao implementada

JWT Bearer fica preparado como dependencia futura na API. Autenticacao real, emissao de tokens, validacao de issuer/audience e autorizacao por roles/claims ficam para etapa posterior.

## MV-007: IA por adapter

Toda integracao com IA sera isolada por adapter na infraestrutura. A primeira implementacao deve ser mock. Nenhuma chamada real a OpenAI sera feita nesta etapa.

## MV-008: Imagem nao salva no banco

Imagens de refeicao nao devem ser persistidas no banco relacional. O contrato futuro deve tratar imagem como entrada temporaria de processamento, com politica explicita de retencao.

## MV-009: Codigo em ingles, documentacao em portugues

Codigo, namespaces, tipos e membros devem ser escritos em ingles. A documentacao do projeto pode ser em portugues.
