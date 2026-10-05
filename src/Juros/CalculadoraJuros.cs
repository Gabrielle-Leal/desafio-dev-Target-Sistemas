namespace Juros;

public record ResultadoJuros(decimal ValorOriginal, DateOnly Vencimento, DateOnly DataCalculo, int DiasAtraso, decimal Juros)
{
    public decimal ValorAtualizado => ValorOriginal + Juros;
}

public static class CalculadoraJuros
{
    /// <summary>Multa de 2,5% ao dia, aplicada como juros simples sobre o valor original.</summary>
    public const decimal TaxaDiaria = 0.025m;

    public static ResultadoJuros Calcular(decimal valor, DateOnly vencimento, DateOnly dataCalculo)
    {
        if (valor <= 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor deve ser maior que zero.");

        var diasAtraso = Math.Max(dataCalculo.DayNumber - vencimento.DayNumber, 0);
        var juros = Math.Round(valor * TaxaDiaria * diasAtraso, 2, MidpointRounding.AwayFromZero);

        return new ResultadoJuros(valor, vencimento, dataCalculo, diasAtraso, juros);
    }
}
