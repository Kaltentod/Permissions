namespace Permissions.Domain.Exceptions
{
    public class PermissionNotFoundException : Exception
    {
        public string TraceError { get; }
        public int PermissionId { get; }
        
        public PermissionNotFoundException(string traceError, int permissionId) : base($"Permission with ID: {permissionId} not found")
        {
            TraceError = traceError;
            PermissionId = permissionId;
        }
    }
}
