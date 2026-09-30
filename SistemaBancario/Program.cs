using System.Text;
using SistemaBancario.Services;
using SistemaBancario.UI;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var banco = new Banco();

// Contas de demonstração para facilitar os testes (contas 1001, 1002 e 1003).
banco.AbrirContaCorrente("Ana Souza", 1000m);
banco.AbrirContaPoupanca("Bruno Lima", 2000m);
banco.AbrirContaEmpresarial("Padaria Sol LTDA", 500m, 3000m);

try
{
    new Menu(banco).Executar();
}
catch (EndOfStreamException)
{
    Console.WriteLine("\nEntrada encerrada. Finalizando o sistema.");
}
catch (Exception ex)
{
    // Rede de segurança: erro inesperado nunca aparece como "crash" para o usuário.
    Mensagens.Erro($"Erro inesperado: {ex.Message}");
    Environment.ExitCode = 1;
}
