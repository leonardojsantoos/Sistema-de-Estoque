using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItensVendaController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public ItensVendaController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: api/ItensVenda
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemVenda>>> GetItensVenda()
        {
            return await _context.ItensVenda
                .Include(i => i.Venda)
                .Include(i => i.Produto)
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/ItensVenda/1
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemVenda>> GetItemVenda(int id)
        {
            var item = await _context.ItensVenda
                .Include(i => i.Venda)
                .Include(i => i.Produto)
                .FirstOrDefaultAsync(i => i.IdItemVenda == id);

            if (item == null)
                return NotFound(new
                {
                    mensagem = "Item de venda não encontrado."
                });

            return item;
        }

        // POST: api/ItensVenda
        [HttpPost]
        public async Task<ActionResult<ItemVenda>> PostItemVenda(
            ItemVenda item)
        {
            var vendaExiste = await _context.Vendas
                .AnyAsync(v => v.IdVenda == item.IdVenda);

            if (!vendaExiste)
                return BadRequest(new
                {
                    mensagem = "Venda não encontrada."
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

            if (produto.Estoque < item.Quantidade)
                return BadRequest(new
                {
                    mensagem = "Estoque insuficiente."
                });

            item.Subtotal = item.Quantidade * item.ValorUnitario;

            produto.Estoque -= item.Quantidade;

            _context.ItensVenda.Add(item);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetItemVenda),
                new { id = item.IdItemVenda },
                item
            );
        }

        // DELETE: api/ItensVenda/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemVenda(int id)
        {
            var item = await _context.ItensVenda
                .FirstOrDefaultAsync(i => i.IdItemVenda == id);

            if (item == null)
                return NotFound(new
                {
                    mensagem = "Item de venda não encontrado."
                });

            var produto = await _context.Produtos
                .FindAsync(item.IdProduto);

            if (produto != null)
            {
                produto.Estoque += item.Quantidade;
            }

            _context.ItensVenda.Remove(item);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}