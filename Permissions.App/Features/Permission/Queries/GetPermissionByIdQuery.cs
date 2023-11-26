using MediatR;
using PermissionEntity = Permissions.Domain.Entities.Permission;

namespace Permissions.App.Features.Permission.Queries
{
    public class GetPermissionByIdQuery : IRequest<PermissionEntity> {
        public int Id { get; set; }
    }
}
