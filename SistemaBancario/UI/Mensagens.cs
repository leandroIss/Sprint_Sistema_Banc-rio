namespace SistemaBancario.UI;

/// <summary>Mensagens coloridas de sucesso, erro e aviso.</summary>
public static class Mensagens
{
    public static void Sucesso(string texto) => Escrever(texto, ConsoleColor.Green);
    public static void Erro(string texto) => Escrever("Erro: " + texto, ConsoleColor.Red);
    public static void Aviso(string texto) => Escrever(texto, ConsoleColor.Yellow);

    public static void Titulo(string texto)
    {
        Console.WriteLine();
        Escrever(new string('=', 60), ConsoleColor.Cyan);
        Escrever($"  {texto}", ConsoleColor.Cyan);
        Escrever(new string('=', 60), ConsoleColor.Cyan);
    }

    private static void Escrever(string texto, ConsoleColor cor)
    {
        var anterior = Console.ForegroundColor;
        Console.ForegroundColor = cor;
        Console.WriteLine(texto);
        Console.ForegroundColor = anterior;
    }
}
