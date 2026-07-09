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

## Bounded contexts candidatos

### Nutrition

Responsavel por alimentos, nutrientes, medidas, porcoes e calculo de macros.

### Meal Tracking

Responsavel por refeicoes registradas, confirmacao do usuario e historico alimentar.

### AI Analysis

Responsavel por transformar imagem em candidatos de alimentos e porcoes estimadas. Nao e fonte da verdade nutricional.

### User Profile

Responsavel por preferencias, metas e informacoes do usuario necessarias para personalizacao segura.

### Identity

Responsavel por autenticacao, autorizacao e isolamento de dados por usuario.

## Linguagem ubiqua inicial

- Meal: refeicao registrada ou em processo de confirmacao.
- Meal analysis: resultado temporario da analise de imagem.
- Food candidate: alimento sugerido pela IA.
- Portion estimate: estimativa de porcao que exige confirmacao.
- Confirmed meal: refeicao validada pelo usuario.
- Nutrition facts: informacoes nutricionais por unidade ou porcao.
- Macro calculation: calculo de calorias, proteinas, carboidratos e gorduras.
- User goal: meta definida pelo usuario, sem prescricao profissional.

## Perguntas em aberto

- Qual fonte inicial da base nutricional propria?
- Como normalizar alimentos entre Brasil, Portugal e LATAM?
- Quais unidades e medidas caseiras entram no MVP?
- Qual retencao sera aceita para imagens temporarias?
- Quais disclaimers precisam aparecer no app e na API?
- Quais limites existem para usuarios em contexto GLP-1 friendly?
