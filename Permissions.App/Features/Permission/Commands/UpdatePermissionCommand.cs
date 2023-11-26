using MediatR;
using Permissions.App.Presenters;
namespace Permissions.App.Features.Permission.Commands
{
    public class UpdatePermissionCommand : IRequest<PermissionPresenter>
    {
        public int Id { get; set; }
        public string EmployeeForename { get; set; }
        public string EmployeeSurname { get; set; }
        public int PermissionType { get; set; }
        public DateTime PermissionDate { get; set; }
    }
}
