namespace SistemaBancario.Exceptions;


public class ContaNaoEncontradaException : BancoException
{
    public int NumeroConta { get; }

    public ContaNaoEncontradaException(int numeroConta)
        : base($"Conta número {numeroConta} não encontrada.")
    {
        NumeroConta = numeroConta;
    }
}