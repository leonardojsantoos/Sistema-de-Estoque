namespace SistemaEstoque.Models
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }

        public ICollection<Produto>? Produtos { get; set; }
    }
}