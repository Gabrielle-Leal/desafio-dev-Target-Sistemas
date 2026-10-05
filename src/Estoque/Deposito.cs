namespace Estoque;

public enum TipoMovimentacao
{
    Entrada,
    Saida
}

public class Produto
{
    public int CodigoProduto { get; init; }
    public string DescricaoProduto { get; init; } = "";
    public int Estoque { get; set; }
}

public record Movimentacao(
    int Id,
    int CodigoProduto,
    TipoMovimentacao Tipo,
    string Descricao,
    int Quantidade,
    DateTime DataHora,
    int EstoqueFinal);

public class Deposito
{
    private readonly Dictionary<int, Produto> _produtos;
    private readonly List<Movimentacao> _movimentacoes = [];
    private int _ultimoId;

    public Deposito(IEnumerable<Produto> produtos)
    {
        _produtos = produtos.ToDictionary(p => p.CodigoProduto);
    }

    public IReadOnlyCollection<Produto> Produtos => _produtos.Values;
    public IReadOnlyList<Movimentacao> Movimentacoes => _movimentacoes;

    public Produto ObterProduto(int codigo) =>
        _produtos.TryGetValue(codigo, out var produto)
            ? produto
            : throw new InvalidOperationException($"Produto {codigo} não encontrado.");

    /// <summary>
    /// Lança uma movimentação e retorna o registro com a quantidade final em estoque do produto.
    /// </summary>
    public Movimentacao Movimentar(int codigoProduto, TipoMovimentacao tipo, int quantidade, string descricao)
    {
        if (quantidade <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade deve ser maior que zero.");
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Informe uma descrição para a movimentação.", nameof(descricao));

        var produto = ObterProduto(codigoProduto);

        if (tipo == TipoMovimentacao.Saida && quantidade > produto.Estoque)
            throw new InvalidOperationException(
                $"Estoque insuficiente: disponível {produto.Estoque}, solicitado {quantidade}.");

        produto.Estoque += tipo == TipoMovimentacao.Entrada ? quantidade : -quantidade;

        var movimentacao = new Movimentacao(
            ++_ultimoId,
            codigoProduto,
            tipo,
            descricao.Trim(),
            quantidade,
            DateTime.Now,
            produto.Estoque);

        _movimentacoes.Add(movimentacao);
        return movimentacao;
    }
}
