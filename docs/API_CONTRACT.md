# API Contract

Contratos planejados para a REST API. Nenhum endpoint de negocio esta implementado nesta etapa.

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

Uso futuro: verificar disponibilidade basica da API e dependencias criticas.

### Meal Image Analysis

```http
POST /api/meal-analyses
```

Uso futuro: receber uma imagem de refeicao, solicitar analise por adapter de IA e retornar candidatos de alimentos e porcoes estimadas.

Importante: a imagem nao deve ser salva no banco.

Contrato planejado da Application:

- Entrada: `content` como stream, `fileName` e `contentType`.
- Saida: `analysisId`, status da analise e lista de itens detectados.
- Cada item pode conter nome sugerido, gramas estimadas, confianca e `suggestedFoodId`.
- `suggestedFoodId` e apenas sugestao; nao confirma alimento final.

### Meal Confirmation

```http
POST /api/meal-analyses/{analysisId}/confirm
```

Uso futuro: confirmar uma analise concluida e criar uma refeicao a partir dos alimentos escolhidos pelo usuario.

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

### Manual Meals

```http
POST /api/meals
GET /api/meals/today
```

Uso futuro: registrar refeicao manual e consultar refeicoes do dia do usuario corrente.

### Food Catalog

```http
GET /api/foods?search=rice
GET /api/foods/{id}
```

Uso futuro: consultar base nutricional propria.

### Supplements

```http
GET /api/supplements
POST /api/me/supplements/check-ins
```

Uso futuro: listar suplementos cadastrados e registrar check-ins de suplementos do usuario. Creatina pode nao impactar macros; whey pode impactar macros quando modelado como alimento/suplemento com macronutrientes.

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

## Observacoes de produto

O backend nao deve prescrever dieta, suplemento ou medicamento. O usuario sempre confirma ou ajusta dados antes de salvar.
