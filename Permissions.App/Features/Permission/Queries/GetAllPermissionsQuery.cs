using MediatR;
using Permissions.App.Presenters;

namespace Permissions.App.Features.Permission.Queries
{
    public class GetAllPermissionsQuery : IRequest<List<PermissionPresenter>> { }
}
