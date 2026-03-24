using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Modelo do Contrato
public class Contrato
{
    // Itens do Contrato
    [Key]
    public int Id { get; set; }

    [Required]
    public string NumeroContrato { get; set; }

    [Required]
    public string Cliente { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Valor { get; set; }

    public DateTime DataVencimento { get; set; }

    // Relacionamento
    public int ImportacaoId { get; set; }
    public Importacao importacao { get; set; }
}