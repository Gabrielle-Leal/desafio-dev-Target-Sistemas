using Estoque;

namespace Desafio.Tests;

public class EstoqueTests
{
    private static Deposito CriarDeposito() => new(
    [
        new Produto { CodigoProduto = 101, DescricaoProduto = "Caneta Azul", Estoque = 150 },
        new Produto { CodigoProduto = 102, DescricaoProduto = "Caderno Universitário", Estoque = 75 }
    ]);

    [Fact]
    public void Entrada_SomaAoEstoqueERetornaQuantidadeFinal()
    {
        var deposito = CriarDeposito();

        var mov = deposito.Movimentar(101, TipoMovimentacao.Entrada, 50, "Compra de fornecedor");

        Assert.Equal(200, mov.EstoqueFinal);
        Assert.Equal(200, deposito.ObterProduto(101).Estoque);
        Assert.Equal("Compra de fornecedor", mov.Descricao);
    }

    [Fact]
    public void Saida_SubtraiDoEstoque()
    {
        var deposito = CriarDeposito();

        var mov = deposito.Movimentar(102, TipoMovimentacao.Saida, 75, "Venda");

        Assert.Equal(0, mov.EstoqueFinal);
    }

    [Fact]
    public void Movimentacoes_RecebemIdsUnicosESequenciais()
    {
        var deposito = CriarDeposito();

        var a = deposito.Movimentar(101, TipoMovimentacao.Entrada, 1, "Ajuste");
        var b = deposito.Movimentar(102, TipoMovimentacao.Saida, 1, "Venda");
        var c = deposito.Movimentar(101, TipoMovimentacao.Saida, 1, "Venda");

        Assert.Equal([1, 2, 3], new[] { a.Id, b.Id, c.Id });
        Assert.Equal(3, deposito.Movimentacoes.Count);
    }

    [Fact]
    public void Saida_MaiorQueEstoque_LancaErroENaoAlteraNada()
    {
        var deposito = CriarDeposito();

        Assert.Throws<InvalidOperationException>(() =>
            deposito.Movimentar(102, TipoMovimentacao.Saida, 76, "Venda"));

        Assert.Equal(75, deposito.ObterProduto(102).Estoque);
        Assert.Empty(deposito.Movimentacoes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Quantidade_DeveSerPositiva(int quantidade)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CriarDeposito().Movimentar(101, TipoMovimentacao.Entrada, quantidade, "Ajuste"));
    }

    [Fact]
    public void ProdutoInexistente_LancaErro()
    {
        Assert.Throws<InvalidOperationException>(() =>
            CriarDeposito().Movimentar(999, TipoMovimentacao.Entrada, 1, "Ajuste"));
    }

    [Fact]
    public void Descricao_EhObrigatoria()
    {
        Assert.Throws<ArgumentException>(() =>
            CriarDeposito().Movimentar(101, TipoMovimentacao.Entrada, 1, "  "));
    }
}
