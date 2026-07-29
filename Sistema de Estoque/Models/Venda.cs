namespace SistemaEstoque.Models
{
    public class Venda
    {
        public int IdVenda { get; set; }

        public int IdCliente { get; set; }
        public Cliente? Cliente { get; set; }

        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }

        public ICollection<ItemVenda>? Itens { get; set; }
    }
}