# MacroViva Backend — Project Context

MacroViva é um app mobile fitness/nutricional para Brasil, Portugal e LATAM.

O usuário tira foto da refeição, a IA identifica alimentos prováveis e estima porção. O backend cruza esses alimentos com uma base nutricional própria e calcula calorias, proteínas, carboidratos e gorduras. O usuário sempre confirma ou ajusta antes de salvar.

Foco inicial:
- academia;
- proteína diária;
- emagrecimento;
- controle de calorias e macronutrientes;
- suplementação básica, como whey e creatina;
- futuro modo baixa fome / GLP-1 friendly.

O produto NÃO deve:
- prescrever dieta;
- prescrever suplemento;
- prescrever medicamento;
- ajustar dose de medicamento;
- prometer precisão perfeita;
- substituir médico ou nutricionista.

Stack backend:
- .NET 10
- C#
- ASP.NET Core Web API
- EF Core 10
- SQL Server

- DDD
- Clean Architecture
- REST API
- xUnit
- OpenAPI/Swagger
- JWT Bearer preparado para autenticação futura
- IA por adapter, começando com mock

Regras arquiteturais:
- Domain não referencia ninguém.
- Application referencia Domain.
- Infrastructure referencia Application e Domain.
- Api referencia Application e Infrastructure.
- Worker referencia Application e Infrastructure.
- IA sempre fica atrás do backend.
- Flutter nunca chama OpenAI diretamente.
- IA identifica alimentos e estima porção.
- Backend calcula macros.
- Usuário confirma antes de salvar.
- Imagem não deve ser salva no banco.
- Código em inglês.
- Documentação pode ser em português.