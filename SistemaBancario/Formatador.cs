using System.Globalization;

namespace SistemaBancario;

/// <summary>
/// Utilitário de formatação no padrão brasileiro (R$ 1.234,56 e dd/MM/yyyy).
/// Usa um NumberFormatInfo próprio para funcionar em qualquer ambiente,
/// mesmo quando a cultura pt-BR não está instalada no sistema.
/// </summary>
public static class Formatador
{
    private static readonly NumberFormatInfo FormatoBr = new()
    {
        NumberDecimalSeparator = ",",
        NumberGroupSeparator = "."
    };

    public static string Moeda(decimal valor)
    {
        string sinal = valor < 0 ? "-" : "";
        return $"{sinal}R$ {Math.Abs(valor).ToString("N2", FormatoBr)}";
    }

    public static string Percentual(decimal taxa) =>
        (taxa * 100).ToString("0.##", FormatoBr) + "%";

    public static string DataHora(DateTime data) =>
        data.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
}
