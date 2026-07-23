# 💰 Sistema de Controle de Gastos Residenciais

Sistema web desenvolvido para gerenciamento e controle de transações financeiras (receitas e despesas) por pessoa de uma residência.

---

## 🛠️ Tecnologias Utilizadas

- **Back-end:** .NET 8 (C#) Web API, Entity Framework Core, SQLite
- **Front-end:** React, TypeScript, Vite, Axios, CSS3
- **Controle de Versão:** Git e GitHub

---

## 📌 Regras de Negócio Implementadas

1. **Gestão de Pessoas:** Cadastro, listagem e remoção de moradores/pessoas.
2. **Controle de Idade:** Bloqueio de cadastro de receitas para menores de 18 anos.
3. **Múltiplos Lançamentos:** Uma pessoa pode ter várias despesas/receitas vinculadas.
4. **Exclusão em Cascata:** Ao remover uma pessoa, todas as suas transações vinculadas são apagadas.
5. **Relatórios Financeiros:** Exibição detalhada por pessoa e consolidação do total geral do sistema.

---

## 🚀 Como Executar o Projeto Localmente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (versão 18 ou superior)

---

### 1. Clonar o Repositório

git clone https://github.com/Iza-Malafaia/ControleGastos.git

### 2. Executar o Back-end (.NET Web API)
# Navegar até a pasta da API
cd ApiControleGastos

# Executar a aplicação  
dotnet run

#A API estará rodando em: http://localhost:5233


### 3. Executar o Front-end (React + Vite) 
em outro terminal execute: 

cd ControleGastos/frontend
# Instalar as dependências
npm install

# Iniciar o servidor de desenvolvimento
npm run dev

#O front-end estará rodando em: http://localhost:5173


```bash
