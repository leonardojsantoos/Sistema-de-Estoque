using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public ProdutosController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: api/Produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/Produtos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .Include(p => p.Fornecedor)
                .Include(p => p.ItensCompra)
                .Include(p => p.ItensVenda)
                .Include(p => p.Movimentacoes)
                .FirstOrDefaultAsync(p => p.IdProduto == id);

            if (produto == null)
            {
                return NotFound(new
                {
                    mensagem = "Produto não encontrado."
                });
            }

            return produto;
        }

        // POST: api/Produtos
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(
            Produto produto)
        {
            if (string.IsNullOrWhiteSpace(produto.Nome))
            {
                return BadRequest(new
                {
                    mensagem = "O nome do produto é obrigatório."
                });
            }

            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == produto.IdCategoria);

            if (!categoriaExiste)
            {
                return BadRequest(new
                {
                    mensagem = "A categoria informada não existe."
                });
            }

            var fornecedorExiste = await _context.Fornecedores
                .AnyAsync(f => f.IdFornecedor == produto.IdFornecedor);

            if (!fornecedorExiste)
            {
                return BadRequest(new
                {
                    mensagem = "O fornecedor informado não existe."
                });
            }

            // Produto começa com estoque zero.
            produto.Estoque = 0;

            _context.Produtos.Add(produto);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProduto),
                new { id = produto.IdProduto },
                produto
            );
        }

        // PUT: api/Produtos/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(
            int id,
            Produto produto)
        {
            if (id != produto.IdProduto)
            {
                return BadRequest(new
                {
                    mensagem = "O ID informado é diferente do ID do produto."
                });
            }

            var produtoExistente = await _context.Produtos
                .FindAsync(id);

            if (produtoExistente == null)
            {
                return NotFound(new
                {
                    mensagem = "Produto não encontrado."
                });
            }

            var categoriaExiste = await _context.Categorias
                .AnyAsync(c => c.IdCategoria == produto.IdCategoria);

            if (!categoriaExiste)
            {
                return BadRequest(new
                {
                    mensagem = "A categoria informada não existe."
                });
            }

            var fornecedorExiste = await _context.Fornecedores
                .AnyAsync(f => f.IdFornecedor == produto.IdFornecedor);

            if (!fornecedorExiste)
            {
                return BadRequest(new
                {
                    mensagem = "O fornecedor informado não existe."
                });
            }

            produtoExistente.IdCategoria = produto.IdCategoria;
            produtoExistente.IdFornecedor = produto.IdFornecedor;
            produtoExistente.Nome = produto.Nome;
            produtoExistente.Descricao = produto.Descricao;
            produtoExistente.CodigoBarras = produto.CodigoBarras;
            produtoExistente.PrecoCusto = produto.PrecoCusto;
            produtoExistente.PrecoVenda = produto.PrecoVenda;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Produtos/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos
                .FindAsync(id);

            if (produto == null)
            {
                return NotFound(new
                {
                    mensagem = "Produto não encontrado."
                });
            }

            if (produto.Estoque > 0)
            {
                return BadRequest(new
                {
                    mensagem = "Não é possível excluir um produto que possui estoque."
                });
            }

            _context.Produtos.Remove(produto);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}