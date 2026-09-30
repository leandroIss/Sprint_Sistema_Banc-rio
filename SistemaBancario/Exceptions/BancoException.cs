namespace SistemaBancario.Exceptions;

/// <summary>
/// Exceção base para todos os erros de regra de negócio do banco.
/// Permite capturar qualquer erro "esperado" do domínio com um único catch.
/// </summary>
public class BancoException : Exception
{
    public BancoException(string mensagem) : base(mensagem) { }
}
