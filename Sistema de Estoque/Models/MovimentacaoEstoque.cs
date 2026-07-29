namespace SistemaEstoque.Models
{
    public class MovimentacaoEstoque
    {
        public int IdMovimentacao { get; set; }

        public int IdProduto { get; set; }
        public Produto? Produto { get; set; }

        public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }

        public string Tipo { get; set; }
        public int Quantidade { get; set; }
        public DateTime DataMovimentacao { get; set; }
        public string? Observacao { get; set; }
    }
}