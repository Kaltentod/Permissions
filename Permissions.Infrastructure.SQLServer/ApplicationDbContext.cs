using Microsoft.EntityFrameworkCore;
using Permissions.Domain.Entities;

namespace Permissions.Infrastructure.SQLServer
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }
    }
}
