# Domain Discovery

Documento inicial de descoberta de dominio. Nao representa modelagem final.

## Proposito do produto

Ajudar usuarios a registrar refeicoes, estimar macronutrientes e acompanhar metas de calorias e proteina, com foco inicial em academia, emagrecimento e rotina alimentar.

O produto nao substitui medico, nutricionista ou outro profissional de saude.

## Subdominios candidatos

### Core

- Meal analysis
- Food logging
- Macro calculation
- Nutrition catalog
- User goals

### Supporting

- Identity and access
- Notifications
- Subscription and billing
- Supplement tracking

### Generic

- Email delivery
- File/object storage, se for aprovado no futuro
- Observability

## Bounded contexts iniciais

### Users

Responsavel pela identidade de dominio do usuario, perfil, objetivos e dados sensiveis necessarios para personalizacao.

Aggregates e entidades iniciais:

- `User`
- `UserProfile`
- `UserGoal`

### Nutrition

Responsavel por alimentos, nutrientes, medidas, porcoes e calculo de macros usando a base nutricional propria.

Aggregates e entidades iniciais:

- `Food`
- `FoodPortion`

### Meals

Responsavel por refeicoes confirmadas, itens registrados, fotos temporarias e historico alimentar.

Aggregates e entidades iniciais:

- `Meal`
- `MealItem`
- `MealPhoto`

### AIAnalysis

Responsavel por transformar imagem em candidatos de alimentos e porcoes estimadas. Nao e fonte da verdade nutricional e nao cria refeicao automaticamente.

Aggregates e entidades iniciais:

- `AIAnalysis`
- `AIAnalysisItem`

Fluxo correto:

```text
Photo -> AIAnalysis -> Suggested items -> User confirmation -> Application creates Meal
```

`AIAnalysisItem.SuggestedFoodId` representa apenas uma sugestao ou candidato gerado por IA/matching. Ele nao e o alimento final confirmado. A selecao final sera feita futuramente pela camada Application a partir da confirmacao explicita do usuario.

### Supplements

Responsavel por suplementos basicos. Whey pode impactar macros; creatina pode ser check-in sem impacto nutricional relevante.

Aggregates e entidades iniciais:

- `Supplement`
- `UserSupplement`

### Subscriptions

Responsavel por planos e assinaturas em nivel de dominio, sem pagamento real nesta etapa.

Aggregates e entidades iniciais:

- `SubscriptionPlan`
- `UserSubscription`

### ConsentAndPrivacy

Responsavel por consentimentos e premissas de privacidade relacionadas a dados sensiveis.

Aggregates e entidades iniciais:

- `UserConsent`

## Linguagem ubiqua inicial

- Meal: refeicao registrada ou em processo de confirmacao.
- Meal analysis: resultado temporario da analise de imagem.
- Food candidate: alimento sugerido pela IA.
- Portion estimate: estimativa de porcao que exige confirmacao.
- Confirmed meal: refeicao validada pelo usuario.
- Nutrition facts: informacoes nutricionais por unidade ou porcao.
- Macro calculation: calculo de calorias, proteinas, carboidratos e gorduras.
- User goal: meta definida pelo usuario, sem prescricao profissional.
- Macronutrients snapshot: copia dos macros calculados no momento do registro de um item de refeicao.
- User consent: decisao explicita do usuario sobre uso de dados ou processamento sensivel.

## Regras de dominio implementadas na etapa 2

- `Food` possui valores nutricionais por 100g.
- `NutritionPer100g` calcula macros por gramas usando `Portion`.
- `Meal` pertence obrigatoriamente a um usuario.
- `MealItem` guarda snapshot do nome e dos macros calculados no momento do registro.
- `Meal` recalcula totais ao adicionar ou remover itens.
- `AIAnalysis` pode iniciar como pending ou completed.
- `AIAnalysis` nao vira `Meal` automaticamente.
- `AIAnalysis` so pode ser confirmada se estiver completed e tiver itens sugeridos.
- `AIAnalysis.CompletedAt` nao pode ser anterior a `CreatedAt`.
- `AIAnalysis.ConfirmedAt` nao pode ser anterior a `CompletedAt`.
- `AIAnalysis` nao pode ser confirmada duas vezes.
- `AIAnalysisItem.SuggestedFoodId` e apenas sugestao, nao confirmacao final.
- Whey protein pode gerar impacto de macros via `Supplement`.
- Creatina pode ser registrada como check-in sem impacto de macros.

## Value objects iniciais

- `Macronutrients`
- `NutritionPer100g`
- `Portion`
- `BodyMetrics`
- `DailyTargets`
- `LocalizedName`
- `DateRange`
- `Money`

## Perguntas em aberto

- Qual fonte inicial da base nutricional propria?
- Como normalizar alimentos entre Brasil, Portugal e LATAM?
- Quais unidades e medidas caseiras entram no MVP?
- Qual retencao sera aceita para imagens temporarias?
- Quais disclaimers precisam aparecer no app e na API?
- Quais limites existem para usuarios em contexto GLP-1 friendly?
