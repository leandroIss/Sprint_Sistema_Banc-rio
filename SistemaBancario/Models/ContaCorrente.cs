namespace SistemaBancario.Models;

/// <summary>Conta corrente: cobra uma taxa fixa a cada saque.</summary>
public class ContaCorrente : ContaBancaria
{
    /// <summary>Taxa fixa cobrada por saque.</summary>
    public const decimal TaxaPorSaque = 2.50m;

    public ContaCorrente(int numeroConta, string titular, decimal saldoInicial = 0m)
        : base(numeroConta, titular, saldoInicial) { }

    public override string TipoConta => "Corrente";

    protected override decimal CalcularTaxaSaque() => TaxaPorSaque;

    protected override IEnumerable<string> DetalhesEspecificos()
    {
        yield return $"Taxa de saque: {Formatador.Moeda(TaxaPorSaque)} por saque";
    }
}
