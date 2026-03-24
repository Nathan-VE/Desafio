using Microsoft.AspNetCore.Http;

namespace Concilig.Desafio.Services;

// Contrato que o service deve implementar — facilita testes e substituição
public interface IImportacaoService
{
    Task<ImportacaoResultado> ImportarCSVAsync(IFormFile arquivo, string usuarioId);
}
