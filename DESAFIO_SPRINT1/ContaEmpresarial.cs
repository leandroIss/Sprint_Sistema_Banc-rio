using SistemaBancario.Exceptions;

namespace SistemaBancario.Models;


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

 
    public override decimal SaldoDisponivel => Saldo + LimiteEmprestimo;


    public decimal EmprestimoUtilizado => Saldo < 0 ? -Saldo : 0m;

    protected override IEnumerable<string> DetalhesEspecificos()
    {
        yield return $"Limite de empréstimo : {Formatador.Moeda(LimiteEmprestimo)}";
        yield return $"Empréstimo utilizado : {Formatador.Moeda(EmprestimoUtilizado)}";
        yield return $"Disponível p/ saque  : {Formatador.Moeda(SaldoDisponivel)}";
    }
}