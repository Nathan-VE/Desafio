using Concilig.Desafio.Data;
using Concilig.Desafio.Models;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Concilig.Desafio.Services;

// Responsavel por importar arquivos CSV
public class ImportacaoService
{
    // Injeta o banco de dados no service
    private readonly ApplicationDbContext _context;

    // Construtor que recebe o contexto via DI (Dependency Injection)
    public ImportacaoService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Método principal. Recebe Usuari e arquivo enviado...
    public async Task ImportarCSV(IFormFile arquivo, string usuarioId)
    {
        // Valida o arquivo
        if (arquivo == null || arquivo.Length == 0)
            throw new Exception("Arquivo inválido");

        // Coleta os dados da importacao
        var importacao = new Importacao
        {
            NomeArquivo = arquivo.FileName,
            DataImportacao = DateTime.Now,
            UsuarioId = usuarioId,
            QuantidadeRegistros = 0
        };

        // Salva a importação primeiro para obter o Id gerado pelo banco
        _context.Importacoes.Add(importacao);
        await _context.SaveChangesAsync();

        // Realiza a leitura do arquivo (linha por linha)
        using (var reader = new StreamReader(arquivo.OpenReadStream()))
        {
            // Percorre o arquivo
            while (!reader.EndOfStream)
            {
                var linha = await reader.ReadLineAsync();

                // Ignora linhas vazias
                if (string.IsNullOrWhiteSpace(linha))
                    continue;

                // Separando as informações por ";"
                var dados = linha.Split(';');

                // Contruindo o obj (transformando text em data)
                var contrato = new Contrato
                {
                    NumeroContrato = dados[0],
                    Cliente = dados[1],
                    Valor = decimal.Parse(dados[2], CultureInfo.InvariantCulture),
                    DataVencimento = DateTime.Parse(dados[3], CultureInfo.InvariantCulture),
                    ImportacaoId = importacao.Id
                };

                // Adiciona no banco e conta os registros importados
                _context.Contratos.Add(contrato);
                importacao.QuantidadeRegistros++;
            }
        }
        // Salva os contratos e atualiza a quantidade de registros
        await _context.SaveChangesAsync();
    }
}