using Autofac;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Permissions.App.Features.Permission.Commands;
using Permissions.App.Features.Permission.Queries;
using Permissions.Controllers;
using Xunit;

namespace Permissions.UnitTest
{
    public class PermissionTesting
    {
        private readonly PermissionsController _permissionsController;

        public PermissionTesting()
        {
            var containerBuilder = new ContainerBuilder();

            // Registrar ILogger y IMediator simulados en el contenedor
            containerBuilder.RegisterInstance(Mock.Of<ILogger<PermissionsController>>()).As<ILogger<PermissionsController>>();
            containerBuilder.RegisterInstance(Mock.Of<IMediator>());

            // Registrar PermissionsController
            containerBuilder.RegisterType<PermissionsController>();

            var container = containerBuilder.Build();

            // Resolver PermissionsController desde el contenedor
            _permissionsController = container.Resolve<PermissionsController>();
        }

        [Fact]
        public async Task GetAll_Ok()
        {
            // Realizar la llamada al método que devuelve una tarea
            var actionResultTask = _permissionsController.GetPermissions();

            // Obtener el resultado de la tarea
            var result = await actionResultTask;

            // Verificar el tipo del resultado obtenido
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetByID_Ok()
        {
            // Query de ID 1 que existen en DB
            var query = new GetPermissionByIdQuery { Id = 1 };

            // Realizar la llamada al método que devuelve una tarea
            var actionResultTask = _permissionsController.GetPermission(query);

            // Obtener el resultado de la tarea
            var result = await actionResultTask;

            // Verificar el tipo del resultado obtenido
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RequestPermission_OK()
        {
            // Command para insertar nuevo Permiso
            var command = new CreatePermissionCommand()
            {
                EmployeeForename = "Cosme",
                EmployeeSurname = "Fulanito",
                PermissionType = 1,
                PermissionDate = new DateTime(),
            };

            // Realizar la llamada al método que devuelve una tarea
            var actionResultTask = _permissionsController.RequestPermission(command);

            // Obtener el resultado de la tarea
            var result = await actionResultTask;

            // Verificar el tipo del resultado obtenido
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public async Task ModifyPermission_OK()
        {
            // Command para modificar permiso
            var command = new UpdatePermissionCommand()
            {
                Id = 1,
                EmployeeForename = "Pepe",
                EmployeeSurname = "Argento",
                PermissionType = 3,
                PermissionDate = new DateTime(),
            };

            // Realizar la llamada al método que devuelve una tarea
            var actionResultTask = _permissionsController.ModifyPermission(command);

            // Obtener el resultado de la tarea
            var result = await actionResultTask;

            // Verificar el tipo del resultado obtenido
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeletePermission_OK()
        {
            // Command para Eliminar permiso
            var command = new DeletePermissionCommand() { Id = 2, };

            // Realizar la llamada al método que devuelve una tarea
            var actionResultTask = _permissionsController.DeletePermission(command);

            // Obtener el resultado de la tarea
            var result = await actionResultTask;

            // Verificar el tipo del resultado obtenido
            Assert.IsAssignableFrom<IActionResult>(result);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
