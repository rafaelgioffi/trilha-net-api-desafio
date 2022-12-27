using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            
            var tarefa = _context.Tarefas.Find(id);
            if(tarefa == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(tarefa);
            }
        }

        [HttpGet("ObterTodos")]
        public async Task<IActionResult> ObterTodos()
        {
            var tarefa = await _context.Tarefas.ToListAsync();
            return !tarefa.Any() ? NotFound("Nenhuma tarefa cadastrada") : Ok(tarefa);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            var tarefa = _context.Tarefas.Where(t => t.Titulo.Contains(titulo));
            
            return !tarefa.Any() ? NotFound("Tarefa não encontrada") : Ok(tarefa);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);
            
            return !tarefa.Any() ? NotFound($"Data {data} não encontrada") : Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {            
            var tarefa = _context.Tarefas.Where(x => x.Status == status);

            return !tarefa.Any() ? NotFound($"Nenhuma tarefa com status {status}") : Ok(tarefa);
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            if (Enum.GetName(tarefa.Status) == "Pendente" || Enum.GetName(tarefa.Status) == "Finalizado")
                return BadRequest(new { Erro = "Status pode ser apenas Pendente ou Finalizado" });
                        
            await _context.AddAsync(tarefa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = await _context.Tarefas.FindAsync(id);

            if (tarefaBanco == null)
                return NotFound("Falha ao atualizar");

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            _context.Tarefas.Update(tarefaBanco);
            await _context.SaveChangesAsync();

            return Ok(tarefa);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var tarefaBanco = await _context.Tarefas.FindAsync(id);

            if (tarefaBanco == null)
                return NotFound("Falha ao excluir a tarefa");

            _context.Tarefas.Remove(tarefaBanco);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
