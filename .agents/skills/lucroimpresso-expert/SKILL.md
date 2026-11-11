---
name: lucroimpresso-expert
description: >
  Domain engineering and architectural knowledge for the LucroImpresso 3D printing
  cost calculation system. Use when implementing, refactoring, or verifying business rules,
  pricing formulas, models, database schemas, multi-tenant filters, or UI features for
  3D makers, print farms, and additive manufacturing.
---

# LucroImpresso Expert: Regras de Negócio e Domínio Maker

Você é o especialista do produto **LucroImpresso**, sistema de gestão financeira e orçamentação técnica para oficinas de manufatura aditiva (Impressão 3D FDM e Resina/SLA).

## 1. Modelo de Negócio e Precificação

O cálculo de um orçamento de impressão 3D é composto por quatro parcelas essenciais:

### A. Custo de Material (Filamento / Resina)
$$\text{Custo Material} = \text{Peso (g)} \times \text{Preço por Grama}$$
Onde:
- `Preço por Grama` = `Preço do Rolo / Peso Total do Rolo (g)`
- Tipos de material suportados: PLA, ABS, PETG, TPU, Resina Standard, Resina Tough, etc.

### B. Custo de Energia Elétrica
$$\text{Custo Energia} = \left(\frac{\text{Consumo em Watts}}{1000}\right) \times \text{Tempo de Impressão (Horas)} \times \text{Custo do kWh (R\$)}$$

### C. Custo de Mão de Obra e Pós-Processamento
$$\text{Tempo Total} = \text{Tempo Fatiamento} + \text{Tempo Calibração} + \text{Tempo Retirada} + \text{Tempo Pintura/Lixamento}$$
$$\text{Custo Mão de Obra} = \text{Tempo Total} \times \text{Custo Hora do Maker}$$

### D. Insumos de Acabamento
$$\text{Custo Acabamentos} = \sum (\text{Quantidade em mL ou Unidade} \times \text{Preço por mL/Un})$$
Exemplos: Álcool Isopropílico (IPA), primer spray, verniz, cola de fixação, lixas, fita de embalagem.

### E. Custo Final e Margem
$$\text{Custos Físicos} = \text{Custo Material} + \text{Custo Energia} + \text{Custo Acabamentos}$$
$$\text{Custo Final (Preço)} = \text{Custos Físicos} + \text{Custo Mão de Obra}$$
$$\text{Margem de Lucro (\%)} = \left(\frac{\text{Custo Mão de Obra}}{\text{Custo Final}}\right) \times 100$$

## 2. Entidades Principais e Banco de Dados (MariaDB 10.11)

- **`Orcamento`**: Entidade central de orçamentos, com status de fluxo (`Pendente`, `Aprovado`, `EmProducao`, `Concluido`, `Entregue`, `Cancelado`).
- **`Maquina`**: Impressoras cadastradas (FDM/SLA) com `ConsumoWatts`, `CustoKwh` e vida útil.
- **`Material`**: Filamentos e resinas com cor, tipo, diâmetro, preço de compra e peso/volume total.
- **`Acabamento`** & **`OrcamentoAcabamento`**: Insumos secundários associados ao orçamento.
- **`Cliente`**, **`PessoaFisica`**, **`PessoaJuridica`**: Base de clientes com validação estrita de CPF/CNPJ e endereços.
- **`Usuario`**: Autenticação com isolamento `TenantId` e senha com hash BCrypt.

## 3. Diretrizes de Design UI (Neo-Brutalismo)

- Estética industrial, limpa e funcional para makers.
- Paleta: Fundo claro/neutro, cores primárias vibrantes (amarelo industrial `#FFE600`, azul elétrico, verde neon), contrastadas com bordas pretas sólidas (`2px solid #000` a `4px solid #000`).
- Sombras: Sem desfoque suave (`box-shadow: 4px 4px 0px #000;`).
- Tipografia: Legível, geométrica, com números e valores monetários em destaque (`font-weight: 700`).
