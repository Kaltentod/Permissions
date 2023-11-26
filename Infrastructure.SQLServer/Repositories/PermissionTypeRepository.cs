using Permissions.Domain.Entities;

namespace Permissions.Infrastructure.SQLServer.Repositories
{
    public class PermissionTypeRepository : CoreRepository<PermissionType, ApplicationDbContext>
    {
        public PermissionTypeRepository(ApplicationDbContext context) : base(context) { }
    }
}
