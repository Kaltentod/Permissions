using Permissions.Domain.Entities;

namespace Permissions.Infrastructure.SQLServer
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated(); // Asegura que la base de datos esté creada

            // Verifica si ya existen datos en la tabla de PermissionType
            if (!context.PermissionTypes.Any())
            {
                // Si no hay datos de PermissionType, agregamos algunos ejemplos
                var permissionTypes = new PermissionType[]
                {
                    new() { Description = "Consulta" },
                    new() { Description = "Operador" },
                    new() { Description = "Supervisor" }
                };

                context.PermissionTypes.AddRange(permissionTypes);
                context.SaveChanges();
            }

            // Verifica si ya existen datos en la tabla de Permission
            if (context.Permissions.Any())
            {
                return; // Si hay datos, no hacemos nada
            }

            // Si no hay datos, agregamos algunos ejemplos de Permission
            var permissions = new Permission[]
            {
                new()
                {
                    EmployeeForename = "John",
                    EmployeeSurname = "Doe",
                    PermissionType = context.PermissionTypes.First(p => p.Description == "Consulta").Id,
                    PermissionDate = DateTime.UtcNow.Date
                },
                new()
                {
                    EmployeeForename = "Jane",
                    EmployeeSurname = "Smith",
                    PermissionType = context.PermissionTypes.First(p => p.Description == "Operador").Id,
                    PermissionDate = DateTime.UtcNow.Date.AddDays(-1)
                },
                new()
                {
                    EmployeeForename = "Alice",
                    EmployeeSurname = "Johnson",
                    PermissionType = context.PermissionTypes.First(p => p.Description == "Supervisor").Id,
                    PermissionDate = DateTime.UtcNow.Date.AddDays(-2)
                }
                // Puedes añadir más ejemplos de permisos aquí
            };

            context.Permissions.AddRange(permissions);
            context.SaveChanges();
        }
    }
}