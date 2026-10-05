using System.Text;
using System.Text.Json;

namespace Estoque;

public static class Program
{
    private record ArquivoEstoque(List<Produto> Estoque);

    public static int Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        var caminho = args.Length > 0 ? args[0] : Path.Combine(AppContext.BaseDirectory, "estoque.json");

        Deposito deposito;
        try
        {
            var json = File.ReadAllText(caminho);
            var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            deposito = new Deposito(JsonSerializer.Deserialize<ArquivoEstoque>(json, opcoes)?.Estoque ?? []);
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            Console.Error.WriteLine($"Não foi possível ler o arquivo de estoque: {ex.Message}");
            return 1;
        }

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== Controle de Estoque ===");
            Console.WriteLine("1 - Entrada de mercadoria");
            Console.WriteLine("2 - Saída de mercadoria");
            Console.WriteLine("3 - Consultar estoque");
            Console.WriteLine("4 - Histórico de movimentações");
            Console.WriteLine("0 - Sair");
            Console.Write("Opção: ");

            var opcao = Console.ReadLine();
            switch (opcao?.Trim())
            {
                case "1": Lancar(deposito, TipoMovimentacao.Entrada); break;
                case "2": Lancar(deposito, TipoMovimentacao.Saida); break;
                case "3": ListarProdutos(deposito); break;
                case "4": ListarMovimentacoes(deposito); break;
                case "0" or null: return 0;
                default: Console.WriteLine("Opção inválida."); break;
            }
        }
    }

    private static void Lancar(Deposito deposito, TipoMovimentacao tipo)
    {
        ListarProdutos(deposito);
        var codigo = LerInteiro("\nCódigo do produto: ");
        var quantidade = LerInteiro("Quantidade: ");
        Console.Write("Descrição da movimentação (ex.: Compra de fornecedor, Venda, Devolução): ");
        var descricao = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(descricao))
            descricao = tipo == TipoMovimentacao.Entrada ? "Entrada de mercadoria" : "Saída de mercadoria";

        try
        {
            var mov = deposito.Movimentar(codigo, tipo, quantidade, descricao);
            var produto = deposito.ObterProduto(codigo);
            Console.WriteLine($"\nMovimentação #{mov.Id} registrada ({mov.Tipo} - {mov.Descricao}).");
            Console.WriteLine($"Estoque final de {produto.DescricaoProduto}: {mov.EstoqueFinal}");
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    private static int LerInteiro(string mensagem)
    {
        while (true)
        {
            Console.Write(mensagem);
            var entrada = Console.ReadLine() ?? throw new EndOfStreamException("Entrada encerrada.");
            if (int.TryParse(entrada, out var valor))
                return valor;
            Console.WriteLine("Digite um número inteiro válido.");
        }
    }

    private static void ListarProdutos(Deposito deposito)
    {
        Console.WriteLine($"\n{"Código",-8}{"Produto",-30}{"Estoque",8}");
        foreach (var p in deposito.Produtos)
            Console.WriteLine($"{p.CodigoProduto,-8}{p.DescricaoProduto,-30}{p.Estoque,8}");
    }

    private static void ListarMovimentacoes(Deposito deposito)
    {
        if (deposito.Movimentacoes.Count == 0)
        {
            Console.WriteLine("\nNenhuma movimentação lançada.");
            return;
        }

        Console.WriteLine($"\n{"ID",-5}{"Data/hora",-21}{"Produto",-9}{"Tipo",-9}{"Qtde",6}{"Estoque final",15}  Descrição");
        foreach (var m in deposito.Movimentacoes)
            Console.WriteLine($"{m.Id,-5}{m.DataHora:dd/MM/yyyy HH:mm:ss}  {m.CodigoProduto,-9}{m.Tipo,-9}{m.Quantidade,6}{m.EstoqueFinal,15}  {m.Descricao}");
    }
}
