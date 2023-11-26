using Permissions.Domain.Entities;

namespace Permissions.App.Presenters
{
    public class PermissionPresenter
    {
        public int Id { get; set; }

        public string EmployeeForename { get; set; }

        public string EmployeeSurname { get; set; }

        public PermissionType PermissionType { get; set; }

        public DateTime PermissionDate { get; set; }

    }
}
