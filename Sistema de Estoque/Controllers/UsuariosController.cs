using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public UsuariosController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/Usuarios/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Movimentacoes)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            return usuario;
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nome) ||
                string.IsNullOrWhiteSpace(usuario.Login) ||
                string.IsNullOrWhiteSpace(usuario.Senha))
            {
                return BadRequest(new
                {
                    mensagem = "Nome, login e senha são obrigatórios."
                });
            }

            var loginExiste = await _context.Usuarios
                .AnyAsync(u => u.Login == usuario.Login);

            if (loginExiste)
                return Conflict(new
                {
                    mensagem = "Este login já está cadastrado."
                });

            _context.Usuarios.Add(usuario);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetUsuario),
                new { id = usuario.IdUsuario },
                usuario
            );
        }

        // PUT: api/Usuarios/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(
            int id,
            Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return BadRequest(new { mensagem = "ID inválido." });

            var existente = await _context.Usuarios.FindAsync(id);

            if (existente == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            var loginExiste = await _context.Usuarios
                .AnyAsync(u =>
                    u.Login == usuario.Login &&
                    u.IdUsuario != id);

            if (loginExiste)
                return Conflict(new
                {
                    mensagem = "Este login já está sendo utilizado."
                });

            existente.Nome = usuario.Nome;
            existente.Login = usuario.Login;
            existente.Senha = usuario.Senha;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Usuarios/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            var possuiMovimentacoes = await _context.MovimentacoesEstoque
                .AnyAsync(m => m.IdUsuario == id);

            if (possuiMovimentacoes)
                return BadRequest(new
                {
                    mensagem = "Não é possível excluir um usuário que possui movimentações."
                });

            _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}