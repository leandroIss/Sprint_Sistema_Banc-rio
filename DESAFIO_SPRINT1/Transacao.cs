namespace SistemaBancario.Models;


public sealed record Transacao(
    DateTime Data,
    TipoTransacao Tipo,
    decimal Valor,
    decimal SaldoApos,
    string Descricao);