namespace SistemaEstoque.Models
{
    public class ItemVenda
    {
        public int IdItemVenda { get; set; }

        public int IdVenda { get; set; }
        public Venda? Venda { get; set; }

        public int IdProduto { get; set; }
        public Produto? Produto { get; set; }

        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}