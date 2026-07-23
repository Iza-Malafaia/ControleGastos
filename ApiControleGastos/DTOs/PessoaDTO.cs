namespace ApiControleGastos.DTOs;

public record CriarPessoaDTO(string Nome, int Idade);

public record RespostaPessoaDTO(Guid Id, string Nome, int Idade);

public record TotaisPessoaDTO(
    Guid Id,
    string Nome,
    int Idade,
    decimal TotalReceitas,
    decimal TotalDespesas,
    decimal Saldo
);

public record RelatorioTotaisGeraisDTO(
    List<TotaisPessoaDTO> Pessoas,
    decimal TotalGeralReceitas,
    decimal TotalGeralDespesas,
    decimal SaldoLiquidoGeral
);