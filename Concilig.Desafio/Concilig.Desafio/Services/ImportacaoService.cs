using Concilig.Desafio.Data;
using Concilig.Desafio.DTOs;
using Concilig.Desafio.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Concilig.Desafio.Services;

public class ImportacaoService : IImportacaoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ImportacaoService> _logger;

    // 5 MB — limite razoável para CSVs de contratos
    private const long TamanhoMaximoBytes = 5 * 1024 * 1024;

    public ImportacaoService(ApplicationDbContext context, ILogger<ImportacaoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImportacaoResultado> ImportarCSVAsync(IFormFile arquivo, string usuarioId)
    {
        // Valida extensão, tamanho e nulidade antes de qualquer processamento
        ValidarArquivo(arquivo);

        var erros = new List<string>();
        var contratos = new List<Contrato>();
        int numeroLinha = 0;

        using var reader = new StreamReader(arquivo.OpenReadStream());

        // Ignora a primeira linha (cabeçalho do CSV)
        await reader.ReadLineAsync();

        while (!reader.EndOfStream)
        {
            numeroLinha++;
            var linha = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(linha))
                continue;

            // Tenta parsear — se falhar, registra o erro e segue para a próxima linha
            var (dto, erro) = TentarParsearLinha(linha, numeroLinha);

            if (erro != null)
            {
                erros.Add(erro);
                _logger.LogWarning("Linha {Numero} ignorada: {Erro}", numeroLinha, erro);
                continue;
            }

            contratos.Add(MapearContrato(dto!));
        }

        if (contratos.Count == 0)
        {
            _logger.LogWarning("Arquivo {Arquivo} não gerou nenhum registro válido.", arquivo.FileName);
            return new ImportacaoResultado(false, 0, erros);
        }

        // Persiste a importação primeiro para obter o Id gerado pelo banco
        var importacao = new Importacao
        {
            NomeArquivo = arquivo.FileName,
            DataImportacao = DateTime.Now,
            UsuarioId = usuarioId,
            QuantidadeRegistros = contratos.Count
        };

        _context.Importacoes.Add(importacao);
        await _context.SaveChangesAsync();

        // Associa cada contrato ao Id da importação recém-criada
        foreach (var contrato in contratos)
            contrato.ImportacaoId = importacao.Id;

        // AddRange é mais eficiente que múltiplos Add individuais
        await _context.Contratos.AddRangeAsync(contratos);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Importação concluída: {Registros} registro(s), {Erros} linha(s) com erro. Arquivo: {Arquivo}",
            contratos.Count, erros.Count, arquivo.FileName);

        return new ImportacaoResultado(true, contratos.Count, erros);
    }

    // --- Métodos privados de suporte ---

    private static void ValidarArquivo(IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            throw new ArgumentException("Nenhum arquivo enviado ou arquivo vazio.");

        if (!arquivo.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Apenas arquivos .csv são permitidos.");

        if (arquivo.Length > TamanhoMaximoBytes)
            throw new ArgumentException("O arquivo excede o limite de 5 MB.");
    }

    // Retorna o DTO preenchido OU uma mensagem de erro — nunca lança exceção
    private static (ContratoImportadoDto? Dto, string? Erro) TentarParsearLinha(string linha, int numeroLinha)
    {
        var colunas = linha.Split(';');

        if (colunas.Length < 4)
            return (null, $"Linha {numeroLinha}: esperadas 4 colunas, encontradas {colunas.Length}.");

        if (string.IsNullOrWhiteSpace(colunas[0]))
            return (null, $"Linha {numeroLinha}: número do contrato é obrigatório.");

        if (string.IsNullOrWhiteSpace(colunas[1]))
            return (null, $"Linha {numeroLinha}: nome do cliente é obrigatório.");

        if (!decimal.TryParse(colunas[2].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var valor))
            return (null, $"Linha {numeroLinha}: valor inválido '{colunas[2].Trim()}'.");

        if (!DateTime.TryParse(colunas[3].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var dataVencimento))
            return (null, $"Linha {numeroLinha}: data de vencimento inválida '{colunas[3].Trim()}'.");

        return (new ContratoImportadoDto
        {
            NumeroContrato = colunas[0].Trim(),
            Cliente = colunas[1].Trim(),
            Valor = valor,
            DataVencimento = dataVencimento
        }, null);
    }

    // Converte o DTO em entidade de banco — separação clara de responsabilidades
    private static Contrato MapearContrato(ContratoImportadoDto dto) => new()
    {
        NumeroContrato = dto.NumeroContrato,
        Cliente = dto.Cliente,
        Valor = dto.Valor,
        DataVencimento = dto.DataVencimento
    };
}
