namespace Comissoes;

public record Venda(string Vendedor, decimal Valor);

public record ResumoVendedor(string Vendedor, int QuantidadeVendas, decimal TotalVendido, decimal TotalComissao);

public static class CalculadoraComissao
{
    private const decimal LimiteSemComissao = 100m;
    private const decimal LimiteComissaoMenor = 500m;
    private const decimal PercentualMenor = 0.01m;
    private const decimal PercentualMaior = 0.05m;

    /// <summary>
    /// Regra por venda: abaixo de R$ 100,00 não gera comissão; abaixo de R$ 500,00 gera 1%;
    /// a partir de R$ 500,00 gera 5%.
    /// </summary>
    public static decimal PercentualPorVenda(decimal valor) => valor switch
    {
        < LimiteSemComissao => 0m,
        < LimiteComissaoMenor => PercentualMenor,
        _ => PercentualMaior
    };

    public static decimal ComissaoDaVenda(decimal valor) => valor * PercentualPorVenda(valor);

    public static IReadOnlyList<ResumoVendedor> CalcularPorVendedor(IEnumerable<Venda> vendas) =>
        vendas
            .GroupBy(v => v.Vendedor)
            .Select(g => new ResumoVendedor(
                g.Key,
                g.Count(),
                g.Sum(v => v.Valor),
                // Arredonda só no total para não acumular erro venda a venda
                Math.Round(g.Sum(v => ComissaoDaVenda(v.Valor)), 2, MidpointRounding.AwayFromZero)))
            .ToList();
}
