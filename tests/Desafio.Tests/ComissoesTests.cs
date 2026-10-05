using Comissoes;

namespace Desafio.Tests;

public class ComissoesTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(99.99, 0)]
    [InlineData(100, 0.01)]
    [InlineData(499.99, 0.01)]
    [InlineData(500, 0.05)]
    [InlineData(2100.40, 0.05)]
    public void PercentualPorVenda_RespeitaAsFaixas(decimal valor, decimal esperado)
    {
        Assert.Equal(esperado, CalculadoraComissao.PercentualPorVenda(valor));
    }

    [Fact]
    public void CalcularPorVendedor_AgrupaESomaComissoes()
    {
        var vendas = new[]
        {
            new Venda("Ana", 90m),    // 0
            new Venda("Ana", 200m),   // 2,00
            new Venda("Ana", 1000m),  // 50,00
            new Venda("Bruno", 500m)  // 25,00
        };

        var resumo = CalculadoraComissao.CalcularPorVendedor(vendas);

        var ana = Assert.Single(resumo, r => r.Vendedor == "Ana");
        Assert.Equal(3, ana.QuantidadeVendas);
        Assert.Equal(1290m, ana.TotalVendido);
        Assert.Equal(52m, ana.TotalComissao);
        Assert.Equal(25m, Assert.Single(resumo, r => r.Vendedor == "Bruno").TotalComissao);
    }
}
