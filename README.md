# Sistema Bancário em C# — Sprint 1 (POO)

Sistema em **modo console** que gerencia diferentes tipos de contas bancárias,
aplicando Programação Orientada a Objetos: classes abstratas, herança,
polimorfismo, interface, encapsulamento, exceções personalizadas, coleções
(`List<T>`), estruturas condicionais e de repetição.

## Como executar

Requisitos: **.NET SDK 8.0** (ou superior) ou Visual Studio 2022+.

- **Visual Studio:** abra a pasta `SistemaBancario`, abra `SistemaBancario.csproj` e pressione **F5**.
- **Terminal:**
  ```
  cd SistemaBancario
  dotnet run
  ```

Ao iniciar, existem 3 contas de demonstração: `1001` (Corrente), `1002` (Poupança) e `1003` (Empresarial).

---

## AÇÃO 1 — Modelagem da solução

### Problema
Um banco precisa gerenciar diferentes tipos de contas. Todas têm saldo e titular,
mas as regras de saque e taxas mudam conforme o tipo.

### Classes, atributos e métodos

| Classe | Papel | Atributos / Propriedades | Métodos principais |
|---|---|---|---|
| `ContaBancaria` (**abstrata**) | Classe-pai | `NumeroConta` (somente leitura), `Titular` (validado), `Saldo` (`protected set`), `SaldoDisponivel` (virtual), `TipoConta` (abstrata), `Historico` (somente leitura) | `Depositar`, `Sacar`, `Transferir`, `CalcularTaxaSaque` (virtual), `ObterDetalhes` |
| `ContaCorrente` | Filha | — | `CalcularTaxaSaque()` → **R$ 2,50 por saque** |
| `ContaPoupanca` | Filha + `IRentavel` | `TaxaRendimento` (0,5%) | `AplicarRendimento()`; **sem taxa** de saque |
| `ContaEmpresarial` | Filha | `LimiteEmprestimo`, `EmprestimoUtilizado` | `SaldoDisponivel` = saldo + limite (saldo pode ficar negativo até o limite) |
| `IRentavel` | Interface | `TaxaRendimento` | `AplicarRendimento()` |
| `Transacao` | Registro imutável (`record`) | `Data`, `Tipo`, `Valor`, `SaldoApos`, `Descricao` | — |
| `TipoTransacao` | `enum` | Abertura, Depósito, Saque, TaxaSaque, TransferênciaEnviada/Recebida, Rendimento | — |
| `Banco` | Serviço | `List<ContaBancaria>` | `AbrirConta*`, `BuscarConta`, `Transferir`, `AplicarRendimentos`, `EncerrarConta` |
| `Menu`, `Entrada`, `Mensagens` | Interface com o usuário | — | Menu interativo, leitura segura de dados, mensagens coloridas |
| `BancoException` + 4 filhas | Exceções | — | `SaldoInsuficiente`, `ValorInvalido`, `ContaNaoEncontrada`, `OperacaoInvalida` |

### Relacionamentos

```
                     ┌──────────────────────┐
                     │ ContaBancaria (abs.) │
                     └──────────▲───────────┘
          ┌─────────────────────┼─────────────────────┐
 ┌────────┴───────┐   ┌─────────┴────────┐   ┌────────┴─────────┐
 │ ContaCorrente  │   │  ContaPoupanca   │   │ ContaEmpresarial │
 └────────────────┘   └─────────┬────────┘   └──────────────────┘
                                │ implementa
                          ┌─────┴─────┐
                          │ IRentavel │
                          └───────────┘

 Banco ◆──────── 0..* ContaBancaria      (composição)
 ContaBancaria ◆── 0..* Transacao        (composição)
```

- **Herança:** as três contas herdam de `ContaBancaria`.
- **Polimorfismo:** `Sacar()` chama `CalcularTaxaSaque()` e `SaldoDisponivel`, que cada filha
  sobrescreve. O `Banco` guarda tudo como `ContaBancaria` e trata todas de forma uniforme.
- **Interface:** `IRentavel` é implementada só pela poupança; `Banco.AplicarRendimentos()`
  usa `conta is IRentavel` para achar quem rende.
- **Composição:** `Banco` tem contas; cada conta tem seu histórico de transações.

## Regras de negócio

- Depósito, saque e transferência exigem valor **maior que zero**.
- **Corrente:** cada saque cobra R$ 2,50 (saldo precisa cobrir valor + taxa).
- **Poupança:** sem taxa; rendimento de 0,5% aplicado pela opção 8 do menu.
- **Empresarial:** pode usar o limite de empréstimo (saldo negativo até o limite).
- Transferência não tem taxa e não pode ser feita para a mesma conta.
- Só é possível encerrar conta com **saldo zero**.

## Encapsulamento e tratamento de exceções

- `Saldo` só é alterado dentro da hierarquia de contas (`protected set`); `NumeroConta` é imutável;
  `Titular` valida vazio; o histórico é exposto como `IReadOnlyList`.
- Entradas inválidas (letras, vazio, negativo) são tratadas em `Entrada` com `TryParse`
  e repetição da pergunta — o programa nunca "quebra".
- Erros de regra usam exceções personalizadas, capturadas no `Menu` (`catch (BancoException)`).
- `Program.cs` tem um `catch` final como rede de segurança.

## AÇÃO 3 — Testes realizados

| # | Teste | Resultado esperado | OK |
|---|---|---|---|
| 1 | Depósito de R$ 100,50 na conta 1001 | Saldo R$ 1.100,50 | ✔ |
| 2 | Saque de R$ 50 na Corrente (saldo 1.000) | Saldo R$ 947,50 (taxa de 2,50 no extrato) | ✔ |
| 3 | Saque de R$ 100 na Poupança | Sem taxa, saldo R$ 1.900,00 | ✔ |
| 4 | Saque de R$ 3.000 na Empresarial (saldo 500, limite 3.000) | Permitido, saldo −R$ 2.500,00 | ✔ |
| 5 | Saque acima do disponível | `SaldoInsuficienteException` | ✔ |
| 6 | Valor `abc`, `-5` e `0` | Mensagem de erro, sem travar | ✔ |
| 7 | Transferência entre contas | Débito e crédito corretos | ✔ |
| 8 | Transferência para a mesma conta / conta inexistente | Erro tratado | ✔ |
| 9 | Opção de menu inválida (`x`, `99`) | Erro e volta ao menu | ✔ |
| 10 | Rendimento na poupança | +0,5% do saldo | ✔ |
| 11 | Encerrar conta com saldo ≠ 0 | Bloqueado | ✔ |
| 12 | Encerrar conta com saldo zero | Conta removida | ✔ |
| 13 | Nome do titular vazio | Pergunta repetida | ✔ |

## Estrutura de arquivos

```
SistemaBancario/
├── SistemaBancario.csproj
├── Program.cs
├── Formatador.cs
├── Models/        ContaBancaria, ContaCorrente, ContaPoupanca, ContaEmpresarial, Transacao, TipoTransacao
├── Interfaces/    IRentavel
├── Exceptions/    BancoException, SaldoInsuficiente, ValorInvalido, ContaNaoEncontrada, OperacaoInvalida
├── Services/      Banco
└── UI/            Menu, Entrada, Mensagens
```
