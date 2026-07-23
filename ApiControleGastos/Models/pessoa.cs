namespace ApiControleGastos.Models;

public class Pessoa
{
    // Identificador único gerado automaticamente
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Nome { get; set; } = string.Empty;
    
    public int Idade { get; set; }

    // Relacionamento 1-para-Muitos com Transacoes
    // Ao deletar a Pessoa, as Transações vinculadas serão excluídas (Cascade Delete)
    public List<Transacao> Transacoes { get; set; } = new();
}