namespace Permissions.Domain.Exceptions
{
    public class PermissionNotFoundException : Exception
    {
        public PermissionNotFoundException(string message) : base(message) { }
    }
}
