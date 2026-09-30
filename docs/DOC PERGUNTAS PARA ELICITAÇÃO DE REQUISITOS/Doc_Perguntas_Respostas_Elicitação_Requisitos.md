# Documento de Elicitação de Requisitos — Sistema LucroImpresso

**Projeto:** LucroImpresso — Sistema Especializado em Gestão de Custos e Precificação para Impressão 3D  
**Trabalho de Graduação (TG):** Faculdade de Tecnologia do Estado de São Paulo (FATEC)  
**Entrevistador (Analista):** Felipe Silveira Pessoa  
**Entrevistado (Cliente/Especialista):** Empreendedor de Impressão 3D e Modelagem  
**Data da Entrevista:** 03/03/2026  

---

### 1. Fluxo de Entrada e Pedidos

**Pergunta 1:** Quando um cliente solicita o orçamento de uma peça ou action figure, como você recebe o modelo 3D? O arquivo já vem pronto para impressão?  
**Resposta do Cliente:** Na maioria das vezes, o cliente não possui o arquivo STL. Ele envia apenas uma imagem de referência ou descreve a ideia. Eu sou o responsável por buscar o modelo em plataformas externas (como Thingiverse ou Cults3D) ou realizar a modelagem do zero. São raros os casos em que o arquivo STL já vem pronto do cliente.

**Pergunta 2:** Após obter o arquivo STL, como você avalia a viabilidade da impressão e os recursos necessários?  
**Resposta do Cliente:** O arquivo STL precisa ser processado em um software fatiador (como Ultimaker Cura ou PrusaSlicer). Esse software me informa se a peça necessita de suportes, a estimativa do peso em gramas do material que será consumido e o tempo total em horas que a impressora ficará operando.

---

### 2. Precificação e Gestão de Custos

**Pergunta 3:** Atualmente, qual é o método utilizado para calcular o preço final cobrado do cliente?  
**Resposta do Cliente:** O processo atual é desestruturado. Eu verifico a estimativa de gramas no fatiador, divido pelo valor pago no rolo de filamento ou resina e tento aplicar uma regra de três. No entanto, é comum eu esquecer de embutir o custo da energia elétrica consumida durante as horas de impressão e não aplico uma margem padronizada para o desgaste e manutenção da máquina, o que compromete a margem de lucro real.

**Pergunta 4:** Descreva o cenário ideal de como um sistema de software poderia resolver esse problema de precificação.  
**Resposta do Cliente:** O ideal seria um sistema onde eu pudesse inserir os dados gerados pelo fatiador (peso da peça e tempo de máquina), selecionar a máquina e o material cadastrado em estoque, e o software automaticamente calculasse o custo real físico de produção (somando material e energia elétrica com base no consumo da máquina). Ao preencher o valor cobrado pelo meu tempo de serviço (mão de obra por hora), o sistema deve somar esses valores para gerar o preço de venda final sugerido e nos mostrar a porcentagem de margem de lucro real gerada sobre essa venda.

---

### 3. Gestão de Estoque e Tratamento de Falhas

**Pergunta 5:** Como é feito o controle atual do estoque de materiais (filamentos e resinas)?  
**Resposta do Cliente:** O controle é puramente visual. Eu olho para o rolo e estimo o que resta. O sistema ideal deveria, ao confirmar a venda ou a conclusão da impressão, subtrair automaticamente a quantidade de gramas utilizadas daquele rolo específico, permitindo a previsibilidade de compra antes que o material acabe durante um serviço.

**Pergunta 6:** Como o sistema deve lidar com impressões que falham (perda de material) ou orçamentos rejeitados?  
**Resposta do Cliente:** O sistema deve permitir registrar a falha de uma impressão para que o material perdido seja descontado do estoque, mesmo sem a geração de receita. Além disso, orçamentos que não forem aprovados pelo cliente devem poder ser arquivados ou excluídos para não poluir a tela principal.

---

### 4. Interface e Usabilidade

**Pergunta 7:** Em quais dispositivos e ambientes o sistema será acessado no seu dia a dia?  
**Resposta do Cliente:** O acesso principal será pelo navegador do computador desktop, que fica próximo à impressora e onde utilizo o software fatiador. Porém, é essencial que a interface se adapte à tela do celular (responsividade) para que eu possa gerar e enviar orçamentos rápidos aos clientes quando estiver fora do local de trabalho.

**Pergunta 8:** Quais informações são prioritárias para visualização imediata na tela inicial (Dashboard) do sistema?  
**Resposta do Cliente:** A tela inicial precisa exibir um resumo claro: a lista de orçamentos pendentes de aprovação e alertas sobre os materiais que estão com o nível de estoque baixo (em gramas).

---

### 5. Segurança e Controle de Acesso

**Pergunta 9:** O sistema será utilizado por múltiplos funcionários ou apenas por você? Há necessidade de restrição de acesso?  
**Resposta do Cliente:** O uso será exclusivo meu. Contudo, é fundamental que o sistema possua uma tela de login com senha. Os dados financeiros armazenados, como os custos de material, lucro e os orçamentos dos clientes, são informações sensíveis do negócio e não podem ficar abertos e acessíveis no navegador.

---

### 6. Regras de Negócio e Edição de Dados

**Pergunta 10:** Após um orçamento ser salvo no banco de dados, é necessário que ele possa ser alterado posteriormente?  
**Resposta do Cliente:** Sim, a edição e exclusão são obrigatórias. Muitas vezes o cliente solicita alterações no tamanho da peça, o que muda o peso e o tempo de impressão. Preciso conseguir abrir o orçamento salvo e atualizar esses valores sem precisar recriar o registro inteiro do zero.
