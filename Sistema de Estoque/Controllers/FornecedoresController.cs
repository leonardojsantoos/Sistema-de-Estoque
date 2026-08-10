using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FornecedoresController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public FornecedoresController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: api/Fornecedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fornecedor>>> GetFornecedores()
        {
            return await _context.Fornecedores
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/Fornecedores/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Fornecedor>> GetFornecedor(int id)
        {
            var fornecedor = await _context.Fornecedores
                .Include(f => f.Produtos)
                .Include(f => f.Compras)
                .FirstOrDefaultAsync(f => f.IdFornecedor == id);

            if (fornecedor == null)
            {
                return NotFound(new
                {
                    mensagem = "Fornecedor não encontrado."
                });
            }

            return fornecedor;
        }

        // POST: api/Fornecedores
        [HttpPost]
        public async Task<ActionResult<Fornecedor>> PostFornecedor(
            Fornecedor fornecedor)
        {
            if (string.IsNullOrWhiteSpace(fornecedor.RazaoSocial))
            {
                return BadRequest(new
                {
                    mensagem = "A razão social é obrigatória."
                });
            }

            _context.Fornecedores.Add(fornecedor);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetFornecedor),
                new { id = fornecedor.IdFornecedor },
                fornecedor
            );
        }

        // PUT: api/Fornecedores/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFornecedor(
            int id,
            Fornecedor fornecedor)
        {
            if (id != fornecedor.IdFornecedor)
            {
                return BadRequest(new
                {
                    mensagem = "O ID informado é diferente do ID do fornecedor."
                });
            }

            var fornecedorExistente = await _context.Fornecedores
                .FindAsync(id);

            if (fornecedorExistente == null)
            {
                return NotFound(new
                {
                    mensagem = "Fornecedor não encontrado."
                });
            }

            fornecedorExistente.RazaoSocial = fornecedor.RazaoSocial;
            fornecedorExistente.NomeFantasia = fornecedor.NomeFantasia;
            fornecedorExistente.Cnpj = fornecedor.Cnpj;
            fornecedorExistente.Telefone = fornecedor.Telefone;
            fornecedorExistente.Email = fornecedor.Email;
            fornecedorExistente.Endereco = fornecedor.Endereco;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Fornecedores/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFornecedor(int id)
        {
            var fornecedor = await _context.Fornecedores
                .FindAsync(id);

            if (fornecedor == null)
            {
                return NotFound(new
                {
                    mensagem = "Fornecedor não encontrado."
                });
            }

            var possuiProdutos = await _context.Produtos
                .AnyAsync(p => p.IdFornecedor == id);

            if (possuiProdutos)
            {
                return BadRequest(new
                {
                    mensagem = "Não é possível excluir um fornecedor que possui produtos."
                });
            }

            _context.Fornecedores.Remove(fornecedor);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}