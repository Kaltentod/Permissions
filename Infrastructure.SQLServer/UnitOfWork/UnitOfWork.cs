using Permissions.Domain.Entities;
using Permissions.Infrastructure.SQLServer.Repositories;


namespace Permissions.Infrastructure.SQLServer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IRepository<Permission> PermissionRepository { get; private set; }
        public IRepository<PermissionType> PermissionTypeRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context, IRepository<Permission> permissionRepository, IRepository<PermissionType> permissionTypeRepository)
        {
            _context = context;
            PermissionRepository = permissionRepository;
            PermissionTypeRepository = permissionTypeRepository;
        }

        public void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _context.Database.CurrentTransaction.Commit();
        }

        public void Rollback()
        {
            _context.Database.CurrentTransaction.Rollback();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
