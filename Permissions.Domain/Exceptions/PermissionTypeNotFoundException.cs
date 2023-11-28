namespace Permissions.Domain.Exceptions
{
    public class PermissionTypeNotFoundException : Exception
    {
        public string TraceError { get; }
        public int PermissionTypeId { get; }

        public PermissionTypeNotFoundException(string traceError, int permissionTypeId) : base($"PermissionType with ID: {permissionTypeId} not found")
        {
            TraceError = traceError;
            PermissionTypeId = permissionTypeId;
        }
    }
}
