export interface Pessoa {
  id: string;
  nome: string;
  idade: number;
}

// Usando 'as const' em vez de 'enum' para evitar o erro ts(1294)
export const TipoTransacao = {
  Despesa: 0,
  Receita: 1,
} as const;

export type TipoTransacao = (typeof TipoTransacao)[keyof typeof TipoTransacao];

export interface Transacao {
  id: string;
  descricao: string;
  valor: number;
  tipo: TipoTransacao;
  pessoaId: string;
}

export interface TotaisPessoa {
  id: string;
  nome: string;
  idade: number;
  totalReceitas: number;
  totalDespesas: number;
  saldo: number;
}

export interface RelatorioTotaisGerais {
  pessoas: TotaisPessoa[];
  totalGeralReceitas: number;
  totalGeralDespesas: number;
  saldoLiquidoGeral: number;
}