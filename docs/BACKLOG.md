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

## Etapa 2: Dominio inicial

- Modelar primeiros agregados pequenos.
- Implementar invariantes centrais.
- Criar testes unitarios de dominio.
- Evitar entidades completas antes de fechar linguagem ubiqua.

## Etapa 2.1: Domain Hardening

- Ajustar ciclo de vida de `AIAnalysis`.
- Garantir que sugestoes da IA nao virem refeicao automaticamente.
- Separar check-ins e assinaturas como aggregates com ciclo proprio.
- Registrar decisoes para manter `User` fora de autenticacao real.

## Etapa 3: Application Layer

- Criar `Result<T>` e catalogo simples de erros.
- Criar portas de repositorio e servicos externos como abstracoes.
- Criar contratos DTO neutros, sem tipos de ASP.NET.
- Implementar casos de uso iniciais para catalogo de alimentos, refeicoes, analise de foto e suplementos.
- Converter `DomainException` para `Result.Failure`.
- Substituir teste placeholder por testes reais com fakes manuais.

## Etapa 4: Infrastructure e persistencia inicial

- Implementar repositorios com EF Core. Concluido.
- Criar `DbContext` inicial. Concluido.
- Configurar connection string por ambiente. Concluido.
- Definir estrategia de migrations. Preparado via design-time factory.
- Implementar adapters mock para storage e IA. Concluido.
- Preparar seed de desenvolvimento. Concluido.
- Criar primeira migration quando `dotnet-ef` estiver disponivel.
- Adicionar health checks de banco em etapa de composicao da API.

## Etapa 5: API inicial

- Criar endpoints REST de primeiro fluxo. Concluido.
- Usar `async/await` e `CancellationToken`. Concluido.
- Padronizar respostas HTTP a partir de `Result<T>`. Concluido.
- Adaptar upload HTTP para contratos neutros da Application. Concluido.
- Registrar `AddInfrastructure(...)` na composicao da API. Concluido.
- Expandir OpenAPI. Concluido.
- Testar persistencia real apos migration/schema.

## Etapa 5.1: Persistencia executavel para desenvolvimento

- Instalar/configurar `dotnet-ef`. Concluido.
- Criar migration inicial. Concluido.
- Aplicar schema no SQL Server local. Concluido.
- Executar seed de desenvolvimento. Concluido.
- Validar endpoints com persistencia real. Concluido.

## Etapa 6: Autenticacao e usuario corrente

- Implementar autenticacao real.
- Definir `CurrentUserService` real.
- Validar autorizacao por usuario.
- Manter senha, refresh token e providers externos fora do dominio.

## Etapa 7: IA mock e observabilidade

- Definir porta de aplicacao para analise de imagem.
- Implementar adapter mock na infraestrutura.
- Validar fluxo sem chamada externa real.
- Adicionar logs e metricas dos fluxos principais.

## Etapa 8: Seguranca e privacidade operacional

- Tratar consentimento, auditoria e retencao de dados sensiveis.
- Revisar LGPD, retencao de imagens e exclusao de conta.
