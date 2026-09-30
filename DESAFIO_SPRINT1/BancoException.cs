namespace SistemaBancario.Exceptions;


public class BancoException : Exception
{
    public BancoException(string mensagem) : base(mensagem) { }
}