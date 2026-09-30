using SistemaBancario.Exceptions;
using SistemaBancario.Interfaces;
using SistemaBancario.Models;

namespace SistemaBancario.Services;

/// <summary>
/// Gerencia o conjunto de contas do banco (composição: o Banco "tem" contas).
/// A lista guarda o tipo base ContaBancaria, então qualquer conta filha
/// pode ser tratada de forma uniforme (polimorfismo).
/// </summary>
public class Banco
{
    private readonly List<ContaBancaria> _contas = new();
    private int _proximoNumero = 1001;

    public IReadOnlyList<ContaBancaria> Contas => _contas.AsReadOnly();

    // ---------- Abertura de contas ----------

    public ContaCorrente AbrirContaCorrente(string titular, decimal saldoInicial)
    {
        var conta = new ContaCorrente(_proximoNumero, titular, saldoInicial);
        Adicionar(conta);
        return conta;
    }

    public ContaPoupanca AbrirContaPoupanca(string titular, decimal saldoInicial)
    {
        var conta = new ContaPoupanca(_proximoNumero, titular, saldoInicial);
        Adicionar(conta);
        return conta;
    }

    public ContaEmpresarial AbrirContaEmpresarial(string titular, decimal saldoInicial, decimal limiteEmprestimo)
    {
        var conta = new ContaEmpresarial(_proximoNumero, titular, saldoInicial, limiteEmprestimo);
        Adicionar(conta);
        return conta;
    }

    private void Adicionar(ContaBancaria conta)
    {
        _contas.Add(conta);
        _proximoNumero++; // só avança se a conta foi criada com sucesso
    }

    // ---------- Consultas ----------

    public ContaBancaria BuscarConta(int numeroConta)
    {
        var conta = _contas.FirstOrDefault(c => c.NumeroConta == numeroConta);
        return conta ?? throw new ContaNaoEncontradaException(numeroConta);
    }

    // ---------- Operações ----------

    public void Transferir(int numeroOrigem, int numeroDestino, decimal valor)
    {
        var origem = BuscarConta(numeroOrigem);
        var destino = BuscarConta(numeroDestino);
        origem.Transferir(destino, valor);
    }

    /// <summary>Aplica rendimento em todas as contas que implementam IRentavel.</summary>
    /// <returns>Lista de (conta, valor rendido).</returns>
    public List<(ContaBancaria Conta, decimal Rendimento)> AplicarRendimentos()
    {
        var resultado = new List<(ContaBancaria, decimal)>();

        foreach (var conta in _contas)
        {
            if (conta is IRentavel rentavel)
                resultado.Add((conta, rentavel.AplicarRendimento()));
        }

        return resultado;
    }

    /// <summary>Encerra uma conta. Só é permitido com saldo exatamente zero.</summary>
    public void EncerrarConta(int numeroConta)
    {
        var conta = BuscarConta(numeroConta);

        if (conta.Saldo != 0)
            throw new OperacaoInvalidaException(
                $"Só é possível encerrar contas com saldo zero (saldo atual: {Formatador.Moeda(conta.Saldo)}).");

        _contas.Remove(conta);
    }
}
