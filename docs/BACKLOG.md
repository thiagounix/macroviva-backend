# Backlog

Backlog tecnico por etapas. Este documento orienta a sequencia de evolucao sem antecipar implementacoes.

## Etapa 1: Fundacao backend

- Validar solution e projetos.
- Garantir `net10.0`, nullable e referencias entre camadas.
- Adicionar pacotes essenciais.
- Remover artefatos de template.
- Criar documentacao inicial.
- Preparar SQL Server local via Docker Compose.
- Garantir `dotnet build`.
- Garantir `dotnet test`.

## Etapa 2: Estrutura de aplicacao

- Criar extensoes de DI por camada.
- Definir padrao de resultados/erros de aplicacao.
- Definir contratos de comandos/queries, sem escolher complexidade desnecessaria.
- Criar testes de arquitetura para dependencias entre projetos.

## Etapa 3: Persistencia inicial

- Criar `DbContext` inicial.
- Configurar connection string por ambiente.
- Definir estrategia de migrations.
- Adicionar health checks de banco.
- Criar primeira migration apenas quando houver modelo real.

## Etapa 4: Dominio inicial

- Modelar primeiros agregados pequenos.
- Implementar invariantes centrais.
- Criar testes unitarios de dominio.
- Evitar entidades completas antes de fechar linguagem ubiqua.

## Etapa 5: API inicial

- Criar endpoints REST de primeiro fluxo.
- Usar `async/await` e `CancellationToken`.
- Padronizar respostas e erros.
- Expandir OpenAPI.

## Etapa 6: IA mock

- Definir porta de aplicacao para analise de imagem.
- Implementar adapter mock na infraestrutura.
- Validar fluxo sem chamada externa real.

## Etapa 7: Seguranca

- Implementar autenticacao real.
- Definir autorizacao por usuario.
- Tratar consentimento, auditoria e retencao de dados sensiveis.
