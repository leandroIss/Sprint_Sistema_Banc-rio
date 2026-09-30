namespace SistemaBancario.Exceptions;


public class SaldoInsuficienteException : BancoException
{
    public decimal ValorNecessario { get; }
    public decimal SaldoDisponivel { get; }

    public SaldoInsuficienteException(decimal valorNecessario, decimal saldoDisponivel)
        : base($"Saldo insuficiente. Necessário: {Formatador.Moeda(valorNecessario)} | " +
               $"Disponível: {Formatador.Moeda(saldoDisponivel)}.")
    {
        ValorNecessario = valorNecessario;
        SaldoDisponivel = saldoDisponivel;
    }
}