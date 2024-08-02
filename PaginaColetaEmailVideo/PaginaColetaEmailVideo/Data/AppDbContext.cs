using Microsoft.EntityFrameworkCore;
using PaginaColetaEmailVideo.Models;

namespace PaginaColetaEmailVideo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        // tabela de email
        public DbSet<EmailModel> Emails { get; set; }

        // tabela de usuário
        public DbSet<UsuarioModel> Usuarios { get; set; }
    }
}
