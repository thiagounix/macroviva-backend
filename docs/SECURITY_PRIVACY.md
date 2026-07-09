# Security and Privacy

Premissas iniciais de seguranca e privacidade para o MacroViva Backend.

## Dados sensiveis

O produto pode lidar com dados pessoais e dados relacionados a saude, habitos alimentares, objetivos corporais e uso de suplementos. Esses dados devem ser tratados como sensiveis.

## LGPD

Diretrizes iniciais:

- coletar apenas dados necessarios;
- documentar finalidade de uso;
- permitir exclusao e exportacao quando aplicavel;
- limitar acesso por usuario autenticado;
- evitar logs com dados sensiveis;
- definir retencao de imagens e dados derivados;
- preparar base para consentimento explicito em fluxos sensiveis.

## Imagens de refeicao

Imagens nao devem ser salvas no banco relacional. O fluxo futuro deve tratar imagens como entrada temporaria para analise. Qualquer armazenamento de arquivo exigira decisao explicita de retencao, criptografia e exclusao.

## Autenticacao e autorizacao

JWT Bearer esta preparado como dependencia tecnica. A autenticacao real ainda nao esta implementada.

Etapas futuras:

- definir provedor de identidade;
- validar issuer, audience, assinatura e expiracao;
- proteger endpoints por usuario;
- impedir acesso cruzado entre usuarios;
- registrar auditoria de operacoes sensiveis.

## IA e provedores externos

Nenhum cliente mobile deve acessar provedores de IA diretamente. Chaves de API ficam somente no backend. Prompts, imagens e respostas de IA devem passar por politicas de privacidade e retencao.

## Logs

Logs devem evitar:

- imagens;
- tokens;
- dados pessoais diretos;
- respostas completas de IA;
- detalhes alimentares identificaveis quando nao necessarios.
