# Desafio Técnico – Target Sistemas

Solução dos três exercícios em **C# / .NET 10**, com testes automatizados (xUnit).

## Estrutura

```
src/
  Comissoes/   Exercício 1 – comissão por vendedor (lê vendas.json)
  Estoque/     Exercício 2 – movimentações de estoque (lê estoque.json)
  Juros/       Exercício 3 – juros por atraso (2,5% ao dia)
tests/
  Desafio.Tests/  Testes unitários das regras de negócio
```

As regras de negócio ficam em classes próprias (`CalculadoraComissao`, `Deposito`,
`CalculadoraJuros`), separadas da interação com o console (`Program.cs`), para que possam
ser testadas de forma isolada. Valores monetários usam `decimal` para evitar erros de
arredondamento.

## Como executar

Pré-requisito: [.NET SDK 10](https://dotnet.microsoft.com/download).

```bash
dotnet run --project src/Comissoes
dotnet run --project src/Estoque
dotnet run --project src/Juros                          # pergunta valor e vencimento
dotnet run --project src/Juros -- "1.000,00" 01/10/2026 # ou recebe por argumento
dotnet test
```

## Exercício 1 – Comissões

Regra aplicada a **cada venda**: abaixo de R$ 100,00 não gera comissão, abaixo de R$ 500,00
gera 1% e a partir de R$ 500,00 gera 5%. As comissões são somadas por vendedor.

| Vendedor        | Vendas | Total vendido | Comissão    |
|-----------------|-------:|--------------:|------------:|
| João Silva      | 10     | R$ 10.754,70  | R$ 495,68   |
| Maria Souza     | 9      | R$ 9.874,30   | R$ 465,95   |
| Carlos Oliveira | 8      | R$ 7.928,35   | R$ 379,37   |
| Ana Lima        | 9      | R$ 8.763,95   | R$ 404,98   |
| **Total**       |        |               | **R$ 1.745,98** |

## Exercício 2 – Estoque

Menu interativo para lançar entradas e saídas, consultar o estoque e ver o histórico.
Cada movimentação tem:

- **ID único** sequencial;
- **descrição** do tipo de movimentação (ex.: "Compra de fornecedor", "Venda", "Devolução");
- tipo (entrada/saída), quantidade, data/hora e a **quantidade final em estoque** do produto,
  exibida ao término do lançamento.

Validações: produto inexistente, quantidade menor ou igual a zero e saída maior que o saldo
disponível (o estoque nunca fica negativo). As movimentações ficam em memória durante a execução.

## Exercício 3 – Juros

A partir de um valor e uma data de vencimento, calcula os juros até a data de hoje com
multa de **2,5% ao dia**, em regime de juros simples:

```
juros = valor × 0,025 × dias de atraso
```

Se o vencimento for hoje ou no futuro, não há juros. Exemplo: R$ 1.000,00 vencido há 4 dias
→ R$ 100,00 de juros, total de R$ 1.100,00.
