using MediatR;
using Permissions.App.Presenters;
using PermissionEntity = Permissions.Domain.Entities.Permission;

namespace Permissions.App.Features.Permission.Queries
{
    public class GetPermissionByIdQuery : IRequest<PermissionPresenter> {
        public int Id { get; set; }
    }
}
