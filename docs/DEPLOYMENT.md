# Deployment

Este documento descreve a preparacao do backend MacroViva para um ambiente
staging/beta. O objetivo e disponibilizar uma URL HTTPS publica para que builds
Android, iOS e Web nao dependam de `localhost` ou `10.0.2.2`.

## Ambiente recomendado

Para o MVP beta, a opcao recomendada e:

- Azure App Service para `MacroViva.Api`.
- Azure SQL Database para persistencia.
- Variaveis de ambiente no App Service para configuracao sensivel.
- Ambiente `Staging`.

## Principios

- Nao publicar segredos no repositorio.
- Nao usar banco local em beta.
- Nao apontar app de testers externos para `localhost`.
- Usar HTTPS.
- Rodar migrations antes de liberar testers.
- Manter seed controlado por variavel de ambiente.

## Variaveis de ambiente do App Service

Obrigatorias:

```text
ASPNETCORE_ENVIRONMENT=Staging
ConnectionStrings__DefaultConnection=<connection-string-do-azure-sql>
```

Recomendadas:

```text
OpenApi__Enabled=true
Seed__RunOnStartup=false
Cors__AllowedOrigins__0=https://<dominio-web-staging>
```

Para uma primeira carga controlada do catalogo no staging, e possivel ligar
temporariamente:

```text
Seed__RunOnStartup=true
```

Depois de validar os dados, voltar para:

```text
Seed__RunOnStartup=false
```

## Connection string Azure SQL

Formato esperado:

```text
Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<database>;Persist Security Info=False;User ID=<user>;Password=<password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

## Publicar API localmente para artefato

```powershell
dotnet restore .\MacroViva.sln
dotnet build .\MacroViva.sln
dotnet test .\MacroViva.sln --no-build
dotnet publish .\src\MacroViva.Api\MacroViva.Api.csproj -c Release -o .\artifacts\publish\api
```

## Aplicar migrations no staging

Antes de executar, defina a connection string do staging na sessao:

```powershell
$env:ConnectionStrings__DefaultConnection="<connection-string-do-azure-sql>"
dotnet ef database update --project .\src\MacroViva.Infrastructure --startup-project .\src\MacroViva.Api
```

## Smoke test da API staging

Substitua a URL pela URL real do App Service:

```powershell
$baseUrl = "https://<app-service>.azurewebsites.net"
Invoke-RestMethod "$baseUrl/health"
Invoke-RestMethod "$baseUrl/api/foods"
Invoke-RestMethod "$baseUrl/api/supplements"
Invoke-RestMethod "$baseUrl/api/meals/today"
```

Validacoes esperadas:

- `/health` retorna `status = ok`.
- `/api/foods` retorna catalogo com porcoes.
- `/api/supplements` retorna `description`, `safetyNote`,
  `requiresProfessionalGuidance` e `hasStimulantWarning`.
- Swagger fica disponivel em Staging apenas se `OpenApi__Enabled=true`.

## Build mobile beta apontando para staging

Android AAB para Google Play:

```powershell
flutter build appbundle --release --dart-define=API_BASE_URL=https://<api-staging>
```

APK interno:

```powershell
flutter build apk --release --dart-define=API_BASE_URL=https://<api-staging>
```

Web:

```powershell
flutter build web --dart-define=API_BASE_URL=https://<api-staging>
```

iOS/TestFlight deve usar a mesma URL:

```powershell
flutter build ipa --release --dart-define=API_BASE_URL=https://<api-staging>
```

## Checklist antes de beta

- API staging publicada com HTTPS.
- Azure SQL staging criado e migrado.
- Seed inicial validado.
- CORS configurado para dominio Web staging, se houver Web publicada.
- App mobile gerado com `API_BASE_URL` staging.
- Fluxos testados: dashboard, alimentos, refeicao manual, suplementos,
  check-in, foto, analise mock e confirmacao.
- IA real ainda desativada e comunicada como mock/simulada.
- Autenticacao real ainda fora do escopo do MVP beta local.

## Limitacao atual

O backend ainda usa `DevelopmentCurrentUserService` e `MockMealVisionAnalyzer`.
Isso e aceitavel para beta fechado controlado, mas nao deve ser tratado como
producao publica com usuarios reais, autenticacao real ou dados sensiveis sem
as proximas etapas de seguranca.
