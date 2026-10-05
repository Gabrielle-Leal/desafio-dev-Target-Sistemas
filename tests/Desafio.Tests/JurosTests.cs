using Juros;

namespace Desafio.Tests;

public class JurosTests
{
    private static readonly DateOnly Hoje = new(2026, 10, 5);

    [Fact]
    public void Atrasado_CalculaJurosSimplesDe2Virgula5PorDia()
    {
        var r = CalculadoraJuros.Calcular(1000m, new DateOnly(2026, 10, 1), Hoje);

        Assert.Equal(4, r.DiasAtraso);
        Assert.Equal(100m, r.Juros);
        Assert.Equal(1100m, r.ValorAtualizado);
    }

    [Theory]
    [InlineData(2026, 10, 5)]  // vence hoje
    [InlineData(2026, 10, 20)] // vence no futuro
    public void SemAtraso_NaoHaJuros(int ano, int mes, int dia)
    {
        var r = CalculadoraJuros.Calcular(1000m, new DateOnly(ano, mes, dia), Hoje);

        Assert.Equal(0, r.DiasAtraso);
        Assert.Equal(0m, r.Juros);
    }

    [Fact]
    public void Juros_SaoArredondadosEmDuasCasas()
    {
        var r = CalculadoraJuros.Calcular(333.33m, new DateOnly(2026, 10, 4), Hoje);

        Assert.Equal(8.33m, r.Juros); // 333,33 * 2,5% = 8,33325
    }

    [Fact]
    public void ValorNaoPositivo_LancaErro()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CalculadoraJuros.Calcular(0m, Hoje, Hoje));
    }
}
