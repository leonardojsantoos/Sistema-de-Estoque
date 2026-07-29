namespace SistemaEstoque.Models
{
    public class Produto
    {
        public int IdProduto { get; set; }

        public int IdCategoria { get; set; }
        public Categoria? Categoria { get; set; }

        public int IdFornecedor { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public string? CodigoBarras { get; set; }

        public decimal? PrecoCusto { get; set; }
        public decimal? PrecoVenda { get; set; }

        public int Estoque { get; set; }

        public ICollection<ItemCompra>? ItensCompra { get; set; }
        public ICollection<ItemVenda>? ItensVenda { get; set; }
        public ICollection<MovimentacaoEstoque>? Movimentacoes { get; set; }
    }
}