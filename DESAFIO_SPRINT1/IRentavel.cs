namespace SistemaBancario.Interfaces;

public interface IRentavel
{
  
    decimal TaxaRendimento { get; }

 
    decimal AplicarRendimento();
}