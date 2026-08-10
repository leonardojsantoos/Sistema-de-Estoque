using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendasController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public VendasController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: api/Vendas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venda>>> GetVendas()
        {
            return await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/Vendas/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Venda>> GetVenda(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Itens)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(v => v.IdVenda == id);

            if (venda == null)
                return NotFound(new { mensagem = "Venda não encontrada." });

            return venda;
        }

        // POST: api/Vendas
        [HttpPost]
        public async Task<ActionResult<Venda>> PostVenda(Venda venda)
        {
            var clienteExiste = await _context.Clientes
                .AnyAsync(c => c.IdCliente == venda.IdCliente);

            if (!clienteExiste)
                return BadRequest(new
                {
                    mensagem = "Cliente não encontrado."
                });

            if (venda.Itens == null || !venda.Itens.Any())
                return BadRequest(new
                {
                    mensagem = "A venda precisa possuir pelo menos um item."
                });

            foreach (var item in venda.Itens)
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

                if (produto.Estoque < item.Quantidade)
                    return BadRequest(new
                    {
                        mensagem = $"Estoque insuficiente para o produto {produto.Nome}."
                    });

                item.Subtotal = item.Quantidade * item.ValorUnitario;

                produto.Estoque -= item.Quantidade;
            }

            venda.DataVenda = DateTime.Now;
            venda.ValorTotal = venda.Itens.Sum(i => i.Subtotal);

            _context.Vendas.Add(venda);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetVenda),
                new { id = venda.IdVenda },
                venda
            );
        }

        // DELETE: api/Vendas/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenda(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens)
                .FirstOrDefaultAsync(v => v.IdVenda == id);

            if (venda == null)
                return NotFound(new { mensagem = "Venda não encontrada." });

            foreach (var item in venda.Itens ?? [])
            {
                var produto = await _context.Produtos
                    .FindAsync(item.IdProduto);

                if (produto != null)
                    produto.Estoque += item.Quantidade;
            }

            _context.Vendas.Remove(venda);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}