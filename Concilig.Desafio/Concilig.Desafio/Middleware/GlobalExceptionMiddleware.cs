namespace Concilig.Desafio.Middleware;

// Captura exceções não tratadas antes que cheguem ao usuário como 500 bruto
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            // Erros de validação esperados — nível Warning, mensagem amigável
            _logger.LogWarning(ex, "Erro de validação: {Mensagem}", ex.Message);
            context.Response.Redirect($"/Error?mensagem={Uri.EscapeDataString(ex.Message)}");
        }
        catch (Exception ex)
        {
            // Erros inesperados — nível Error, mensagem genérica
            _logger.LogError(ex, "Erro inesperado na requisição {Path}", context.Request.Path);
            context.Response.Redirect("/Error");
        }
    }
}
