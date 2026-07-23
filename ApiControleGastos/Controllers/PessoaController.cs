using ApiControleGastos.Data;
using ApiControleGastos.DTOs;
using ApiControleGastos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiControleGastos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PessoasController : ControllerBase
{
    private readonly AppDbContext _context;

    public PessoasController(AppDbContext context)
    {
        _context = context;
    }

    // api/pessoas - Lista todas as pessoas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RespostaPessoaDTO>>> GetPessoas()
    {
        var pessoas = await _context.Pessoas
            .Select(p => new RespostaPessoaDTO(p.Id, p.Nome, p.Idade))
            .ToListAsync();

        return Ok(pessoas);
    }

    //  api/pessoas - Cadastra uma nova pessoa
    [HttpPost]
    public async Task<ActionResult<RespostaPessoaDTO>> CriarPessoa([FromBody] CriarPessoaDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            return BadRequest("O nome da pessoa é obrigatório.");

        var pessoa = new Pessoa
        {
            Nome = dto.Nome,
            Idade = dto.Idade
        };

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        var resposta = new RespostaPessoaDTO(pessoa.Id, pessoa.Nome, pessoa.Idade);
        return CreatedAtAction(nameof(GetPessoas), new { id = pessoa.Id }, resposta);
    }

    // DELETE: api/pessoas/{id} - Deleta a pessoa e suas transações vinculadas
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletarPessoa(Guid id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);
        if (pessoa == null)
            return NotFound("Pessoa não encontrada.");

        // Por conta do DeleteBehavior.Cascade configurado no DbContext, 
        // a exclusão da pessoa removerá automaticamente as transações no banco.
        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    //  Retorna consulta de totais por pessoa e geral
    [HttpGet("totais")]
    public async Task<ActionResult<RelatorioTotaisGeraisDTO>> ObterTotais()
    {
        var pessoas = await _context.Pessoas
            .Include(p => p.Transacoes)
            .ToListAsync();

        var listaTotais = pessoas.Select(p =>
        {
            var receitas = p.Transacoes
                .Where(t => t.Tipo == TipoTransacao.Receita)
                .Sum(t => t.Valor);

            var despesas = p.Transacoes
                .Where(t => t.Tipo == TipoTransacao.Despesa)
                .Sum(t => t.Valor);

            return new TotaisPessoaDTO(
                p.Id,
                p.Nome,
                p.Idade,
                receitas,
                despesas,
                receitas - despesas
            );
        }).ToList();

        decimal totalGeralReceitas = listaTotais.Sum(t => t.TotalReceitas);
        decimal totalGeralDespesas = listaTotais.Sum(t => t.TotalDespesas);
        decimal saldoLiquidoGeral = totalGeralReceitas - totalGeralDespesas;

        return Ok(new RelatorioTotaisGeraisDTO(
            listaTotais,
            totalGeralReceitas,
            totalGeralDespesas,
            saldoLiquidoGeral
        ));
    }
}