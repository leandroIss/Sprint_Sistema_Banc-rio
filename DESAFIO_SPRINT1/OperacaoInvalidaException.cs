namespace SistemaBancario.Exceptions;

public class OperacaoInvalidaException : BancoException
{
    public OperacaoInvalidaException(string mensagem) : base(mensagem) { }
}