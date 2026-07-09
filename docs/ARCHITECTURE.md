# Architecture

MacroViva Backend segue Clean Architecture com DDD e sera evoluido como Modular Monolith. A decisao principal e manter um unico deployable backend enquanto os limites internos sao desenhados por contexto de negocio.

## Camadas

### MacroViva.Domain

Camada de dominio puro. Deve conter regras de negocio, entidades, value objects, domain services e eventos de dominio quando forem necessarios. Nao referencia nenhum outro projeto.

Nesta etapa ela permanece sem dominio completo.

### MacroViva.Application

Camada de casos de uso e contratos de aplicacao. Deve orquestrar regras do dominio, expor interfaces para infraestrutura e definir DTOs internos quando necessario. Pode depender apenas de `MacroViva.Domain`.

Nao deve conter detalhes de ASP.NET, EF Core, SQL Server, OpenAI ou provedores externos.

### MacroViva.Infrastructure

Camada de detalhes tecnicos. Deve conter implementacoes de persistencia, providers externos, adapters, clock, storage e clientes de servicos externos. Pode depender de `MacroViva.Application` e `MacroViva.Domain`.

EF Core 10 e SQL Server estao preparados por pacote, mas nenhum `DbContext` real foi criado nesta etapa.

### MacroViva.Api

Camada de entrega HTTP REST. Deve conter controllers, filtros, configuracao de middleware, OpenAPI e composicao de dependencias.

A API nao implementa regra de negocio. Endpoints futuros devem chamar a camada de aplicacao, usar `async/await` e receber `CancellationToken`.

### MacroViva.Worker

Processo para jobs e tarefas assincronas futuras. Pode depender de `MacroViva.Application` e `MacroViva.Infrastructure`. Nao deve duplicar regra de negocio.

## Regras de dependencia

```text
Domain
Application -> Domain
Infrastructure -> Application, Domain
Api -> Application, Infrastructure
Worker -> Application, Infrastructure
Tests -> projeto sob teste
```

Qualquer dependencia inversa deve ser tratada por interfaces na camada de aplicacao e implementacoes na infraestrutura.

## Modular Monolith

O MacroViva sera um Modular Monolith porque:

- o produto ainda esta em descoberta;
- os limites de dominio precisam amadurecer antes de qualquer distribuicao;
- um unico deploy reduz custo operacional e complexidade;
- modulos internos ainda podem proteger limites de contexto;
- evita microservicos prematuros.

Modulos internos provaveis:

- Identity and Access
- Nutrition Catalog
- Meal Analysis
- Food Logging
- User Goals
- Supplements
- Notifications

Esses modulos sao candidatos, nao implementacoes desta etapa.

## IA

A IA sempre fica atras do backend. O app mobile nunca deve chamar OpenAI ou outro provedor diretamente. A IA identifica alimentos provaveis e estima porcoes; o backend calcula macros e exige confirmacao do usuario antes de salvar.
