using System.Globalization;

namespace SistemaBancario.UI;


public static class Entrada
{
    private static string LerLinha(string mensagem)
    {
        Console.Write(mensagem);
        string? linha = Console.ReadLine();


        if (linha is null)
            throw new EndOfStreamException("Entrada de dados encerrada.");

        return linha.Trim();
    }

    public static string LerTexto(string mensagem)
    {
        while (true)
        {
            string texto = LerLinha(mensagem);
            if (!string.IsNullOrWhiteSpace(texto))
                return texto;

            Mensagens.Erro("O texto não pode ser vazio.");
        }
    }

    public static int LerInteiro(string mensagem)
    {
        while (true)
        {
            string texto = LerLinha(mensagem);
            if (int.TryParse(texto, NumberStyles.None, CultureInfo.InvariantCulture, out int valor))
                return valor;

            Mensagens.Erro("Entrada inválida. Digite um número inteiro (ex.: 1001).");
        }
    }


    public static decimal LerDecimal(string mensagem)
    {
        while (true)
        {
            string texto = LerLinha(mensagem).Replace(',', '.');
            if (decimal.TryParse(texto, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal valor))
                return valor;

            Mensagens.Erro("Valor inválido. Digite apenas números (ex.: 150,75).");
        }
    }

    public static bool Confirmar(string mensagem)
    {
        while (true)
        {
            string resposta = LerLinha($"{mensagem} (S/N): ").ToUpperInvariant();
            if (resposta is "S" or "SIM") return true;
            if (resposta is "N" or "NAO" or "NÃO") return false;

            Mensagens.Erro("Responda com S ou N.");
        }
    }

    public static void AguardarTecla()
    {
        Console.Write("\nPressione ENTER para continuar...");
        if (Console.ReadLine() is null)
            throw new EndOfStreamException("Entrada de dados encerrada.");
    }
}