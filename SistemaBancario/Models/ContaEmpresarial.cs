using SistemaBancario.Exceptions;

namespace SistemaBancario.Models;

/// <summary>
/// Conta empresarial: possui um limite de empréstimo extra, que permite
/// ao saldo ficar negativo até o limite contratado.
/// </summary>
public class ContaEmpresarial : ContaBancaria
{
    public decimal LimiteEmprestimo { get; }

    public ContaEmpresarial(int numeroConta, string titular, decimal saldoInicial = 0m,
                            decimal limiteEmprestimo = 5000m)
        : base(numeroConta, titular, saldoInicial)
    {
        if (limiteEmprestimo < 0)
            throw new ValorInvalidoException("O limite de empréstimo não pode ser negativo.");
        LimiteEmprestimo = limiteEmprestimo;
    }

    public override string TipoConta => "Empresarial";

    /// <summary>Saldo + limite de empréstimo ainda não utilizado.</summary>
    public override decimal SaldoDisponivel => Saldo + LimiteEmprestimo;

    /// <summary>Quanto do empréstimo está em uso (saldo negativo).</summary>
    public decimal EmprestimoUtilizado => Saldo < 0 ? -Saldo : 0m;

    protected override IEnumerable<string> DetalhesEspecificos()
    {
        yield return $"Limite de empréstimo : {Formatador.Moeda(LimiteEmprestimo)}";
        yield return $"Empréstimo utilizado : {Formatador.Moeda(EmprestimoUtilizado)}";
        yield return $"Disponível p/ saque  : {Formatador.Moeda(SaldoDisponivel)}";
    }
}
