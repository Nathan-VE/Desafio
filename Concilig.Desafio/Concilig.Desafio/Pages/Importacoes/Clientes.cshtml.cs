using Concilig.Desafio.Data;
using Concilig.Desafio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Concilig.Desafio.Pages.Importacoes;

public record ClienteResumo(
    string Cliente,
    string CPF,
    decimal ValorTotal,
    int MaiorAtrasoEmDias
);

[Authorize]
public class ClientesModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ClientesModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Importacao? Importacao { get; set; }
    public List<ClienteResumo> Resumo { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var usuarioId = _userManager.GetUserId(User)!;

        Importacao = await _context.Importacoes
            .FirstOrDefaultAsync(i => i.Id == id && i.UsuarioId == usuarioId);

        if (Importacao is null)
            return NotFound();

        var hoje = DateTime.Today;

        var contratos = await _context.Contratos
            .Where(c => c.ImportacaoId == id)
            .ToListAsync();

        Resumo = contratos
            .GroupBy(c => new { c.CPF, c.Cliente })
            .Select(g => new ClienteResumo(
                g.Key.Cliente,
                g.Key.CPF,
                g.Sum(c => c.Valor),
                g.Max(c => (hoje - c.DataVencimento).Days)
            ))
            .OrderByDescending(r => r.MaiorAtrasoEmDias)
            .ToList();

        return Page();
    }
}