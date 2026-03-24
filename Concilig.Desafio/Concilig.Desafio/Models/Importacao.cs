using System.ComponentModel.DataAnnotations;

// Modelo de Importacao
public class Importacao
{
    // Itens de Importacao
    [Key]
    public int Id { get; set; }

    public string NomeArquivo { get; set; }

    public DateTime DataImportacao { get; set; }

    public string UsuarioId { get; set; }

    public int QuantidadeRegistros { get; set; }

    // Relacionamento
    public List<Contrato> Contratos { get; set; }
}