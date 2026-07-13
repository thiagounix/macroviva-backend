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

EF Core 10 e SQL Server serao a base de persistencia.

A Infrastructure contem `MacroVivaDbContext`, mappings explicitos, repositorios concretos e `UnitOfWork`. Connection string deve vir de configuracao ou variavel de ambiente, sem senha real versionada.

## MV-006: Autenticacao preparada, nao implementada

JWT Bearer fica preparado como dependencia futura na API. Autenticacao real, emissao de tokens, validacao de issuer/audience e autorizacao por roles/claims ficam para etapa posterior.

## MV-007: IA por adapter

Toda integracao com IA sera isolada por adapter na infraestrutura. A primeira implementacao deve ser mock. Nenhuma chamada real a OpenAI sera feita nesta etapa.

## MV-008: Imagem nao salva no banco

Imagens de refeicao nao devem ser persistidas no banco relacional. O contrato futuro deve tratar imagem como entrada temporaria de processamento, com politica explicita de retencao.

## MV-009: Codigo em ingles, documentacao em portugues

Codigo, namespaces, tipos e membros devem ser escritos em ingles. A documentacao do projeto pode ser em portugues.

## MV-010: DomainException para invariantes de dominio

Nesta etapa o dominio usa `DomainException` para rejeitar estados invalidos em construtores, factories e metodos de dominio.

A alternativa seria um Result Pattern, mas ele sera mais util na camada Application, quando existirem casos de uso e mapeamento consistente para respostas HTTP. Para o nucleo de dominio inicial, exceptions deixam as invariantes simples, diretas e testaveis.

Na Application Layer sera criado um `Result<T>` padronizado. A API nao deve depender de exceptions para fluxo normal; ela deve traduzir resultados da Application para HTTP.

## MV-011: Aggregates iniciais pequenos

Os aggregates iniciais foram criados para representar os bounded contexts Users, Nutrition, Meals, AIAnalysis, Supplements, Subscriptions e ConsentAndPrivacy.

Eles ainda nao incluem persistencia, EF Core mappings, endpoints REST ou integracao com IA. A prioridade e proteger regras centrais e linguagem ubiqua.

## MV-012: IA nao e fonte final de calorias

`AIAnalysis` modela sugestoes temporarias de alimentos e porcoes. A confirmacao da analise nao salva uma refeicao automaticamente.

O calculo final de macros pertence ao backend e usa `Food.NutritionPer100g` ao criar `MealItem`.

`AIAnalysisItem.SuggestedFoodId` e apenas sugestao/candidato. Ele nao representa o alimento final confirmado pelo usuario. A camada Application futuramente recebera o `selectedFoodId`, carregara o alimento confirmado e criara a `Meal`.

## MV-013: User nao representa autenticacao real nesta fase

`User` representa o usuario do produto dentro do dominio. Ele nao deve conter senha, refresh token, provider externo ou regras de autenticacao.

Autenticacao real, JWT, Apple/Google login e refresh token ficam fora do dominio neste momento.

## MV-014: Check-ins e assinaturas como aggregate roots separados

`UserSupplement` sera tratado como aggregate root separado, vinculado por `UserId` e `SupplementId`, porque tem ciclo de vida proprio de rotina e check-in.

`UserSubscription` sera tratado como aggregate root separado, vinculado por `UserId` e `SubscriptionPlanId`, porque assinatura tem ciclo de vida proprio.

`User` nao deve virar um aggregate gigante nem conter todo o historico de refeicoes, suplementos ou assinaturas.

## MV-015: Result Pattern na Application Layer

A camada `Application` usa `Result<T>` para respostas previsiveis de casos de uso.

O dominio continua podendo lancar `DomainException` para proteger invariantes. A Application deve prevenir erros conhecidos quando possivel e converter `DomainException` em `Result.Failure`, sem deixar exceptions virarem fluxo normal da futura API.

## MV-016: Application orquestra, Domain decide regras centrais

Casos de uso da `Application` coordenam repositorios, servicos externos abstratos e aggregates do dominio.

A criacao de uma refeicao confirmada a partir de uma analise de imagem usa o `selectedFoodId` enviado pelo usuario. `AIAnalysisItem.SuggestedFoodId` permanece apenas sugestao da IA e nunca representa confirmacao final.

Contratos da Application nao devem depender de ASP.NET. Uploads futuros devem ser adaptados na API para abstracoes neutras como `Stream`, `fileName` e `contentType`.

## MV-017: Sem CQRS/MediatR/AutoMapper nesta fase

Para o MVP inicial, os casos de uso ficam como classes explicitas e pequenas. Nao foram adicionados MediatR, CQRS completo, AutoMapper ou outros pacotes de orquestracao.

Essa escolha reduz complexidade enquanto os fluxos principais ainda estao sendo descobertos.

## MV-018: IClock.Today e timezone do usuario

`IClock.Today` existe como abstracao simples para consultas e check-ins diarios no MVP.

Em etapa futura, datas locais devem considerar timezone/locale do usuario. A regra atual nao deve ser usada como decisao final para usuarios em fusos diferentes.

## MV-019: Adapters temporarios de Infrastructure

`LocalFileStorageService` salva arquivos em pasta configuravel e retorna apenas referencia local. O banco nao deve armazenar binario de imagem.

`MockMealVisionAnalyzer` simula a resposta de IA para desenvolvimento. OpenAI real, Azure, custos, quotas e politicas de provedor ficam para etapa futura.

`DevelopmentCurrentUserService` fornece usuario corrente fixo/configuravel apenas para desenvolvimento. Ele nao representa login, token, senha, refresh token ou autenticacao real.

## MV-020: Migrations

O `DbContext`, mappings e `DesignTimeDbContextFactory` estao prontos para migrations. A migration inicial foi criada na Infrastructure usando a API como startup project.

Comando padrao:

```powershell
dotnet ef migrations add InitialCreate --project src/MacroViva.Infrastructure --startup-project src/MacroViva.Api --output-dir Persistence/Migrations
dotnet ef database update --project src/MacroViva.Infrastructure --startup-project src/MacroViva.Api
```

## MV-021: Controllers finos na API

A API usa controllers ASP.NET Core para expor REST e Swagger.

Controllers devem apenas receber request HTTP, fazer validacoes basicas de transporte, chamar use cases da Application e converter `Result<T>` para HTTP. Eles nao acessam `DbContext`, repositories ou regras de dominio diretamente.

Upload usa `IFormFile` somente na camada API. Antes de chamar Application, o controller converte para `Stream`, `fileName` e `contentType`.

Autenticacao JWT permanece preparada/inativa, sem obrigar autorizacao nos endpoints desta etapa.

## MV-022: Seed controlado por configuração e idempotente

Dados iniciais sao inseridos por `DatabaseSeeder` somente quando `Seed:RunOnStartup=true`. Development pode habilitar o seed para uso local; Staging tambem pode habilita-lo explicitamente, embora o padrao seja `Seed:RunOnStartup=false` para evitar execucao acidental. O seed permanece controlado por configuracao de ambiente.

O seed usa IDs deterministicos para alimentos, suplementos e planos de assinatura. Antes de inserir, consulta IDs existentes e chave natural simples:

- alimentos: nome, categoria e indicador de suplemento;
- suplementos: nome e tipo;
- planos: nome e tier.

Com isso, execucoes repetidas nao duplicam dados e dados equivalentes inseridos manualmente nao recebem duplicata do seed.

O seed nao contem dados sensiveis reais.

## MV-023: Materializacao EF sem abrir o dominio

Entidades, aggregates e value objects mantem encapsulamento e validacoes publicas por factories/construtores expressivos.

Para permitir materializacao pelo EF Core, foram adicionados construtores privados/protegidos sem parametros com inicializadores neutros. Essa decisao e infraestrutura de persistencia e nao deve virar API publica de criacao de objetos invalidos.
