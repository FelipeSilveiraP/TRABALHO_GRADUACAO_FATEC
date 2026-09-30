# Especificação Técnica e Diagrama de Máquina de Estados

### Trabalho de Graduação (TG) — Análise e Desenvolvimento de Sistemas (FATEC)
**Aluno:** Felipe Silveira Pessoa  
**Projeto:** LucroImpresso — Sistema de Gestão de Custos, Controle de Estoque e Precificação para Manufatura Aditiva (Impressão 3D)  
**Stack Tecnológica:** C# .NET 9 | ASP.NET Core Blazor Server | Entity Framework Core | MariaDB  

---

## 1. Introdução e Fundamentação Teórica

O **Diagrama de Máquina de Estados** (na especificação OMG UML 2.5, *State Machine Diagram*) é um artefato comportamental essencial da Engenharia de Software voltado a modelar os estados discretos pelos quais um objeto passa durante seu ciclo de vida dentro do sistema, em resposta a eventos internos e externos, bem como as transições, condições de guarda e efeitos colaterais provocados.

No sistema **LucroImpresso**, o ciclo de vida do **Orçamento e da Ordem de Produção (`Orcamento`)** representa o núcleo dinâmico da aplicação, governando:
1. A elaboração matemática e científica dos custos (filamento por grama, insumos químicos por mL e energia elétrica por Watts/kWh);
2. A execução física da impressão 3D e controle da fila de trabalho na oficina maker;
3. O gatilho atômico de dedução física de estoque mediante desfecho com sucesso ou falha operacional de impressão ("espaguete") (`RN004`);
4. A proteção contra exclusões destrutivas (`RN005`);
5. A rotina programada de expurgo de dados inativos após 180 dias (`RN006`) e a salvaguarda perpétua no cofre financeiro acumulador (`RN007`).

Adicionalmente, outros subsistemas vitais contêm comportamentos de estado relevantes: o **Ciclo de Segurança e Expiração de Tokens de Recuperação de Senha** e o **Ciclo de Disponibilidade de Estoque de Insumos**.

---

## 2. Diagrama Principal: Máquina de Estados do Orçamento e Produção

### 2.1 Representação Visual em Mermaid

```mermaid
stateDiagram-v2
    [*] --> EmElaboracao : Usuário acessa formulário /orcamentos

    state EmElaboracao {
        [*] --> CarregandoParametros
        CarregandoParametros --> CalculandoCustos : Seleciona Peça/Máquina/Material/Acabamentos
        CalculandoCustos --> CalculandoCustos : Ajusta peso (g), tempos (h) ou margem (%)
    }

    EmElaboracao --> Pendente : SalvarOrcamento() [Formulário Válido] / Persistir no BD com Status = Pendente

    state Pendente {
        [*] --> DisponivelParaEdicao
        DisponivelParaEdicao --> DisponivelParaEdicao : EditarParametros() / Recalcular e Atualizar BD
    }

    Pendente --> EmImpressao : IniciarProducao() / Alocar Impressora e Mudar Status = EmImpressao
    Pendente --> ExclusaoFisica : Excluir() [Confirmado pelo Maker (RN005)] / Remover Registro do BD

    state EmImpressao {
        [*] --> FilaDeManufatura
        FilaDeManufatura --> EmExecucaoMecanica : Impressora 3D em Operação
    }

    EmImpressao --> Concluido : ConcluirProducao() [Confirmado pelo Maker (RN005)]\n/ Deduzir estoque físico: Material (g) e Acabamentos (mL) (RN004)\n/ Atualizar Status = Concluido
    
    EmImpressao --> Falha : RegistrarFalha() [Confirmado pelo Maker (RN005)]\n/ Deduzir material consumido como perda física (RN004)\n/ Atualizar Status = Falha ("Espaguete")

    state Concluido {
        [*] --> DisponivelParaEntrega
        DisponivelParaEntrega --> ContabilizadoNoDashboard : Totalizar Receita Líquida nos KPIs (RF029)
    }

    state Falha {
        [*] --> RegistradoComoPrejuizo
        RegistradoComoPrejuizo --> RegistradoComoPrejuizo : Exibir Alerta no Painel
    }

    Falha --> EmImpressao : ReiniciarImpressao() [Maker opta por reimprimir lote] / Resetar para nova tentativa

    Concluido --> CofreHistoricoFinanceiro : Excluir() [Confirmado pelo Maker (RN005)]\n/ Transferir faturamento e custos para HistoricoFinanceiro (RN007)\n/ Excluir registro transacional

    Falha --> CofreHistoricoFinanceiro : Excluir() [Confirmado pelo Maker (RN005)]\n/ Transferir despesas operacionais para HistoricoFinanceiro (RN007)\n/ Excluir registro transacional

    Concluido --> CofreHistoricoFinanceiro : TimerDiarioDatabaseCleanup()\n[DataCriacao < UtcNow - 180 dias (RN006)]\n/ Consolidar perpétuo no cofre contábil (RN007)\n/ Expurgo físico transacional

    Falha --> CofreHistoricoFinanceiro : TimerDiarioDatabaseCleanup()\n[DataCriacao < UtcNow - 180 dias (RN006)]\n/ Consolidar perpétuo no cofre contábil (RN007)\n/ Expurgo físico transacional

    state CofreHistoricoFinanceiro {
        [*] --> ConsolidadoPerpetuo
        ConsolidadoPerpetuo : Preservar Receitas e Custos em Historico_Financeiro
        ConsolidadoPerpetuo : Blindar KPIs históricos contra perda de dados
    }

    CofreHistoricoFinanceiro --> [*] : Encerramento do ciclo contábil
    ExclusaoFisica --> [*] : Registro descartado
```

---

### 2.2 Tabela Formal de Estados da Entidade `Orcamento`

| Identificador do Estado | Nome do Estado | Descrição Semântica e Contexto | Atividades Internas (*entry / do / exit*) |
| :--- | :--- | :--- | :--- |
| **ST01** | `EmElaboracao` | O usuário está na tela de orçamentos inserindo os parâmetros operacionais ou importando dados do fatiador. | **entry /** Carregar listas relacionais de clientes, peças, impressoras, materiais e insumos de acabamento.<br>**do /** Calcular reativamente custos parciais e preço sugerido.<br>**exit /** Validar integridade via `OrcamentoValidator`. |
| **ST02** | `Pendente` | Orçamento gravado no banco de dados com chave do locatário (`UsuarioId`). Aguarda decisão comercial ou alocação na oficina. | **entry /** Habilitar edição completa de campos e exclusão.<br>**do /** Permitir ajustes nos parâmetros de custo e margem. |
| **ST03** | `EmImpressao` | Pedido autorizado e enviado para execução na oficina maker. A impressora 3D selecionada está em processo de fabricação mecânica da peça. | **entry /** Travar alteração de parâmetros de custo para preservar histórico do lote.<br>**do /** Acompanhar tempo decorrido de máquina na fila de produção (`/producao`). |
| **ST04** | `Concluido` | A peça 3D foi impressa com perfeição e o acabamento manual/químico (lixamento, pintura, verniz) foi finalizado com sucesso. | **entry /** Efetuar baixa física atômica de estoque (`RN004`) e disponibilizar valores para apuração de lucro líquido no Dashboard.<br>**do /** Permitir entrega ao cliente e exclusão com preservação contábil. |
| **ST05** | `Falha` | Ocorreu sinistro irrecuperável durante a impressão 3D (descolamento da mesa aquecida, perda de passos, bico entupido ou formação de "espaguete"). | **entry /** Efetuar baixa física do filamento desperdiçado (`RN004`) e registrar custos elétricos da máquina perdida.<br>**do /** Permitir descarte com salvaguarda contábil ou reinicialização da impressão. |
| **ST06** | `CofreHistoricoFinanceiro` | Estado terminal contábil no qual os valores de faturamento e custos são salvaguardados na entidade `HistoricoFinanceiro` e o registro transacional é purgado. | **entry /** Incrementar `ReceitasAcumuladas` e `DespesasEnergiaAcumuladas` no cofre acumulador (`RN007`).<br>**exit /** Executar `DbSet.Remove()` e `SaveChangesAsync()`. |

---

### 2.3 Matriz de Transições de Estados de `Orcamento`

| Estado Origem | Evento / Gatilho (*Trigger*) | Condição de Guarda (*Guard*) | Ação / Efeito (*Action / Effect*) | Estado Destino | Regra de Negócio |
| :--- | :--- | :--- | :--- | :--- | :---: |
| `[*]` | Acesso a `/orcamentos` | Usuário autenticado | Inicializar instância vazia | `EmElaboracao` | `RN008` |
| `EmElaboracao` | `SalvarOrcamento()` | `[OrcamentoValidator.IsValid == true]` | Gravar no banco de dados com `Status = StatusOrcamento.Pendente`. | `Pendente` | `RN008` |
| `Pendente` | `EditarParametros()` | `[DadosValidos == true]` | Recalcular custo total e margem; atualizar registro via `OrcamentoRepository.UpdateAsync()`. | `Pendente` | `RN001`, `RN002`, `RN003` |
| `Pendente` | `IniciarProducao()` | Nenhuma restrição | Atualizar `Status = StatusOrcamento.EmImpressao`. Inserir na fila operacional da oficina. | `EmImpressao` | — |
| `Pendente` | `Excluir()` | `[ConfirmadoEmTela == true]` | Excluir registro do banco sem gerar receitas (não houve produção). | `[*]` (Excluído) | `RN005` |
| `EmImpressao` | `ConcluirProducao()` | `[ConfirmadoEmTela == true]` | **1.** `Material.SaldoGramas -= PesoGramas`<br>**2.** `Acabamento.SaldoMililitros -= QuantidadeMl`<br>**3.** `Status = StatusOrcamento.Concluido`<br>**4.** Gravar alterações atômicas no banco. | `Concluido` | `RN004`, `RN005` |
| `EmImpressao` | `RegistrarFalha()` | `[ConfirmadoEmTela == true]` | **1.** `Material.SaldoGramas -= PesoGramas`<br>**2.** `Acabamento.SaldoMililitros -= QuantidadeMl`<br>**3.** `Status = StatusOrcamento.Falha`<br>**4.** Registrar perda contábil. | `Falha` | `RN004`, `RN005` |
| `Falha` | `ReiniciarImpressao()` | Maker opta por reimprimir lote | Atualizar `Status = StatusOrcamento.EmImpressao`. Retornar para fila da impressora. | `EmImpressao` | — |
| `Concluido` | `ExcluirManual()` | `[ConfirmadoEmTela == true]` | **1.** `HistoricoFinanceiro.ReceitasAcumuladas += CustoFinal`<br>**2.** `HistoricoFinanceiro.DespesasEnergiaAcumuladas += CustoEnergia`<br>**3.** Remover da tabela `Orcamento`. | `CofreHistoricoFinanceiro` | `RN005`, `RN007` |
| `Falha` | `ExcluirManual()` | `[ConfirmadoEmTela == true]` | **1.** `HistoricoFinanceiro.DespesasEnergiaAcumuladas += CustoEnergia`<br>**2.** Remover da tabela `Orcamento`. | `CofreHistoricoFinanceiro` | `RN005`, `RN007` |
| `Concluido` | `TimerDiarioExpurgo()` | `[DataCriacao < UtcNow - 180 dias]` | Execução por `DatabaseCleanupService`. Transferir receita para `HistoricoFinanceiro` e expurgar. | `CofreHistoricoFinanceiro` | `RN006`, `RN007` |
| `Falha` | `TimerDiarioExpurgo()` | `[DataCriacao < UtcNow - 180 dias]` | Execução por `DatabaseCleanupService`. Transferir despesas de perda para `HistoricoFinanceiro` e expurgar. | `CofreHistoricoFinanceiro` | `RN006`, `RN007` |

---

## 3. Diagramas Complementares de Máquinas de Estado

### 3.1 Ciclo de Vida do Token de Recuperação de Senha (`Usuario`)

Este diagrama detalha o comportamento dos campos de segurança `TokenRedefinicaoSenha` e `TokenRedefinicaoExpiracao` mapeados na entidade `Usuario` (`RF034`, `RF035`, `RNF002`).

```mermaid
stateDiagram-v2
    [*] --> Inativo : Conta de usuário ativa e operante
    
    state Inativo {
        [*] --> SemToken
        SemToken : TokenRedefinicaoSenha = null
        SemToken : TokenRedefinicaoExpiracao = null
    }

    Inativo --> TokenGerado : SolicitarRecuperacao(email)\n[Email existe na base]\n/ Gerar GUID criptográfico seguro\n/ TokenRedefinicaoExpiracao = UtcNow + 2 Horas\n/ Disparar e-mail assíncrono via EmailService

    state TokenGerado {
        [*] --> AguardandoAcesso
        AguardandoAcesso : Token gravado no banco de dados
        AguardandoAcesso : Janela de segurança de 120 minutos ativa
    }

    TokenGerado --> SenhaRedefinida : SubmeterNovaSenha(token, novaSenha)\n[UtcNow <= TokenRedefinicaoExpiracao E SenhaAtendePoliticaForte]\n/ Gerar hash BCrypt com salt dinâmico\n/ Limpar token e timestamp de expiração

    TokenGerado --> Expirado : AcessarLink(token) ou SubmeterNovaSenha()\n[UtcNow > TokenRedefinicaoExpiracao]\n/ Bloquear redefinição e emitir alerta

    state Expirado {
        [*] --> TokenInvalido
        TokenInvalido : Token ultrapassou limite temporal de 2 horas
    }

    Expirado --> TokenGerado : SolicitarRecuperacao(email)\n/ Gerar novo token e renovar expiração para +2h

    state SenhaRedefinida {
        [*] --> CredenciaisAtualizadas
        CredenciaisAtualizadas : Nova senha persistida com BCrypt
    }

    SenhaRedefinida --> Inativo : Usuário autentica com a nova senha
```

---

### 3.2 Ciclo de Estados de Inventário e Nível de Estoque (`Material` / `Acabamento`)

Mapeia a transição dos insumos de produção (filamentos, resinas, primers, tintas e solventes) frente ao consumo fabril e reposição (`RF001` a `RF006`, `RN001`, `RN002`, `RN004`).

```mermaid
stateDiagram-v2
    [*] --> Disponivel : CadastrarInsumo(saldoInicial)\n[saldoInicial > MargemSeguranca]

    state Disponivel {
        [*] --> NivelNormal
        NivelNormal : Saldo suficiente para múltiplas impressões
        NivelNormal : Permite orçar e fabricar sem advertências
    }

    Disponivel --> Critico : BaixarEstoqueProducao(g ou mL)\n[Saldo <= MargemSeguranca E Saldo > 0]\n/ Exibir badge amarelo de alerta no Dashboard (RF004, RF031)

    state Critico {
        [*] --> NivelDeAlerta
        NivelDeAlerta : Saldo abaixo da margem de segurança configurada
        NivelDeAlerta : Notifica o empreendedor sobre iminência de ruptura
    }

    Critico --> Esgotado : BaixarEstoqueProducao(g ou mL)\n[Saldo == 0]\n/ Travar saldo em zero (não permite saldo negativo)

    state Esgotado {
        [*] --> SemInsumoFisico
        SemInsumoFisico : Saldo zerado no estoque
    }

    Esgotado --> Critico : RegistrarCompraLote()\n[NovoSaldo <= MargemSeguranca]
    Esgotado --> Disponivel : RegistrarCompraLote()\n[NovoSaldo > MargemSeguranca]\n/ Atualizar custo ponderado por grama ou por mL (RN001, RN002)
    Critico --> Disponivel : RegistrarCompraLote()\n[NovoSaldo > MargemSeguranca]\n/ Normalizar indicador visual no painel
```

---

## 4. Mapeamento e Rastreabilidade no Código-Fonte (.NET 9 / C#)

A tabela abaixo estabelece a ponte formal de rastreabilidade entre as transições de estados especificadas na modelagem UML e a respectiva implementação no código-fonte do sistema:

| Transição de Estado | Camada de Implementação | Arquivo de Código / Método | Regra Aplicada |
| :--- | :--- | :--- | :---: |
| `[*] -> EmElaboracao -> Pendente` | Visão Blazor / Repositório | `Components/Pages/Orcamentos/Orcamentos.razor`<br>`Repositories/OrcamentoRepository.cs` (`AddAsync`) | `RN008` |
| `Pendente -> EmImpressao` | Visão Blazor / Repositório | `Components/Pages/Producao/Producao.razor` (`MudarStatus`) | — |
| `EmImpressao -> Concluido` | Visão Blazor / Repositórios | `Components/Pages/Producao/Producao.razor` (`FinalizarProducao(true)`)<br>`MaterialRepository.cs`<br>`AcabamentoRepository.cs` | `RN004`, `RN005` |
| `EmImpressao -> Falha` | Visão Blazor / Repositórios | `Components/Pages/Producao/Producao.razor` (`FinalizarProducao(false)`) | `RN004`, `RN005` |
| `Concluido -> CofreHistorico` | Repositório | `Repositories/OrcamentoRepository.cs` (`DeleteAsync`) | `RN005`, `RN007` |
| `Falha -> CofreHistorico` | Repositório | `Repositories/OrcamentoRepository.cs` (`DeleteAsync`) | `RN005`, `RN007` |
| `Expurgo Diário (180 dias)` | Serviço de Segundo Plano / Repositório | `Services/DatabaseCleanupService.cs` (`DoCleanup`)<br>`Repositories/OrcamentoRepository.cs` (`DeleteAntigosAsync`) | `RN006`, `RN007` |
| `Token: Inativo -> Gerado -> Consumido` | Serviços / Repositório | `Services/EmailService.cs`<br>`Repositories/UsuarioRepository.cs`<br>`Components/Pages/Login/RedefinirSenha.razor` | `RF034`, `RF035`, `RNF002` |

---

## 5. Formalização Arquitetural: `OrcamentoStateMachine`

Para garantir desacoplamento, testabilidade unitária e aderência aos princípios do SOLID (*Single Responsibility Principle* e *Open/Closed Principle*), a lógica de validação de transições de estado, condições de guarda e efeitos colaterais de estoque está encapsulada no serviço de domínio `OrcamentoStateMachine.cs` (`src/LucroImpresso.Web/Services/OrcamentoStateMachine.cs`), permitindo que a camada Blazor e os serviços de background invoquem transições protegidas por regras de validação estritas.
