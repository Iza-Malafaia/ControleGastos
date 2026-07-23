import { useEffect, useState } from 'react';
import { api } from './services/api';
import { TipoTransacao } from './types';
import type { Pessoa, RelatorioTotaisGerais } from './types';
import './index.css';

export function App() {
  const [pessoas, setPessoas] = useState<Pessoa[]>([]);
  const [relatorio, setRelatorio] = useState<RelatorioTotaisGerais | null>(null);

  // Form Pessoa
  const [nome, setNome] = useState('');
  const [idade, setIdade] = useState<number | ''>('');

  // Form Transação
  const [descricao, setDescricao] = useState('');
  const [valor, setValor] = useState<number | ''>('');
  const [tipo, setTipo] = useState<TipoTransacao>(TipoTransacao.Despesa);
  const [pessoaId, setPessoaId] = useState('');

  const [erro, setErro] = useState('');

  // Função para buscar os dados mais recentes da API
  const carregarDados = async () => {
    try {
      const [resPessoas, resTotais] = await Promise.all([
        api.get<Pessoa[]>('/pessoas'),
        api.get<RelatorioTotaisGerais>('/pessoas/totais'),
      ]);
      setPessoas([...resPessoas.data]);
      setRelatorio({ ...resTotais.data });
    } catch (err) {
      console.error('Erro ao buscar dados:', err);
    }
  };

  useEffect(() => {
    carregarDados();
  }, []);

  // 1. Criar Pessoa com aguardo assíncrono
  const handleCriarPessoa = async (e: React.FormEvent) => {
    e.preventDefault();
    setErro('');

    if (!nome || idade === '') return;

    try {
      await api.post('/pessoas', { nome, idade: Number(idade) });
      setNome('');
      setIdade('');
      await carregarDados(); // Aguarda a API persistir antes de atualizar a tela
    } catch (err: any) {
      setErro(err.response?.data || 'Erro ao criar pessoa.');
    }
  };

  // 2. Deletar Pessoa
  const handleDeletarPessoa = async (id: string) => {
    if (!confirm('Tem certeza? Todas as transações desta pessoa também serão apagadas.')) return;

    try {
      await api.delete(`/pessoas/${id}`);
      await carregarDados();
    } catch (err) {
      console.error('Erro ao deletar pessoa:', err);
    }
  };

  // 3. Criar Transação com aguardo assíncrono
  const handleCriarTransacao = async (e: React.FormEvent) => {
    e.preventDefault();
    setErro('');

    if (!descricao || valor === '' || !pessoaId) {
      setErro('Preencha todos os campos da transação.');
      return;
    }

    try {
      await api.post('/transacoes', {
        descricao,
        valor: Number(valor),
        tipo: Number(tipo),
        pessoaId,
      });

      setDescricao('');
      setValor('');
      await carregarDados(); // Aguarda a API persistir antes de atualizar a tela
    } catch (err: any) {
      setErro(err.response?.data || 'Erro ao registrar transação.');
    }
  };

  const pessoaSelecionada = pessoas.find((p) => p.id === pessoaId);

  return (
    <div className="container">
      <h1>Sistema de Controle de Gastos Residenciais</h1>

      {erro && (
        <div style={{ background: '#fef2f2', border: '1px solid #fecaca', color: '#991b1b', padding: '12px 16px', borderRadius: '8px', marginBottom: '20px' }}>
          <strong>Atenção:</strong> {erro}
        </div>
      )}

      {/* PAINEL DE FORMULÁRIOS LADO A LADO */}
      <div className="grid-forms">
        {/* CARD 1: CADASTRO DE PESSOA */}
        <section className="card">
          <h2>Cadastro de Pessoa</h2>
          <form onSubmit={handleCriarPessoa}>
            <div className="form-group">
              <label>Nome:</label>
              <input
                type="text"
                placeholder="Ex: Iza Malafaia"
                value={nome}
                onChange={(e) => setNome(e.target.value)}
                required
              />
            </div>
            <div className="form-group">
              <label>Idade:</label>
              <input
                type="number"
                placeholder="Ex: 22"
                value={idade}
                onChange={(e) => setIdade(e.target.value === '' ? '' : Number(e.target.value))}
                required
              />
            </div>
            <button type="submit">Salvar Pessoa</button>
          </form>
        </section>

        {/* CARD 2: TRANSAÇÃO CADASTRAL */}
        <section className="card">
          <h2>Transação Cadastral</h2>
          <form onSubmit={handleCriarTransacao}>
            <div className="form-group">
              <label>Pessoa:</label>
              <select
                value={pessoaId}
                onChange={(e) => setPessoaId(e.target.value)}
                required
              >
                <option value="">Selecione uma pessoa</option>
                {pessoas.map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.nome} ({p.idade} anos)
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label>Descrição:</label>
              <input
                type="text"
                placeholder="Ex: Aluguel, Mercado"
                value={descricao}
                onChange={(e) => setDescricao(e.target.value)}
                required
              />
            </div>

            <div className="form-group">
              <label>Valor (R$):</label>
              <input
                type="number"
                step="0.01"
                placeholder="0.00"
                value={valor}
                onChange={(e) => setValor(e.target.value === '' ? '' : Number(e.target.value))}
                required
              />
            </div>

            <div className="form-group">
              <label>Tipo:</label>
              <select
                value={tipo}
               onChange={(e) => setTipo(Number(e.target.value) as TipoTransacao)}
              >
                <option value={TipoTransacao.Despesa}>Despesa</option>
                <option
                  value={TipoTransacao.Receita}
                  disabled={pessoaSelecionada ? pessoaSelecionada.idade < 18 : false}
                >
                  Receita {pessoaSelecionada && pessoaSelecionada.idade < 18 ? '(Bloqueado < 18)' : ''}
                </option>
              </select>
            </div>

            <button type="submit">Salvar Transação</button>
          </form>
        </section>
      </div>

      {/* CARD 3: CONSULTA DE TOTAIS */}
      <section className="card">
        <h2>Consulta de Totais por Pessoa</h2>
        
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Nome</th>
                <th>Idade</th>
                <th>Receitas totais</th>
                <th>Total Despesas</th>
                <th>Saldo</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              {relatorio?.pessoas.map((p) => (
                <tr key={p.id}>
                  <td><strong>{p.nome}</strong></td>
                  <td>{p.idade}</td>
                  <td className="val-receita">R$ {p.totalReceitas.toFixed(2)}</td>
                  <td className="val-despesa">R$ {p.totalDespesas.toFixed(2)}</td>
                  <td className="val-saldo">R$ {p.saldo.toFixed(2)}</td>
                  <td>
                    <button
                      className="btn-danger"
                      onClick={() => handleDeletarPessoa(p.id)}
                    >
                      Excluir
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* TOTAL GERAL CONSOLIDADO */}
        {relatorio && (
          <div className="consolidado-card">
            <h3>Total Geral Consolidado</h3>
            <div className="consolidado-metrics">
              <div className="metric-item">
                <span className="metric-label">Total de Receitas</span>
                <span className="metric-value val-receita">R$ {relatorio.totalGeralReceitas.toFixed(2)}</span>
              </div>
              <div className="metric-item">
                <span className="metric-label">Total de Despesas</span>
                <span className="metric-value val-despesa">R$ {relatorio.totalGeralDespesas.toFixed(2)}</span>
              </div>
              <div className="metric-item">
                <span className="metric-label">Saldo Líquido Geral</span>
                <span className="metric-value val-saldo">R$ {relatorio.saldoLiquidoGeral.toFixed(2)}</span>
              </div>
            </div>
          </div>
        )}
      </section>
    </div>
  );
}

export default App;