using Concilig.Desafio.Data;
using Concilig.Desafio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Concilig.Desafio.Pages.Importacoes;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    // Quantidade de registros por página
    private const int ItensPorPagina = 10;

    public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<Importacao> Importacoes { get; set; } = [];

    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemAviso { get; set; }

    public int PaginaAtual { get; set; }
    public int TotalPaginas { get; set; }
    public int TotalRegistros { get; set; }

    public async Task OnGetAsync(int pagina = 1)
    {
        var usuarioId = _userManager.GetUserId(User)!;

        // Filtra apenas as importações do usuário logado, mais recentes primeiro
        var query = _context.Importacoes
            .Where(i => i.UsuarioId == usuarioId)
            .OrderByDescending(i => i.DataImportacao);

        TotalRegistros = await query.CountAsync();
        TotalPaginas = (int)Math.Ceiling(TotalRegistros / (double)ItensPorPagina);

        // Garante que a página requisitada está dentro do intervalo válido
        PaginaAtual = Math.Clamp(pagina, 1, Math.Max(1, TotalPaginas));

        Importacoes = await query
            .Skip((PaginaAtual - 1) * ItensPorPagina)
            .Take(ItensPorPagina)
            .ToListAsync();
    }
}
