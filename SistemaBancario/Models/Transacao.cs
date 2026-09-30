namespace SistemaBancario.Models;

/// <summary>Registro imutável de uma movimentação na conta (usado no extrato).</summary>
public sealed record Transacao(
    DateTime Data,
    TipoTransacao Tipo,
    decimal Valor,
    decimal SaldoApos,
    string Descricao);
