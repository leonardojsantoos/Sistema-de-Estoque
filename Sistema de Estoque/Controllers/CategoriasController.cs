using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public CategoriasController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categoria>>> GetCategorias()
        {
            return await _context.Categorias
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/Categorias/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.IdCategoria == id);

            if (categoria == null)
                return NotFound(new
                {
                    mensagem = "Categoria não encontrada."
                });

            return categoria;
        }

        // POST: api/Categorias
        [HttpPost]
        public async Task<ActionResult<Categoria>> PostCategoria(
            Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nome))
            {
                return BadRequest(new
                {
                    mensagem = "O nome da categoria é obrigatório."
                });
            }

            _context.Categorias.Add(categoria);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategoria),
                new { id = categoria.IdCategoria },
                categoria
            );
        }

        // PUT: api/Categorias/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(
            int id,
            Categoria categoria)
        {
            if (id != categoria.IdCategoria)
            {
                return BadRequest(new
                {
                    mensagem = "O ID informado é diferente do ID da categoria."
                });
            }

            var categoriaExistente = await _context.Categorias
                .FindAsync(id);

            if (categoriaExistente == null)
            {
                return NotFound(new
                {
                    mensagem = "Categoria não encontrada."
                });
            }

            categoriaExistente.Nome = categoria.Nome;
            categoriaExistente.Descricao = categoria.Descricao;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Categorias/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias
                .FindAsync(id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensagem = "Categoria não encontrada."
                });
            }

            var possuiProdutos = await _context.Produtos
                .AnyAsync(p => p.IdCategoria == id);

            if (possuiProdutos)
            {
                return BadRequest(new
                {
                    mensagem = "Não é possível excluir uma categoria que possui produtos."
                });
            }

            _context.Categorias.Remove(categoria);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}