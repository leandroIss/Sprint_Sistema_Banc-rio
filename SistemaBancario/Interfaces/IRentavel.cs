namespace SistemaBancario.Interfaces;

/// <summary>
/// Contrato para contas que geram rendimento sobre o saldo (ex.: poupança).
/// </summary>
public interface IRentavel
{
    /// <summary>Taxa de rendimento por período (0,005 = 0,5%).</summary>
    decimal TaxaRendimento { get; }

    /// <summary>Aplica o rendimento ao saldo e devolve o valor rendido.</summary>
    decimal AplicarRendimento();
}
