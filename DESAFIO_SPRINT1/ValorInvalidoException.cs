namespace SistemaBancario.Exceptions;


public class ValorInvalidoException : BancoException
{
    public ValorInvalidoException(string mensagem) : base(mensagem) { }
}