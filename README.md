# LucroImpresso

### Trabalho de Graduação (TG) - Análise e Desenvolvimento de Sistemas (Fatec)
**Aluno:** Felipe Silveira Pessoa  
**Orientador:** [Nome do Orientador]  

---

## 📝 Descrição do Projeto
O **LucroImpresso** é um sistema web especializado em gestão de custos, controle de estoque e precificação precisa para o setor de manufatura aditiva (Impressão 3D). O objetivo principal é retirar pequenos empreendedores e *makers* do "chutômetro" financeiro, fornecendo uma ferramenta científica para cálculo de custos reais de produção (insumos, energia elétrica e mão de obra).

---

## 🛠️ Stack Técnica
* **Ambiente de Execução:** .NET 9
* **Framework Web:** ASP.NET Core Blazor (Componentização Reativa)
* **Persistência de Dados:** Entity Framework Core (Code-First)
* **Banco de Dados:** MariaDB
* **Estilização & UI:** Bootstrap 5 / CSS3 Vanilla

---

## 📂 Estrutura do Repositório
A organização do repositório segue os padrões de governança de TI e rigor acadêmico:
* **`/docs`**: Contém todos os artefatos de engenharia de software e modelagem de negócios aprovados (BPMN, Caso de Uso, Elicitação de Requisitos, EAP, TAP, Matriz SWOT e 5W2H).
* **`/src`**: Código-fonte da aplicação desenvolvido em C#.
  * `LucroImpresso.Web/`: Projeto principal Blazor contendo as Camadas de Models, Interfaces, Repositories e Services.

---

## 💼 Proposta Comercial e Orçamentária (APF / IFPUG)
Proposta comercial formal de desenvolvimento de software em regime de Preço Fixo (*Turnkey*), baseada em **Análise de Pontos de Função (ISO/IEC 20926)** conforme métricas acadêmicas homologadas pela FATEC:
* **[Documento Detalhado da Proposta Comercial](docs/PROPOSTA%20COMERCIAL/Documento_Proposta_Comercial.md)**: Justificativa econômica, escopo dos 9 módulos, matriz métrica de APF (278,1 FP ajustados / 5.562 LOC), cronograma de marcos de entrega e SLA de 90 dias.
* **[Proposta Comercial em PDF para Impressão](docs/PROPOSTA%20COMERCIAL/PROPOSTA%20COMERCIAL.pdf)**: Versão formatada em alta fidelidade com campos de assinatura formal do proponente e contratante.
* **[Layout Visual em HTML5](docs/PROPOSTA%20COMERCIAL/Documento_Proposta_Comercial.html)**: Visualização responsiva com cards de resumo financeiro (R$ 4.449,60 / R$ 20,00/h / 222,48h / R$ 0,00 de licenças).

---

## 📊 Estrutura Analítica do Projeto (EAP / WBS)
O escopo total do **LucroImpresso** foi decomposto hierarquicamente em 6 fases de nível 2 e 36 pacotes de trabalho de nível 3, abrangendo desde o planejamento estratégico maker até a suíte de testes automatizados e encerramento acadêmico:
* **[Documento Detalhado da EAP (com WBS Dictionary e Mermaid)](docs/EAP/EAP_Documento_Detalhado.md)**
* **[Diagrama Gráfico da EAP em Alta Resolução (PNG)](docs/EAP/EAP.png)**
* **[Diagrama da EAP Vetorial para Impressão (PDF)](docs/EAP/EAP.pdf)**

---

## 🔄 Diagramas de Máquina de Estados (UML 2.5 Statechart)
Modelagem formal do comportamento dinâmico do sistema, ciclos de vida de entidades, condições de guarda e efeitos colaterais atômicos de estoque (`RN001` a `RN008`):
* **[Documento Técnico Completo de Máquinas de Estado](docs/DIAGRAMA%20DE%20MAQUINA%20DE%20ESTADO/Documento_Diagrama_Maquina_de_Estados.md)**: Matrizes de transição, tabelas formais de estados e diagramas interativos Mermaid.
* **[Diagrama Visual em Alta Resolução (PNG)](docs/DIAGRAMA%20DE%20MAQUINA%20DE%20ESTADO/diagrama_maquina_estados_orcamento.png)**: Visualização gráfica em 2560x1440 px cobrindo o ciclo principal do Orçamento/Produção e ciclos complementares de Token e Estoque.
* **[Diagrama Vetorial em PDF para Impressão](docs/DIAGRAMA%20DE%20MAQUINA%20DE%20ESTADO/DIAGRAMA_DE_MAQUINA_DE_ESTADOS.pdf)**: Versão formatada em alta fidelidade para inclusão na monografia do TG.
* **[Serviço de Domínio C#](src/LucroImpresso.Web/Services/OrcamentoStateMachine.cs)**: Implementação formal em C# .NET 9 com 100% de cobertura de testes unitários.

---

## 🌐 Portabilidade e Compatibilidade do Sistema (ISO/IEC 25010)
Especificação formal de portabilidade segundo a norma internacional **ISO/IEC 25010 (SQuaRE)**, cobrindo adaptabilidade multiplataforma, facilidade de instalação, substituição de infraestrutura e coexistência no ecossistema maker:
* **[Documento Formal de Portabilidade do Sistema](docs/PORTABILIDADE/Documento_de_Portabilidade_do_Sistema.md)**: Matrizes completas de SOs (Windows, Linux, macOS), navegadores (Chrome, Edge, Firefox, Safari), viewports responsivos, diretrizes de caminhos e fusos horários (`DateTimeKind.Utc`).
* **[Documento de Portabilidade em PDF para Impressão](docs/PORTABILIDADE/Documento_de_Portabilidade_do_Sistema.pdf)**: Versão formatada em alta fidelidade com tipografia técnica para monografia acadêmica da FATEC.
* **[Multi-stage Dockerfile](Dockerfile)**: Construção conteinerizada otimizada (.NET 9 SDK + ASP.NET 9 Runtime) para orquestração portátil em qualquer nuvem ou servidor local.
* **[Docker Compose Multi-serviço](docker-compose.yml)**: Subida declarativa de ambiente completo de produção contendo a aplicação Blazor Server e banco de dados MariaDB 10.11 com saúde automatizada (`healthcheck`).

---

## 📋 Matriz de Rastreabilidade de Requisitos (RTM - ISO/IEC/IEEE 29148)
Mapeamento bidirecional estrito (*Forward & Backward Traceability*) conectando 100% dos requisitos elicitados, regras de negócio, casos de uso UML, pacotes da EAP, código-fonte C# e testes automatizados:
* **[Documento Técnico Completo da Matriz de Rastreabilidade](docs/MATRIZ%20DE%20RASTREABILIDADE/Documento_Matriz_de_Rastreabilidade.md)**: Matrizes analíticas cobrindo os 35 Requisitos Funcionais (RF001 a RF035), 8 Requisitos Não-Funcionais (RNF001 a RNF008), 8 Regras de Negócio e cruzamento com a suíte de QA.
* **[Matriz de Rastreabilidade em PDF para Impressão](docs/MATRIZ%20DE%20RASTREABILIDADE/Documento_Matriz_de_Rastreabilidade.pdf)**: Versão em alta fidelidade tipográfica (699 KB) formatada para a banca examinadora do Trabalho de Graduação.
* **[Layout Interativo em HTML5](docs/MATRIZ%20DE%20RASTREABILIDADE/Documento_Matriz_de_Rastreabilidade.html)**: Visualização estruturada com métricas executivas de cobertura (100% de cobertura de requisitos, 0 requisitos órfãos, 0 código órfão).

---

## 🔍 Rastreabilidade de Regras de Negócio (RN)
Para fins de auditoria da banca avaliadora, as regras de negócio cruciais descritas na documentação técnica estão rigorosamente implementadas e mapeadas no código-fonte conforme os identificadores abaixo:

| Identificador | Descrição da Regra de Negócio | Mapeamento no Sistema |
| :--- | :--- | :--- |
| **RN001** | O preço do material é calculado estritamente por grama (`Preço do Rolo / Peso Total em Gramas`). | `Models/Material.cs` e `MaterialPage.razor` |
| **RN002** | Insumos químicos de acabamento (tintas, primers, vernizes, solventes) são dosados e precificados por mililitro (`mL`). | `Models/Acabamento.cs` e `AcabamentoPage.razor` |
| **RN003** | Cálculo do consumo de energia elétrica proporcional à potência da impressora (Watts), tempo de impressão (horas) e tarifa de kWh. | `Models/Maquina.cs` e `Orcamentos.razor` |
| **RN004** | Gatilho de baixa física de estoque ocorre estritamente quando o orçamento transita para "Concluído" ou "Falha de Impressão" (Espaguete). | `Components/Pages/Producao/Producao.razor` |
| **RN005** | Exclusões manuais de registros exigem confirmação explícita em tela do usuário para prevenção de destruição de dados. | Camada de Visão Blazor (`Components/Pages/`) |
| **RN006** | Rotina automática de expurgo via *Timer* para exclusão de registros de orçamentos inativos há mais de 180 dias (6 meses). | `Services/DatabaseCleanupService.cs` |
| **RN007** | Preservação contábil perpétua: valores totais de orçamentos expurgados são salvaguardados no cofre acumulador de Histórico Financeiro. | `Repositories/HistoricoFinanceiroRepository.cs` e `Dashboard.razor` |
| **RN008** | Isolamento arquitetural Multi-Tenant estrito (Anti-IDOR): toda consulta e gravação é segregada pelo `UsuarioId` da sessão. | `Repositories/` e `Data/AppDbContext.cs` |

---

## 🧪 Garantia da Qualidade (QA)
A integridade dos cálculos, validadores de domínio (anti-XSS em links STL, dígitos verificadores de CPF/CNPJ) e isolamento multi-tenant é assegurada por suíte automatizada de testes com **xUnit** e **FluentValidation**:
```powershell
dotnet test tests/LucroImpresso.Tests/LucroImpresso.Tests.csproj
```

