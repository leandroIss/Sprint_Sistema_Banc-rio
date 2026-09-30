namespace SistemaBancario.Exceptions;

/// <summary>Lançada quando o número de conta informado não existe no banco.</summary>
public class ContaNaoEncontradaException : BancoException
{
    public int NumeroConta { get; }

    public ContaNaoEncontradaException(int numeroConta)
        : base($"Conta número {numeroConta} não encontrada.")
    {
        NumeroConta = numeroConta;
    }
}
