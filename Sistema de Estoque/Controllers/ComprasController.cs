using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public ComprasController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Compra>>> GetCompras()
        {
            return await _context.Compras
                .Include(c => c.Fornecedor)
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/Compras/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> GetCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Fornecedor)
                .Include(c => c.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(c => c.IdCompra == id);

            if (compra == null)
                return NotFound(new { mensagem = "Compra não encontrada." });

            return compra;
        }

        // POST: api/Compras
        [HttpPost]
        public async Task<ActionResult<Compra>> PostCompra(Compra compra)
        {
            var fornecedorExiste = await _context.Fornecedores
                .AnyAsync(f => f.IdFornecedor == compra.IdFornecedor);

            if (!fornecedorExiste)
                return BadRequest(new
                {
                    mensagem = "Fornecedor não encontrado."
                });

            if (compra.Itens == null || !compra.Itens.Any())
                return BadRequest(new
                {
                    mensagem = "A compra precisa possuir pelo menos um item."
                });

            foreach (var item in compra.Itens)
            {
                var produto = await _context.Produtos
                    .FindAsync(item.IdProduto);

                if (produto == null)
                    return BadRequest(new
                    {
                        mensagem = $"Produto {item.IdProduto} não encontrado."
                    });

                if (item.Quantidade <= 0)
                    return BadRequest(new
                    {
                        mensagem = "A quantidade deve ser maior que zero."
                    });

                item.Subtotal = item.Quantidade * item.ValorUnitario;

                produto.Estoque += item.Quantidade;
            }

            compra.DataCompra = DateTime.Now;
            compra.ValorTotal = compra.Itens.Sum(i => i.Subtotal);

            _context.Compras.Add(compra);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCompra),
                new { id = compra.IdCompra },
                compra
            );
        }

        // DELETE: api/Compras/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.IdCompra == id);

            if (compra == null)
                return NotFound(new { mensagem = "Compra não encontrada." });

            foreach (var item in compra.Itens ?? [])
            {
                var produto = await _context.Produtos
                    .FindAsync(item.IdProduto);

                if (produto != null)
                    produto.Estoque -= item.Quantidade;
            }

            _context.Compras.Remove(compra);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}