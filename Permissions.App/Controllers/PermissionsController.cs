using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Permissions.App.Features.Permission.Commands;
using Permissions.App.Features.Permission.Queries;
using System.Text.Json;

namespace Permissions.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<PermissionsController> _logger;
        private readonly IMediator _mediator;

        public PermissionsController(ILogger<PermissionsController> logger, IElasticClient elasticClient, IMediator mediator)
        {
            _logger = logger;
            _elasticClient = elasticClient;
            _mediator = mediator;
        }

        [HttpPost("requestPermission")]
        public async Task<IActionResult> RequestPermission(CreatePermissionCommand command)
        {
            try
            {
                //var response = await _elasticClient.IndexAsync(permission, idx => idx.Index(_defaultIndex));
                _logger.LogTrace($"RequestPermission: {JsonSerializer.Serialize(command)}");
                var requestedPermission = await _mediator.Send(command);
                _logger.LogInformation($"Se insertó un nuevo Permiso: {JsonSerializer.Serialize(requestedPermission)}");
                return StatusCode(201, new { message = "Permission requested successfully", data = requestedPermission});
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al insertar el permiso: {JsonSerializer.Serialize(command)}");
                _logger.LogError($"StackTrace: {ex}");
                return StatusCode(500, new { error = "Error requesting permission", details = ex.Message });
            }
        }

        [HttpPut("modifyPermission")]
        public async Task<IActionResult> ModifyPermission([FromBody] UpdatePermissionCommand command)
        {
            try
            {
                //var response = await _elasticClient.UpdateAsync<PermissionModel>(permissionId, u => u
                //    .Index(_defaultIndex)
                //    .Doc(modifiedPermission)
                //);
                _logger.LogTrace($"ModifyPermission: {JsonSerializer.Serialize(command)}");
                var modifiedPermission = await _mediator.Send(command);
                _logger.LogInformation($"Permiso modificado: {JsonSerializer.Serialize(modifiedPermission)}");
                return Ok(new { message = "Permission modified successfully", data = modifiedPermission });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al modificar el permiso: {JsonSerializer.Serialize(command)}");
                _logger.LogError($"StackTrace: {ex}");
                return StatusCode(500, new { error = "Error modifying permission", details = ex.Message });
            }
        }

        [HttpGet("getPermissions")]
        public async Task<IActionResult> GetPermissions()
        {
            try
            {
                //var searchResponse = await _elasticClient.SearchAsync<PermissionModel>(s => s
                //    .Index(_defaultIndex)
                //    .Size(100) // Cantidad máxima de permisos a obtener (modificar según sea necesario)
                //);

                //var permissions = searchResponse.Documents;
                //return Ok(new { permissions });
                _logger.LogTrace($"GetPermissions");
                return Ok(await _mediator.Send(new GetAllPermissionsQuery()));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al consultar los permisos");
                _logger.LogError($"StackTrace: {ex}");
                return StatusCode(500, new { error = "Error getting permissions", details = ex.Message });
            }
        }

        [HttpGet("getPermission/{Id}")]
        public async Task<IActionResult> GetPermission([FromRoute] GetPermissionByIdQuery query)
        {
            try
            {
                _logger.LogTrace($"GetPermissionByIdQuery: {JsonSerializer.Serialize(query)}");
                return Ok(await _mediator.Send(query));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al consultar el permiso ID: {JsonSerializer.Serialize(query)}");
                _logger.LogError($"StackTrace: {ex}");
                return StatusCode(500, new { error = "Error getting permission", details = ex.Message });
            }
        }

        [HttpDelete("DeletePermission/{Id}")]
        public async Task<IActionResult> DeletePermission([FromRoute] DeletePermissionCommand command)
        {
            try
            {
                _logger.LogTrace($"DeletePermissionCommand: {JsonSerializer.Serialize(command)}");
                var deletedPermission = await _mediator.Send(command);
                _logger.LogInformation($"Se eliminó el permiso: {JsonSerializer.Serialize(deletedPermission)}");
                return Ok(new { message = "Permission deleted successfully", data = deletedPermission });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar el permiso ID: {JsonSerializer.Serialize(command)}");
                _logger.LogError($"StackTrace: {ex}");
                return StatusCode(500, new { error = "Error getting permission", details = ex.Message });
            }
        }
    }
}