namespace Concilig.Desafio.DTOs;

// Representa os dados de uma linha do CSV antes de virar entidade
public class ContratoImportadoDto
{
    public string NumeroContrato { get; set; } = string.Empty;
    public string Cliente { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
}
