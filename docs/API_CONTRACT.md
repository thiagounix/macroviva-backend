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

### Meal Confirmation

```http
POST /api/meals
```

Uso futuro: salvar uma refeicao confirmada ou ajustada pelo usuario.

### Food Catalog

```http
GET /api/foods
GET /api/foods/{id}
```

Uso futuro: consultar base nutricional propria.

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
