using Concilig.Desafio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Concilig.Desafio.Pages.Importacoes;

// Apenas usuários autenticados podem importar
[Authorize]
public class UploadModel : PageModel
{
    private readonly IImportacaoService _importacaoService;
    private readonly UserManager<IdentityUser> _userManager;

    public UploadModel(IImportacaoService importacaoService, UserManager<IdentityUser> userManager)
    {
        _importacaoService = importacaoService;
        _userManager = userManager;
    }

    // TempData persiste mensagens durante o redirect
    [TempData]
    public string? MensagemSucesso { get; set; }

    [TempData]
    public string? MensagemAviso { get; set; }

    [TempData]
    public string? MensagemErro { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(IFormFile arquivo)
    {
        // Obtém o Id do usuário autenticado pelo Identity
        var usuarioId = _userManager.GetUserId(User)!;

        try
        {
            var resultado = await _importacaoService.ImportarCSVAsync(arquivo, usuarioId);

            if (!resultado.Sucesso)
            {
                MensagemErro = "Nenhum registro válido encontrado no arquivo.";
                return Page();
            }

            MensagemSucesso = $"{resultado.RegistrosImportados} registro(s) importado(s) com sucesso!";

            // Importou com sucesso mas algumas linhas tinham problemas
            if (resultado.Erros.Any())
                MensagemAviso = $"{resultado.Erros.Count} linha(s) foram ignoradas por erro de formato.";

            return RedirectToPage("/Importacoes/Index");
        }
        catch (ArgumentException ex)
        {
            // Erros de validação (tamanho, extensão, arquivo vazio)
            MensagemErro = ex.Message;
            return Page();
        }
    }
}
