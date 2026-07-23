namespace ApiControleGastos.Models;

public enum TipoTransacao
{
    Despesa = 0,
    Receita = 1
}

public class Transacao
{
    // Identificador único gerado automaticamente
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Descricao { get; set; } = string.Empty;
    
    public decimal Valor { get; set; }
    
    public TipoTransacao Tipo { get; set; }

    // Chave Estrangeira apontando para Pessoa
    public Guid PessoaId { get; set; }
    
    public Pessoa? Pessoa { get; set; }
}