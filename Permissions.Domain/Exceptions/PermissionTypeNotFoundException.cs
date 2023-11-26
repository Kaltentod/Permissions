namespace Permissions.Domain.Exceptions
{
    public class PermissionTypeNotFoundException : Exception
    {
        public PermissionTypeNotFoundException(string message) : base(message) { }
    }
}
