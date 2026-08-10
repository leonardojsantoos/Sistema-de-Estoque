using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Models;

namespace SistemaEstoque.Data
{
    public class EstoqueDbContext : DbContext
    {
        public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(p => p.IdProduto);

                entity.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(p => p.Descricao)
                    .HasMaxLength(500);

                entity.Property(p => p.CodigoBarras)
                    .HasMaxLength(50);

                entity.Property(p => p.PrecoCusto)
                    .HasPrecision(18, 2);

                entity.Property(p => p.PrecoVenda)
                    .HasPrecision(18, 2);

                entity.HasOne(p => p.Categoria)
                    .WithMany(c => c.Produtos)
                    .HasForeignKey(p => p.IdCategoria)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Fornecedor)
                    .WithMany(f => f.Produtos)
                    .HasForeignKey(p => p.IdFornecedor)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(c => c.IdCategoria);

                entity.Property(c => c.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(c => c.Descricao)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Fornecedor>(entity =>
            {
                entity.HasKey(f => f.IdFornecedor);

                entity.Property(f => f.RazaoSocial)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(f => f.NomeFantasia)
                    .HasMaxLength(200);

                entity.Property(f => f.Cnpj)
                    .HasMaxLength(18);

                entity.Property(f => f.Telefone)
                    .HasMaxLength(20);

                entity.Property(f => f.Email)
                    .HasMaxLength(150);

                entity.Property(f => f.Endereco)
                    .HasMaxLength(300);
            });
        }
    }
}