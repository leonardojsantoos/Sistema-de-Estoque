using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItensCompraController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public ItensCompraController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: api/ItensCompra
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemCompra>>> GetItensCompra()
        {
            return await _context.ItensCompra
                .Include(i => i.Compra)
                .Include(i => i.Produto)
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/ItensCompra/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemCompra>> GetItemCompra(int id)
        {
            var item = await _context.ItensCompra
                .Include(i => i.Compra)
                .Include(i => i.Produto)
                .FirstOrDefaultAsync(i => i.IdItemCompra == id);

            if (item == null)
                return NotFound(new
                {
                    mensagem = "Item de compra não encontrado."
                });

            return item;
        }

        // POST: api/ItensCompra
        [HttpPost]
        public async Task<ActionResult<ItemCompra>> PostItemCompra(
            ItemCompra item)
        {
            var compraExiste = await _context.Compras
                .AnyAsync(c => c.IdCompra == item.IdCompra);

            if (!compraExiste)
                return BadRequest(new
                {
                    mensagem = "Compra não encontrada."
                });

            var produto = await _context.Produtos
                .FindAsync(item.IdProduto);

            if (produto == null)
                return BadRequest(new
                {
                    mensagem = "Produto não encontrado."
                });

            if (item.Quantidade <= 0)
                return BadRequest(new
                {
                    mensagem = "A quantidade deve ser maior que zero."
                });

            item.Subtotal = item.Quantidade * item.ValorUnitario;

            produto.Estoque += item.Quantidade;

            _context.ItensCompra.Add(item);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetItemCompra),
                new { id = item.IdItemCompra },
                item
            );
        }

        // DELETE: api/ItensCompra/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemCompra(int id)
        {
            var item = await _context.ItensCompra
                .FirstOrDefaultAsync(i => i.IdItemCompra == id);

            if (item == null)
                return NotFound(new
                {
                    mensagem = "Item de compra não encontrado."
                });

            var produto = await _context.Produtos
                .FindAsync(item.IdProduto);

            if (produto != null)
            {
                produto.Estoque -= item.Quantidade;
            }

            _context.ItensCompra.Remove(item);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}