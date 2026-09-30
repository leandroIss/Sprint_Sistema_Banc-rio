using SistemaBancario.Exceptions;
using SistemaBancario.Models;
using SistemaBancario.Services;

namespace SistemaBancario.UI;


public class Menu
{
    private readonly Banco _banco;

    public Menu(Banco banco)
    {
        _banco = banco;
    }

    public void Executar()
    {
        bool sair = false;

        while (!sair)
        {
            ExibirMenuPrincipal();
            int opcao = Entrada.LerInteiro("Escolha uma opção: ");


            try
            {
                switch (opcao)
                {
                    case 1: AbrirConta(); break;
                    case 2: Depositar(); break;
                    case 3: Sacar(); break;
                    case 4: Transferir(); break;
                    case 5: ConsultarConta(); break;
                    case 6: ExibirExtrato(); break;
                    case 7: ListarContas(); break;
                    case 8: AplicarRendimentos(); break;
                    case 9: EncerrarConta(); break;
                    case 0: sair = true; break;
                    default: Mensagens.Erro("Opção inexistente. Escolha um número do menu."); break;
                }
            }
            catch (BancoException ex)
            {
                Mensagens.Erro(ex.Message);
            }

            if (!sair)
                Entrada.AguardarTecla();
        }

        Mensagens.Sucesso("\nObrigado por usar o Sistema Bancário. Até logo!");
    }

    private static void ExibirMenuPrincipal()
    {
        Mensagens.Titulo("SISTEMA BANCÁRIO");
        Console.WriteLine("  1 - Abrir conta");
        Console.WriteLine("  2 - Depositar");
        Console.WriteLine("  3 - Sacar");
        Console.WriteLine("  4 - Transferir");
        Console.WriteLine("  5 - Consultar conta");
        Console.WriteLine("  6 - Extrato");
        Console.WriteLine("  7 - Listar todas as contas");
        Console.WriteLine("  8 - Aplicar rendimento (poupanças)");
        Console.WriteLine("  9 - Encerrar conta");
        Console.WriteLine("  0 - Sair");
        Console.WriteLine(new string('-', 60));
    }

 

    private void AbrirConta()
    {
        Mensagens.Titulo("ABRIR CONTA");
        Console.WriteLine("  1 - Conta Corrente   (taxa de " + Formatador.Moeda(ContaCorrente.TaxaPorSaque) + " por saque)");
        Console.WriteLine("  2 - Conta Poupança   (sem taxa, rende " + Formatador.Percentual(ContaPoupanca.TaxaRendimentoPadrao) + " por período)");
        Console.WriteLine("  3 - Conta Empresarial (limite de empréstimo extra)");

        int tipo = Entrada.LerInteiro("Tipo de conta: ");
        if (tipo is < 1 or > 3)
        {
            Mensagens.Erro("Tipo de conta inválido.");
            return;
        }

        string titular = Entrada.LerTexto("Nome do titular: ");
        decimal saldoInicial = Entrada.LerDecimal("Depósito inicial (R$): ");

        ContaBancaria conta;
        switch (tipo)
        {
            case 1:
                conta = _banco.AbrirContaCorrente(titular, saldoInicial);
                break;
            case 2:
                conta = _banco.AbrirContaPoupanca(titular, saldoInicial);
                break;
            default:
                decimal limite = Entrada.LerDecimal("Limite de empréstimo (R$): ");
                conta = _banco.AbrirContaEmpresarial(titular, saldoInicial, limite);
                break;
        }

        Mensagens.Sucesso($"\nConta aberta com sucesso! Número da conta: {conta.NumeroConta}");
        Console.WriteLine(conta.ObterDetalhes());
    }

    private void Depositar()
    {
        Mensagens.Titulo("DEPÓSITO");
        var conta = _banco.BuscarConta(Entrada.LerInteiro("Número da conta: "));
        decimal valor = Entrada.LerDecimal("Valor do depósito (R$): ");

        conta.Depositar(valor);
        Mensagens.Sucesso($"Depósito realizado. Novo saldo: {Formatador.Moeda(conta.Saldo)}");
    }

    private void Sacar()
    {
        Mensagens.Titulo("SAQUE");
        var conta = _banco.BuscarConta(Entrada.LerInteiro("Número da conta: "));
        Console.WriteLine($"Disponível para saque: {Formatador.Moeda(conta.SaldoDisponivel)}");
        decimal valor = Entrada.LerDecimal("Valor do saque (R$): ");

        conta.Sacar(valor);
        Mensagens.Sucesso($"Saque realizado. Novo saldo: {Formatador.Moeda(conta.Saldo)}");
    }

    private void Transferir()
    {
        Mensagens.Titulo("TRANSFERÊNCIA");
        int numeroOrigem = Entrada.LerInteiro("Conta de origem : ");
        int numeroDestino = Entrada.LerInteiro("Conta de destino: ");
        decimal valor = Entrada.LerDecimal("Valor (R$): ");

        _banco.Transferir(numeroOrigem, numeroDestino, valor);

        var origem = _banco.BuscarConta(numeroOrigem);
        Mensagens.Sucesso($"Transferência realizada. Saldo da conta {origem.NumeroConta}: {Formatador.Moeda(origem.Saldo)}");
    }

    private void ConsultarConta()
    {
        Mensagens.Titulo("CONSULTAR CONTA");
        var conta = _banco.BuscarConta(Entrada.LerInteiro("Número da conta: "));
        Console.WriteLine();
        Console.WriteLine(conta.ObterDetalhes());
    }

    private void ExibirExtrato()
    {
        Mensagens.Titulo("EXTRATO");
        var conta = _banco.BuscarConta(Entrada.LerInteiro("Número da conta: "));

        Console.WriteLine($"\nConta {conta.NumeroConta} - {conta.Titular} ({conta.TipoConta})");
        Console.WriteLine(new string('-', 78));
        Console.WriteLine($"{"Data/Hora",-20} {"Operação",-24} {"Valor",16} {"Saldo",16}");
        Console.WriteLine(new string('-', 78));

        foreach (var t in conta.Historico)
        {
            Console.WriteLine($"{Formatador.DataHora(t.Data),-20} {DescreverTipo(t.Tipo),-24} " +
                              $"{Formatador.Moeda(t.Valor),16} {Formatador.Moeda(t.SaldoApos),16}");
        }

        Console.WriteLine(new string('-', 78));
        Console.WriteLine($"Saldo atual: {Formatador.Moeda(conta.Saldo)}");
    }

    private void ListarContas()
    {
        Mensagens.Titulo("CONTAS DO BANCO");

        if (_banco.Contas.Count == 0)
        {
            Mensagens.Aviso("Nenhuma conta cadastrada.");
            return;
        }

        foreach (var conta in _banco.Contas)
            Console.WriteLine(conta);

        Console.WriteLine($"\nTotal: {_banco.Contas.Count} conta(s).");
    }

    private void AplicarRendimentos()
    {
        Mensagens.Titulo("RENDIMENTO DAS POUPANÇAS");
        var resultados = _banco.AplicarRendimentos();

        if (resultados.Count == 0)
        {
            Mensagens.Aviso("Não há contas poupança cadastradas.");
            return;
        }

        foreach (var (conta, rendimento) in resultados)
            Console.WriteLine($"Conta {conta.NumeroConta} ({conta.Titular}): +{Formatador.Moeda(rendimento)} " +
                              $"=> saldo {Formatador.Moeda(conta.Saldo)}");

        Mensagens.Sucesso("\nRendimentos aplicados.");
    }

    private void EncerrarConta()
    {
        Mensagens.Titulo("ENCERRAR CONTA");
        var conta = _banco.BuscarConta(Entrada.LerInteiro("Número da conta: "));
        Console.WriteLine(conta);

        if (!Entrada.Confirmar("Deseja realmente encerrar esta conta?"))
        {
            Mensagens.Aviso("Operação cancelada.");
            return;
        }

        _banco.EncerrarConta(conta.NumeroConta);
        Mensagens.Sucesso("Conta encerrada com sucesso.");
    }

    private static string DescreverTipo(TipoTransacao tipo) => tipo switch
    {
        TipoTransacao.AberturaConta => "Abertura de conta",
        TipoTransacao.Deposito => "Depósito",
        TipoTransacao.Saque => "Saque",
        TipoTransacao.TaxaSaque => "Taxa de saque",
        TipoTransacao.TransferenciaEnviada => "Transf. enviada",
        TipoTransacao.TransferenciaRecebida => "Transf. recebida",
        TipoTransacao.Rendimento => "Rendimento",
        _ => tipo.ToString()
    };
}