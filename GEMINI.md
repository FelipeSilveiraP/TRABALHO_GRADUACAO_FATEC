# GEMINI.md — Diretrizes Específicas para o Modelo Gemini no LucroImpresso

Este documento orienta o agente Gemini durante o desenvolvimento neste repositório.

## ⚠️ Anti-Alucinação Mandatória

1. **BANCO DE DADOS: MARIADB 10.11 LTS**
   - O projeto utiliza **MariaDB 10.11** com o driver `Pomelo.EntityFrameworkCore.MySql`.
   - O banco roda no **Debian local** na porta `3308`. O usuário gerencia o banco pelo **Beekeeper Studio**.
   - **JAMAIS** mencione, configure ou assuma PostgreSQL, SQLite ou SQL Server.

2. **DOCKER É OPCIONAL**
   - Não exija ou assuma `docker-compose up` para desenvolvimento diário.
   - O ambiente do desenvolvedor já possui o SGBD MariaDB rodando nativamente. Para rodar a aplicação, use `dotnet run --project src/LucroImpresso.Web`.

3. **VERIFICAÇÃO PRIMEIRO, AFIRMAÇÃO DEPOIS**
   - Antes de sugerir correções de configuração ou afirmar dependências, execute buscas no código (`grep_search` ou `view_file` em `appsettings.json`, `Program.cs`, `.csproj`).

4. **SKILLS E PADRÕES**
   - Utilize a skill `dotnet-csharp-pro` para padrões de C# 13, .NET 9, Blazor Server e Entity Framework Core.
   - Utilize a skill `lucroimpresso-expert` para regras de negócio de manufatura aditiva (cálculos de filamento, resina, custos de máquina e impostos).
   - Respeite o modo `ponytail` para evitar código inflado, mantendo a simplicidade e evitando dependências desnecessárias.
