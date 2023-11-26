using MediatR;
using Permissions.App.Presenters;


namespace Permissions.App.Features.Permission.Commands
{
    public class DeletePermissionCommand : IRequest<PermissionPresenter> {
        public int Id { get; set; }
    }
}
