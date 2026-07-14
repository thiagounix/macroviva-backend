# Isolamento Anonimo de Testers

Este mecanismo existe somente para o beta privado. Ele separa refeicoes e
check-ins entre instalacoes sem criar cadastro, senha ou autenticacao real.

## Funcionamento

O app gera um UUID v4 anonimo por instalacao e o envia no header
`X-MacroViva-Tester-Id`. Em Staging, a API valida o formato UUID canonico e usa
HMAC-SHA256 com `BetaTesterIdentity__HashingKey` para derivar um `User.Id`
interno deterministico. O UUID bruto nao e persistido nem registrado em logs.

Quando o identificador e valido, a API cria ou reutiliza um usuario tecnico com
e-mail no dominio reservado `anonymous.macroviva.invalid`. Nenhum nome, e-mail,
telefone ou outro dado pessoal e solicitado por este fluxo.

## Ambientes e endpoints

- Development mantem `DevelopmentCurrentUserService` para o fluxo local atual.
- Staging exige o header apenas nos endpoints com dados do tester.
- Production e outros ambientes nunca recebem o usuario fixo de Development.
- `/health`, Swagger/OpenAPI, `/api/foods`, `/api/foods/{id}` e
  `/api/supplements` funcionam sem header.
- `OPTIONS` de CORS nao exige identidade.
- Header ausente em endpoint dependente ou header invalido retorna `400` com
  uma mensagem generica, sem stack trace.

`Cors:AllowedOrigins` deve incluir apenas as origens Web de staging. A politica
de Staging permite o header customizado, sem liberar qualquer origem.

## Configuracao

Em Staging, configure uma chave privada com pelo menos 32 caracteres:

```text
BetaTesterIdentity__HashingKey=<chave-privada>
```

Nao versionar esse valor. A troca da chave muda o `User.Id` derivado e, por
consequencia, desvincula o dispositivo dos registros beta anteriores.

## Limites e cuidados

O header e uma medida de isolamento temporario, nao uma credencial. Quem obtiver
o UUID de outra instalacao pode tentar se passar por ela. Nao ha recuperacao de
conta, revogacao ou migracao de dados. Reinstalar o app ou limpar seus dados
locais cria uma identidade nova e perde o vinculo anterior.

Use somente em beta privado com dados ficticios ou de baixo risco. Antes de uma
publicacao aberta, este fluxo deve ser substituido por autenticacao real e pelos
controles de seguranca, privacidade e suporte correspondentes.
