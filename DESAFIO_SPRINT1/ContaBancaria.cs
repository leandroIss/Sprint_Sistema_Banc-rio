using SistemaBancario.Exceptions;

namespace SistemaBancario.Models;


public abstract class ContaBancaria
{
    private readonly List<Transacao> _historico = new();
    private string _titular = string.Empty;

  
    public int NumeroConta { get; }

  
    public string Titular
    {
        get => _titular;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValorInvalidoException("O nome do titular não pode ser vazio.");
            _titular = value.Trim();
        }
    }

  
    public decimal Saldo { get; protected set; }


    public virtual decimal SaldoDisponivel => Saldo;

  
    public abstract string TipoConta { get; }

  
    public IReadOnlyList<Transacao> Historico => _historico.AsReadOnly();

 

    protected ContaBancaria(int numeroConta, string titular, decimal saldoInicial)
    {
        if (numeroConta <= 0)
            throw new ValorInvalidoException("O número da conta deve ser positivo.");
        if (saldoInicial < 0)
            throw new ValorInvalidoException("O saldo inicial não pode ser negativo.");

        NumeroConta = numeroConta;
        Titular = titular;
        Saldo = saldoInicial;

        Registrar(TipoTransacao.AberturaConta, saldoInicial, "Abertura de conta");
    }

  
    public virtual void Depositar(decimal valor)
    {
        ValidarValor(valor);
        Saldo += valor;
        Registrar(TipoTransacao.Deposito, valor, "Depósito");
    }

    /// <see cref="CalcularTaxaSaque"
   
    public void Sacar(decimal valor)
    {
        ValidarValor(valor);

        decimal taxa = CalcularTaxaSaque();
        decimal total = valor + taxa;

        if (total > SaldoDisponivel)
            throw new SaldoInsuficienteException(total, SaldoDisponivel);

        Saldo -= valor;
        Registrar(TipoTransacao.Saque, valor, "Saque");

        if (taxa > 0)
        {
            Saldo -= taxa;
            Registrar(TipoTransacao.TaxaSaque, taxa, "Taxa de saque");
        }
    }


    public void Transferir(ContaBancaria destino, decimal valor)
    {
        if (destino is null)
            throw new OperacaoInvalidaException("Conta de destino inválida.");
        if (ReferenceEquals(destino, this))
            throw new OperacaoInvalidaException("Não é possível transferir para a mesma conta.");

        ValidarValor(valor);

        if (valor > SaldoDisponivel)
            throw new SaldoInsuficienteException(valor, SaldoDisponivel);

        Saldo -= valor;
        Registrar(TipoTransacao.TransferenciaEnviada, valor,
            $"Transferência enviada para conta {destino.NumeroConta}");

        destino.Saldo += valor;
        destino.Registrar(TipoTransacao.TransferenciaRecebida, valor,
            $"Transferência recebida da conta {NumeroConta}");
    }


    protected virtual decimal CalcularTaxaSaque() => 0m;


    protected virtual IEnumerable<string> DetalhesEspecificos() => Enumerable.Empty<string>();

 
    public string ObterDetalhes()
    {
        var linhas = new List<string>
        {
            $"Tipo         : {TipoConta}",
            $"Número       : {NumeroConta}",
            $"Titular      : {Titular}",
            $"Saldo        : {Formatador.Moeda(Saldo)}"
        };
        linhas.AddRange(DetalhesEspecificos());
        return string.Join(Environment.NewLine, linhas);
    }

  
    public override string ToString() =>
        $"[{NumeroConta}] {TipoConta,-11} | {Titular,-25} | {Formatador.Moeda(Saldo)}";



    protected static void ValidarValor(decimal valor)
    {
        if (valor <= 0)
            throw new ValorInvalidoException("O valor da operação deve ser maior que zero.");
    }

    protected void Registrar(TipoTransacao tipo, decimal valor, string descricao) =>
        _historico.Add(new Transacao(DateTime.Now, tipo, valor, Saldo, descricao));
}