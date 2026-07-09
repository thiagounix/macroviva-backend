# API Contract

Contratos da REST API. Os endpoints iniciais chamam casos de uso da camada `Application` e nao contem regra de negocio.

## Principios

- API nao contem regra de negocio.
- Controllers chamam casos de uso da camada `Application`.
- Endpoints futuros devem ser assincronos.
- Endpoints futuros devem receber `CancellationToken`.
- Respostas devem ser previsiveis e documentadas em OpenAPI.
- Erros devem usar um formato consistente.

## Endpoints planejados

### Health

```http
GET /health
```

Verifica disponibilidade basica da API.

### Meal Image Analysis

```http
POST /api/ai/meal-photo/analyze
Content-Type: multipart/form-data
```

Recebe uma imagem de refeicao no campo `file`, solicita analise por adapter mock de IA e retorna candidatos de alimentos e porcoes estimadas.

Importante: a imagem nao deve ser salva no banco.

Contrato planejado da Application:

- Entrada: `content` como stream, `fileName` e `contentType`.
- Saida: `analysisId`, status da analise e lista de itens detectados.
- Cada item pode conter nome sugerido, gramas estimadas, confianca e `suggestedFoodId`.
- `suggestedFoodId` e apenas sugestao; nao confirma alimento final.

### Meal Confirmation

```http
POST /api/ai/meal-photo/{analysisId}/confirm
```

Confirma uma analise concluida e cria uma refeicao a partir dos alimentos escolhidos pelo usuario.

Contrato planejado:

```json
{
  "mealType": "Lunch",
  "occurredAt": "2026-01-01T12:00:00Z",
  "items": [
    {
      "selectedFoodId": "00000000-0000-0000-0000-000000000000",
      "grams": 150
    }
  ]
}
```

`selectedFoodId` e o alimento final confirmado. Sugestoes da IA nunca devem ser usadas como confirmacao implicita.

Em desenvolvimento, use um ID real retornado por `GET /api/foods`. IDs de exemplo documentais nao devem ser usados como dado final confirmado.

### Manual Meals

```http
POST /api/meals
GET /api/meals/today
```

Registra refeicao manual e consulta refeicoes do dia do usuario corrente.

Exemplo de request:

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

O `foodId` deve vir de `GET /api/foods`.

### Food Catalog

```http
GET /api/foods?search=rice
GET /api/foods/{id}
```

Consulta base nutricional propria.

### Supplements

```http
GET /api/supplements
POST /api/user-supplements/check-in
```

Lista suplementos cadastrados e registra check-ins de suplementos do usuario. Creatina pode nao impactar macros; whey pode impactar macros quando modelado como alimento/suplemento com macronutrientes.

### User Goals

```http
GET /api/me/goals
PUT /api/me/goals
```

Uso futuro: consultar e ajustar metas do usuario sem prescricao medica ou nutricional.

### Authentication

```http
POST /api/auth/login
POST /api/auth/refresh
```

Uso futuro: autenticacao. Nao implementado nesta etapa.

## Erros

Controllers convertem `Result<T>` da Application para HTTP:

- `Validation`: 400.
- `NotFound`: 404.
- `Unauthorized`: 401.
- `Conflict`: 409.
- `Failure`: 500.

## Observacoes de produto

O backend nao deve prescrever dieta, suplemento ou medicamento. O usuario sempre confirma ou ajusta dados antes de salvar.

## Smoke test local

Com SQL Server local, migration aplicada e API rodando:

```http
GET /health
GET /swagger/v1/swagger.json
GET /api/foods
GET /api/foods/{id}
GET /api/supplements
POST /api/meals
GET /api/meals/today
POST /api/user-supplements/check-in
POST /api/ai/meal-photo/analyze
POST /api/ai/meal-photo/{analysisId}/confirm
```

`GET /api/foods` fornece os IDs reais para refeicoes e confirmacao de analise. `GET /api/supplements` fornece os IDs reais para check-in.
