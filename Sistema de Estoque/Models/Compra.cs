namespace SistemaEstoque.Models
{
    public class Compra
    {
        public int IdCompra { get; set; }

        public int IdFornecedor { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        public DateTime DataCompra { get; set; }
        public decimal ValorTotal { get; set; }

        public ICollection<ItemCompra>? Itens { get; set; }
    }
}