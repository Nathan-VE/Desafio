using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Concilig.Desafio.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext(options)
    {
        // Definindo contratos e importacoes como tabelas existentes no DataBase
        public DbSet<Contrato> Contratos { get; set; }
        public DbSet<Importacao> Importacoes { get; set; }
    }
}