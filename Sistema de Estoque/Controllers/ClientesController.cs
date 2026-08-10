using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public ClientesController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/Clientes/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Vendas)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound(new { mensagem = "Cliente não encontrado." });

            return cliente;
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nome))
                return BadRequest(new { mensagem = "O nome do cliente é obrigatório." });

            _context.Clientes.Add(cliente);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCliente),
                new { id = cliente.IdCliente },
                cliente
            );
        }

        // PUT: api/Clientes/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(
            int id,
            Cliente cliente)
        {
            if (id != cliente.IdCliente)
                return BadRequest(new { mensagem = "ID inválido." });

            var existente = await _context.Clientes.FindAsync(id);

            if (existente == null)
                return NotFound(new { mensagem = "Cliente não encontrado." });

            existente.Nome = cliente.Nome;
            existente.Cpf = cliente.Cpf;
            existente.Telefone = cliente.Telefone;
            existente.Email = cliente.Email;
            existente.Endereco = cliente.Endereco;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Clientes/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
                return NotFound(new { mensagem = "Cliente não encontrado." });

            var possuiVendas = await _context.Vendas
                .AnyAsync(v => v.IdCliente == id);

            if (possuiVendas)
                return BadRequest(new
                {
                    mensagem = "Não é possível excluir um cliente que possui vendas."
                });

            _context.Clientes.Remove(cliente);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}