using System.Text;
using SistemaBancario.Services;
using SistemaBancario.UI;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var banco = new Banco();


banco.AbrirContaCorrente("Gioavana Gusmão", 1000m);
banco.AbrirContaPoupanca("Vitoria Alvez", 2000m);
banco.AbrirContaEmpresarial("JayJay bank", 500m, 3000m);

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
  
    Mensagens.Erro($"Erro inesperado: {ex.Message}");
    Environment.ExitCode = 1;
}