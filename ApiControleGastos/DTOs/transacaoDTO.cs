using ApiControleGastos.Models;

namespace ApiControleGastos.DTOs;

public record CriarTransacaoDTO(
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    Guid PessoaId
);

public record RespostaTransacaoDTO(
    Guid Id,
    string Descricao,
    decimal Valor,
    TipoTransacao Tipo,
    Guid PessoaId
);