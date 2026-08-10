using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaEstoque.Data;
using SistemaEstoque.Models;

namespace SistemaEstoque.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentacoesEstoqueController : ControllerBase
    {
        private readonly EstoqueDbContext _context;

        public MovimentacoesEstoqueController(EstoqueDbContext context)
        {
            _context = context;
        }

        // GET: api/MovimentacoesEstoque
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimentacaoEstoque>>>
            GetMovimentacoes()
        {
            return await _context.MovimentacoesEstoque
                .Include(m => m.Produto)
                .Include(m => m.Usuario)
                .AsNoTracking()
                .OrderByDescending(m => m.DataMovimentacao)
                .ToListAsync();
        }

        // GET: api/MovimentacoesEstoque/1
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimentacaoEstoque>>
            GetMovimentacao(int id)
        {
            var movimentacao = await _context.MovimentacoesEstoque
                .Include(m => m.Produto)
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.IdMovimentacao == id);

            if (movimentacao == null)
                return NotFound(new
                {
                    mensagem = "Movimentação não encontrada."
                });

            return movimentacao;
        }

        // POST: api/MovimentacoesEstoque
        [HttpPost]
        public async Task<ActionResult<MovimentacaoEstoque>>
            PostMovimentacao(MovimentacaoEstoque movimentacao)
        {
            var produto = await _context.Produtos
                .FindAsync(movimentacao.IdProduto);

            if (produto == null)
                return BadRequest(new
                {
                    mensagem = "Produto não encontrado."
                });

            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.IdUsuario == movimentacao.IdUsuario);

            if (!usuarioExiste)
                return BadRequest(new
                {
                    mensagem = "Usuário não encontrado."
                });

            if (movimentacao.Quantidade <= 0)
                return BadRequest(new
                {
                    mensagem = "A quantidade deve ser maior que zero."
                });

            if (movimentacao.Tipo.ToUpper() != "ENTRADA" &&
                movimentacao.Tipo.ToUpper() != "SAIDA")
            {
                return BadRequest(new
                {
                    mensagem = "O tipo deve ser ENTRADA ou SAIDA."
                });
            }

            if (movimentacao.Tipo.ToUpper() == "ENTRADA")
            {
                produto.Estoque += movimentacao.Quantidade;
                movimentacao.Tipo = "ENTRADA";
            }
            else
            {
                if (produto.Estoque < movimentacao.Quantidade)
                {
                    return BadRequest(new
                    {
                        mensagem = "Estoque insuficiente para realizar a saída."
                    });
                }

                produto.Estoque -= movimentacao.Quantidade;
                movimentacao.Tipo = "SAIDA";
            }

            movimentacao.DataMovimentacao = DateTime.Now;

            _context.MovimentacoesEstoque.Add(movimentacao);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMovimentacao),
                new { id = movimentacao.IdMovimentacao },
                movimentacao
            );
        }

        // DELETE: api/MovimentacoesEstoque/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimentacao(int id)
        {
            var movimentacao = await _context.MovimentacoesEstoque
                .FirstOrDefaultAsync(m => m.IdMovimentacao == id);

            if (movimentacao == null)
                return NotFound(new
                {
                    mensagem = "Movimentação não encontrada."
                });

            var produto = await _context.Produtos
                .FindAsync(movimentacao.IdProduto);

            if (produto != null)
            {
                if (movimentacao.Tipo == "ENTRADA")
                {
                    if (produto.Estoque < movimentacao.Quantidade)
                    {
                        return BadRequest(new
                        {
                            mensagem = "Não é possível desfazer esta entrada porque o estoque atual é insuficiente."
                        });
                    }

                    produto.Estoque -= movimentacao.Quantidade;
                }
                else if (movimentacao.Tipo == "SAIDA")
                {
                    produto.Estoque += movimentacao.Quantidade;
                }
            }

            _context.MovimentacoesEstoque.Remove(movimentacao);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}