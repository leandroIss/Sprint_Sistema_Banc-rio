using SistemaBancario.Exceptions;

namespace SistemaBancario.Models;

/// <summary>
/// Classe-pai ABSTRATA de todas as contas do banco.
/// Concentra o que é comum (número, titular, saldo, depósito, extrato) e deixa
/// para as classes filhas o que muda: taxa de saque e limite disponível.
/// </summary>
public abstract class ContaBancaria
{
    private readonly List<Transacao> _historico = new();
    private string _titular = string.Empty;

    // ---------- Propriedades (encapsulamento) ----------

    /// <summary>Número único da conta. Só pode ser definido na criação.</summary>
    public int NumeroConta { get; }

    /// <summary>Nome do titular. Não aceita valor vazio.</summary>
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

    /// <summary>Saldo atual. Somente a própria conta (e suas filhas) altera o saldo.</summary>
    public decimal Saldo { get; protected set; }

    /// <summary>Valor total que pode ser sacado (saldo + limites extras, se houver).</summary>
    public virtual decimal SaldoDisponivel => Saldo;

    /// <summary>Nome legível do tipo de conta (implementado pelas filhas).</summary>
    public abstract string TipoConta { get; }

    /// <summary>Extrato somente leitura.</summary>
    public IReadOnlyList<Transacao> Historico => _historico.AsReadOnly();

    // ---------- Construtor ----------

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

    // ---------- Operações ----------

    /// <summary>Deposita um valor positivo na conta.</summary>
    public virtual void Depositar(decimal valor)
    {
        ValidarValor(valor);
        Saldo += valor;
        Registrar(TipoTransacao.Deposito, valor, "Depósito");
    }

    /// <summary>
    /// Saca um valor. A taxa (que varia por tipo de conta) é calculada por
    /// <see cref="CalcularTaxaSaque"/> - aqui está o POLIMORFISMO em ação.
    /// </summary>
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

    /// <summary>Transfere um valor (sem taxa) desta conta para outra.</summary>
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

    /// <summary>Taxa cobrada a cada saque. Padrão: sem taxa. Filhas podem sobrescrever.</summary>
    protected virtual decimal CalcularTaxaSaque() => 0m;

    /// <summary>Linhas extras de detalhe específicas de cada tipo de conta.</summary>
    protected virtual IEnumerable<string> DetalhesEspecificos() => Enumerable.Empty<string>();

    /// <summary>Texto completo com os dados da conta.</summary>
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

    /// <summary>Resumo em uma linha (usado na listagem de contas).</summary>
    public override string ToString() =>
        $"[{NumeroConta}] {TipoConta,-11} | {Titular,-25} | {Formatador.Moeda(Saldo)}";

    // ---------- Auxiliares ----------

    protected static void ValidarValor(decimal valor)
    {
        if (valor <= 0)
            throw new ValorInvalidoException("O valor da operação deve ser maior que zero.");
    }

    protected void Registrar(TipoTransacao tipo, decimal valor, string descricao) =>
        _historico.Add(new Transacao(DateTime.Now, tipo, valor, Saldo, descricao));
}
