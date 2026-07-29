namespace SistemaEstoque.Models
{
    public class ItemCompra
    {
        public int IdItemCompra { get; set; }

        public int IdCompra { get; set; }
        public Compra? Compra { get; set; }

        public int IdProduto { get; set; }
        public Produto? Produto { get; set; }

        public int Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}