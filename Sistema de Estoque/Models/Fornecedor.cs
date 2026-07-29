namespace SistemaEstoque.Models
{
    public class Fornecedor
    {
        public int IdFornecedor { get; set; }
        public string RazaoSocial { get; set; }
        public string? NomeFantasia { get; set; }
        public string? Cnpj { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Endereco { get; set; }

        public ICollection<Produto>? Produtos { get; set; }
        public ICollection<Compra>? Compras { get; set; }
    }
}