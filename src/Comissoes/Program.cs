using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Comissoes;

public static class Program
{
    private record ArquivoVendas(List<Venda> Vendas);

    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        var ptBr = new CultureInfo("pt-BR");
        var caminho = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "vendas.json");

        List<Venda> vendas;
        try
        {
            var json = File.ReadAllText(caminho);
            var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            vendas = JsonSerializer.Deserialize<ArquivoVendas>(json, opcoes)?.Vendas ?? [];
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Não foi possível ler o arquivo de vendas: {ex.Message}");
            return 1;
        }

        var resumo = CalculadoraComissao.CalcularPorVendedor(vendas);

        Console.WriteLine($"{"Vendedor",-20}{"Vendas",8}{"Total vendido",18}{"Comissão",15}");
        Console.WriteLine(new string('-', 61));
        foreach (var r in resumo)
            Console.WriteLine($"{r.Vendedor,-20}{r.QuantidadeVendas,8}{r.TotalVendido.ToString("C", ptBr),18}{r.TotalComissao.ToString("C", ptBr),15}");
        Console.WriteLine(new string('-', 61));
        Console.WriteLine($"{"Total de comissões",-46}{resumo.Sum(r => r.TotalComissao).ToString("C", ptBr),15}");
        return 0;
    }
}
