namespace SistemaBancario.Exceptions;

/// <summary>Lançada quando a operação viola uma regra (ex.: transferir para a mesma conta).</summary>
public class OperacaoInvalidaException : BancoException
{
    public OperacaoInvalidaException(string mensagem) : base(mensagem) { }
}
