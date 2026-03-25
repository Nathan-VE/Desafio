using Concilig.Desafio.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Concilig.Desafio.Pages.Importacoes;

public record ImportacaoComUsuario(
    int Id,
    string NomeArquivo,
    DateTime DataImportacao,
    int QuantidadeRegistros,
    string UsuarioEmail
);

[Authorize]
public class ConsultaModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ConsultaModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<ImportacaoComUsuario> Importacoes { get; set; } = [];

    public async Task OnGetAsync()
    {
        var dados = await _context.Importacoes
            .Join(
                _context.Users,
                i => i.UsuarioId,
                u => u.Id,
                (i, u) => new
                {
                    i.Id,
                    i.NomeArquivo,
                    i.DataImportacao,
                    i.QuantidadeRegistros,
                    u.Email,
                    u.UserName,
                    i.UsuarioId
                }
            )
            .OrderByDescending(i => i.DataImportacao)
            .ToListAsync();

        Importacoes = dados
            .Select(d => new ImportacaoComUsuario(
                d.Id,
                d.NomeArquivo,
                d.DataImportacao,
                d.QuantidadeRegistros,
                d.Email ?? d.UserName ?? d.UsuarioId
            ))
            .ToList();
    }
}