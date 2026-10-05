using System.Globalization;
using System.Text;

namespace Juros;

public static class Program
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    // Uso: Juros [valor] [vencimento dd/MM/aaaa]. Sem argumentos, pergunta no console.
    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        var textoValor = args.Length > 0 ? args[0] : Perguntar("Valor (ex.: 1.500,00): ");
        var textoData = args.Length > 1 ? args[1] : Perguntar("Data de vencimento (dd/mm/aaaa): ");

        if (!decimal.TryParse(textoValor.Replace("R$", "").Trim(), NumberStyles.Currency, PtBr, out var valor) || valor <= 0)
        {
            Console.Error.WriteLine("Valor inválido. Informe um número positivo, ex.: 1.500,00");
            return 1;
        }

        if (!DateOnly.TryParseExact(textoData.Trim(), "dd/MM/yyyy", PtBr, DateTimeStyles.None, out var vencimento))
        {
            Console.Error.WriteLine("Data inválida. Use o formato dd/mm/aaaa.");
            return 1;
        }

        var resultado = CalculadoraJuros.Calcular(valor, vencimento, DateOnly.FromDateTime(DateTime.Today));

        Console.WriteLine();
        Console.WriteLine($"Valor original   : {resultado.ValorOriginal.ToString("C", PtBr)}");
        Console.WriteLine($"Vencimento       : {resultado.Vencimento.ToString("dd/MM/yyyy", PtBr)}");
        Console.WriteLine($"Data de hoje     : {resultado.DataCalculo.ToString("dd/MM/yyyy", PtBr)}");
        Console.WriteLine($"Dias em atraso   : {resultado.DiasAtraso}");
        Console.WriteLine($"Juros (2,5%/dia) : {resultado.Juros.ToString("C", PtBr)}");
        Console.WriteLine($"Valor atualizado : {resultado.ValorAtualizado.ToString("C", PtBr)}");
        if (resultado.DiasAtraso == 0)
            Console.WriteLine("Título não está vencido, não há juros a cobrar.");

        return 0;
    }

    private static string Perguntar(string mensagem)
    {
        Console.Write(mensagem);
        return Console.ReadLine() ?? "";
    }
}
