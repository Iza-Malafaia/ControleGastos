using ApiControleGastos.Data;
using ApiControleGastos.DTOs;
using ApiControleGastos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiControleGastos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly AppDbContext _context;

    public TransacoesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/transacoes - Listagem de transações
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RespostaTransacaoDTO>>> GetTransacoes()
    {
        var transacoes = await _context.Transacoes
            .Select(t => new RespostaTransacaoDTO(t.Id, t.Descricao, t.Valor, t.Tipo, t.PessoaId))
            .ToListAsync();

        return Ok(transacoes);
    }

    // POST: api/transacoes - Cadastro com regra de negócio para menores de idade
    [HttpPost]
    public async Task<ActionResult<RespostaTransacaoDTO>> CriarTransacao([FromBody] CriarTransacaoDTO dto)
    {
        // 1. Validação de existência da pessoa no cadastro
        var pessoa = await _context.Pessoas.FindAsync(dto.PessoaId);
        if (pessoa == null)
            return BadRequest("Pessoa informada não existe no cadastro.");

        // 2. Regra de negócio: menor de 18 anos só pode registrar DESPESAS
        if (pessoa.Idade < 18 && dto.Tipo == TipoTransacao.Receita)
        {
            return BadRequest("Pessoas menores de 18 anos só podem ter transações do tipo DESPESA.");
        }

        var transacao = new Transacao
        {
            Descricao = dto.Descricao,
            Valor = dto.Valor,
            Tipo = dto.Tipo,
            PessoaId = dto.PessoaId
        };

        _context.Transacoes.Add(transacao);
        await _context.SaveChangesAsync();

        var resposta = new RespostaTransacaoDTO(
            transacao.Id,
            transacao.Descricao,
            transacao.Valor,
            transacao.Tipo,
            transacao.PessoaId
        );

        return CreatedAtAction(nameof(GetTransacoes), new { id = transacao.Id }, resposta);
    }
}