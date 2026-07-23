import axios from 'axios';

// URL da nossa API .NET rodando na porta 5233
export const api = axios.create({
  baseURL: 'http://localhost:5233/api',
});