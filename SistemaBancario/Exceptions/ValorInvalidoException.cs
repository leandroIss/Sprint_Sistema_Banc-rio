namespace SistemaBancario.Exceptions;

/// <summary>Lançada quando um valor informado não é aceitável (zero, negativo, etc.).</summary>
public class ValorInvalidoException : BancoException
{
    public ValorInvalidoException(string mensagem) : base(mensagem) { }
}
