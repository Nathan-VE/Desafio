using Concilig.Desafio.Data;
using Concilig.Desafio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Concilig.Desafio.Pages.Importacoes;

[Authorize]
public class DetalhesModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public DetalhesModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Importacao? Importacao { get; set; }
    public List<Contrato> Contratos { get; set; } = [];

    // SupportsGet = true permite que os filtros venham pela query string (GET)
    [BindProperty(SupportsGet = true)]
    public string? FiltroCliente { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? DataInicio { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? DataFim { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var usuarioId = _userManager.GetUserId(User)!;

        // Verifica se a importação existe E pertence ao usuário logado
        Importacao = await _context.Importacoes
            .FirstOrDefaultAsync(i => i.Id == id && i.UsuarioId == usuarioId);

        if (Importacao is null)
            return NotFound();

        // Monta a query com filtros opcionais encadeados
        var query = _context.Contratos
            .Where(c => c.ImportacaoId == id)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(FiltroCliente))
            query = query.Where(c => c.Cliente.Contains(FiltroCliente));

        if (DataInicio.HasValue)
            query = query.Where(c => c.DataVencimento >= DataInicio.Value);

        if (DataFim.HasValue)
            query = query.Where(c => c.DataVencimento <= DataFim.Value);

        Contratos = await query
            .OrderBy(c => c.DataVencimento)
            .ToListAsync();

        return Page();
    }
}
