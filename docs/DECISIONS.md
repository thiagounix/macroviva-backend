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
