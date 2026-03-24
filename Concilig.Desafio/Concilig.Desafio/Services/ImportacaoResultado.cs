namespace Concilig.Desafio.Services;

// Retorno da importação: sucesso, quantidade e lista de erros por linha
public record ImportacaoResultado(bool Sucesso, int RegistrosImportados, List<string> Erros);
