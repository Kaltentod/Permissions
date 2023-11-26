using Permissions.Domain.Entities;

namespace Permissions.Infrastructure.SQLServer.Repositories
{
    public class PermissionRepository : CoreRepository<Permission, ApplicationDbContext>
    {
        public PermissionRepository(ApplicationDbContext context) : base(context) { }
    }
}
