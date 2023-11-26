using Permissions.Domain.Entities;
using Permissions.Infrastructure.SQLServer.Repositories;

namespace Permissions.Infrastructure.SQLServer.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Permission> PermissionRepository { get; }
        IRepository<PermissionType> PermissionTypeRepository { get; }

        void BeginTransaction();
        void Commit();
        void Rollback();
        Task<int> SaveChangesAsync();
    }

}
