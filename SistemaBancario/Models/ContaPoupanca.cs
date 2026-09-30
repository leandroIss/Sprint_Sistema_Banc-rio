using SistemaBancario.Exceptions;
using SistemaBancario.Interfaces;

namespace SistemaBancario.Models;

/// <summary>Conta poupança: sem taxa de saque e com rendimento (interface IRentavel).</summary>
public class ContaPoupanca : ContaBancaria, IRentavel
{
    /// <summary>Rendimento mensal padrão (0,5%).</summary>
    public const decimal TaxaRendimentoPadrao = 0.005m;

    public decimal TaxaRendimento { get; }

    public ContaPoupanca(int numeroConta, string titular, decimal saldoInicial = 0m,
                         decimal taxaRendimento = TaxaRendimentoPadrao)
        : base(numeroConta, titular, saldoInicial)
    {
        if (taxaRendimento < 0 || taxaRendimento > 1)
            throw new ValorInvalidoException("A taxa de rendimento deve estar entre 0% e 100%.");
        TaxaRendimento = taxaRendimento;
    }

    public override string TipoConta => "Poupança";

    // Poupança não cobra taxa: usa o CalcularTaxaSaque() padrão (0) da classe pai.

    public decimal AplicarRendimento()
    {
        if (Saldo <= 0)
            return 0m;

        decimal rendimento = Math.Round(Saldo * TaxaRendimento, 2, MidpointRounding.AwayFromZero);
        if (rendimento <= 0)
            return 0m;

        Saldo += rendimento;
        Registrar(TipoTransacao.Rendimento, rendimento,
            $"Rendimento de {Formatador.Percentual(TaxaRendimento)}");
        return rendimento;
    }

    protected override IEnumerable<string> DetalhesEspecificos()
    {
        yield return "Taxa de saque: isenta";
        yield return $"Rendimento   : {Formatador.Percentual(TaxaRendimento)} por período";
    }
}
