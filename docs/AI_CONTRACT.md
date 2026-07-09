# AI Contract

Contrato futuro para analise de imagem de refeicao. Nao ha integracao real com IA nesta etapa.

## Objetivo

Receber uma imagem de refeicao e retornar candidatos provaveis de alimentos com estimativa de porcao. O backend usara esses candidatos para calcular macronutrientes com base nutricional propria.

## Regras

- IA nao calcula macros finais.
- IA nao prescreve dieta.
- IA nao recomenda suplemento ou medicamento.
- IA nao ajusta dose de medicamento.
- Usuario sempre confirma ou corrige antes de salvar.
- Imagem nao deve ser salva no banco.
- Flutter nunca chama provedor de IA diretamente.

## Request futuro

```json
{
  "imageContentType": "image/jpeg",
  "imageBytes": "<binary-or-upload-reference>",
  "locale": "pt-BR",
  "hints": {
    "mealType": "lunch",
    "userNotes": "optional"
  }
}
```

## Response futuro

```json
{
  "analysisId": "temporary-id",
  "items": [
    {
      "name": "rice",
      "displayName": "Arroz",
      "estimatedPortion": {
        "quantity": 120,
        "unit": "g"
      },
      "confidence": 0.78,
      "alternatives": [
        {
          "displayName": "Arroz integral",
          "confidence": 0.42
        }
      ]
    }
  ],
  "warnings": [
    "Portion estimate requires user confirmation."
  ]
}
```

## Adapter esperado

A camada `Application` deve depender de uma porta, por exemplo `IMealImageAnalysisProvider`. A camada `Infrastructure` deve implementar essa porta.

Primeira implementacao planejada: mock deterministico para desenvolvimento e testes.

Implementacao real com OpenAI ou outro provedor fica para etapa posterior.
