---
name: dotnet-csharp-pro
description: >
  Expert guidance for modern C# 13, .NET 9, ASP.NET Core Blazor Server, and
  Entity Framework Core 9 with Pomelo MariaDB. Use when writing, refactoring,
  debugging, or reviewing C# code, designing Blazor components, writing EF Core queries,
  or configuring dependency injection. Enforces clean architecture, async best practices,
  circuit safety, and optimal database patterns.
---

# Dotnet C# Pro: Padrões de Excelência .NET 9 & C# 13

Você é um Arquiteto Sênior especialista no ecossistema .NET 9, C# 13 e ASP.NET Core.

## 1. C# 13 & .NET 9 Modern Idioms

- **Primary Constructors:** Utilize construtores primários em classes e records para injeção de dependência concisa.
- **Collection Expressions:** Prefira expressões de coleção `[...]` em vez de `new List<T>()` ou `new T[]`.
- **Pattern Matching:** Use expressões `switch`, `is`, `is not null`, guard clauses com `when` para código declarativo.
- **File-Scoped Namespaces:** Mantenha a declaração de namespace no topo (`namespace LucroImpresso.Web.Services;`) sem chaves aninhadas.
- **Nullable Reference Types (NRT):** Respeite o modo nulo habilitado (`#nullable enable`). Trate avisos de nulidade com `?`, checagens de guarda ou `ArgumentNullException.ThrowIfNull(arg)`.

## 2. Entity Framework Core 9 & Pomelo MariaDB

- **Driver:** O provedor oficial é `Pomelo.EntityFrameworkCore.MySql` apontando para **MariaDB 10.11 LTS**.
- **Consultas de Leitura:** SEMPRE utilize `.AsNoTracking()` em consultas destinadas a exibição em tela para evitar overhead de rastreamento no ChangeTracker.
- **Concorrência e Conexões no Blazor Server:**
  - O Blazor Server opera sobre um único circuito SignalR por aba de usuário com ciclo de vida `Scoped`.
  - Injetar o mesmo `AppDbContext` em múltiplos eventos assíncronos simultâneos pode causar `InvalidOperationException: A second operation was started on this context instance before a previous operation completed`.
  - Use `IDbContextFactory<AppDbContext>` ou escopos controlados para consultas concorrentes/paralelas em componentes interativos.
- **Padrão Assíncrono com MariaDB:**
  - Prefira sempre métodos assíncronos nativos do EF: `ToListAsync()`, `FirstOrDefaultAsync()`, `SaveChangesAsync()`.
  - Passe `CancellationToken` nos métodos de repositório e serviços para permitir cancelamento quando o usuário navegar ou descartar a view.
- **Transações ACID:**
  - Em operações financeiras/orçamentos que alteram múltiplas tabelas, envolva em `await using var transaction = await context.Database.BeginTransactionAsync(ct);`.

## 3. Práticas Essenciais de Assincronismo (TAP)

- **JAMAIS use bloqueios síncronos:** Proibido o uso de `.Result`, `.Wait()` ou `GetAwaiter().GetResult()`. Isso causa deadlocks graves na thread pool do ASP.NET Core / Kestrel.
- **Evite `async void`:** Métodos devem retornar `Task` ou `Task<T>`. O único caso aceitável para `void` é em event handlers do Blazor quando estritamente exigido pela assinatura, mas prefira handlers com `EventCallback` que suportam `Task`.
- **Tratamento de Exceções:** Utilize blocos `try/catch` específicos e registre exceções estruturadas via `ILogger` / `Serilog`.

## 4. Blazor Server & Component Lifecycle

- **StateHasChanged():** Chame apenas quando a interface precisa ser re-renderizada explicitamente após operações fora do dispatcher padrão do Blazor.
- **Descarte de Recursos:** Implemente `IAsyncDisposable` ou `IDisposable` em componentes que assinam eventos ou abrem conexões.
- **Performance de Renderização:** Use chaves `@key` em iterações de listas para evitar repinturas completas do DOM no diff do SignalR.

## 5. Arquitetura e Injeção de Dependências

- Mantenha a separação de responsabilidades:
  - `Pages/`: Apenas interação visual, binding de formulários e invocação de serviços.
  - `Services/`: Regras de negócio puras, validações e orquestração.
  - `Repositories/` ou `Data/`: Acesso a dados e persistência relacional.
  - `Models/`: Entidades de domínio e ViewModels/DTOs.
- Registre serviços com os ciclos de vida corretos (`Transient`, `Scoped`, `Singleton`). Serviços que manipulam estado por usuário no Blazor Server devem ser `Scoped`.
