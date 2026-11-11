# AGENTS.md — Single Source of Truth: LucroImpresso

Este documento é a diretriz absoluta e mandatória para todos os agentes de IA (Gemini, Claude, ChatGPT, Codex) que operam neste repositório. Nenhuma decisão arquitetural deve contrariar os fatos listados aqui.

---

## 1. Stack Tecnológica Real (Ground Truth)

- **Linguagem:** C# 13
- **Framework:** .NET 9 (ASP.NET Core Blazor Server)
- **Acesso a Dados (ORM):** Entity Framework Core 9 (provedor `Pomelo.EntityFrameworkCore.MySql`)
- **Banco de Dados Oficial:** **MariaDB 10.11 LTS**
  - **PROIBIDO:** Supor, sugerir ou configurar PostgreSQL, SQL Server ou SQLite, a menos que expressamente solicitado pelo usuário.
  - **Ambiente Real:** MariaDB rodando nativamente no Debian Linux na porta `3308`.
  - **Client de Inspeção:** Beekeeper Studio.
  - **Database Name:** `LucroImpressoDB`
  - **Connection String:** Configurada em `src/LucroImpresso.Web/appsettings.json` apontando para `Server=localhost;Port=3308;Database=LucroImpressoDB`.
- **Estilo Visual / UI:** Neo-Brutalismo (alto contraste, bordas sólidas pretas, sombras duras `box-shadow: 4px 4px 0px #000`, Vanilla CSS refinado e Bootstrap 5).

---

## 2. Infraestrutura e Execução Local

- O usuário **JÁ POSSUI** o MariaDB rodando no Debian localmente.
- **NÃO force nem exija Docker para desenvolvimento local**. O sistema pode ser executado diretamente via:
  ```powershell
  dotnet run --project src/LucroImpresso.Web
  ```
- O arquivo `docker-compose.yml` existe no projeto exclusivamente como opção para deployment ou empacotamento em servidores de produção externos, não sendo pré-requisito para o trabalho do usuário.

---

## 3. Domínio do Negócio (Impressão 3D Maker)

O sistema **LucroImpresso** é um sistema web (TG FATEC) de gestão de custos, estoque e orçamentação para manufatura aditiva (impressão 3D).
**O escopo abaixo é o escopo implementado e validado pelo orientador. NÃO adicione novas variáveis de precificação sem pedido expresso do usuário.**

A precificação (`Models/Orcamento.cs` → `CustoFinal`) é a soma de:
1. **Material:** `Peso da peça (g) × Preço por grama` — RN001: `Preço por grama = Valor pago / Peso do rolo (g)`.
2. **Energia da Máquina:** `(Potência W / 1000) × Horas de impressão × Tarifa kWh` — RN003. (Não há depreciação da máquina no escopo.)
3. **Mão de Obra:** `(Horas de impressão + Horas de pintura/lixamento) × Valor da hora`. A mão de obra é tratada como o lucro do maker (`MargemLucroPercentual`).
4. **Acabamento:** `Quantidade (mL) × Preço por mL` — RN002: `Preço por mL = Valor pago / (Litros × 1000)`. Tipos: tinta, primer, verniz, solvente etc.

Fora do escopo atual: taxa de perda/falha, depreciação, markup separado, taxas de cartão/PIX e impostos.

**Ciclo de vida do Orçamento:** governado EXCLUSIVAMENTE por `Services/OrcamentoStateMachine.cs`
(`Pendente → EmImpressao → Concluido | Falha`, e `Falha → EmImpressao` para reimpressão).
A baixa de estoque (material em g, acabamentos em mL) acontece ao concluir ou registrar falha, sempre
via `IOrcamentoRepository.FinalizarProducaoAsync` (uma única transação). Telas não alteram estoque diretamente.

---

## 4. Segurança e Arquitetura de Software

- **Multi-Tenancy:** Isolamento estrito por usuário — a coluna `fk_usuario` (`UsuarioId`) em todas as entidades. Não existe `TenantId`. Toda consulta, edição e exclusão nos repositórios DEVE filtrar por `UsuarioId` do usuário logado (`UsuarioContextService`).
- **Senhas:** Criptografia exclusiva com `BCrypt.Net`. NUNCA use MD5 ou SHA1. NUNCA armazene senhas administrativas ou credenciais hardcoded em arquivos de código (a senha do admin padrão vem só de `AdminSettings:SenhaPadrao`). Tokens de redefinição de senha são salvos apenas como hash SHA-256.
- **Banco de Dados:** Alterações de schema SOMENTE via migrations do EF Core (`dotnet ef migrations add`), aplicadas automaticamente na inicialização. Não escreva SQL de alteração de schema no `Program.cs`.
- **Logging:** Estruturado via `Serilog`.
- **Segurança Web:** Headers de segurança configurados (CSP, Anti-Clickjacking, XSS Protection, HSTS em Produção).

---

## 5. Diretrizes de Código C# / .NET 9

- Utilize recursos modernos do C# 13 / .NET 9: Primary Constructors, Collection Expressions `[...]`, Pattern Matching, File-scoped Namespaces.
- Operações de leitura no EF Core devem usar `.AsNoTracking()` para alta performance.
- Respeite o padrão assíncrono (TAP): Métodos assíncronos devem receber `CancellationToken` quando aplicável. NUNCA utilize `.Result` ou `.Wait()`.
- Antes de afirmar qualquer coisa sobre a infraestrutura ou configurações do projeto, **LEIA** os arquivos reais em `src/LucroImpresso.Web/`.
