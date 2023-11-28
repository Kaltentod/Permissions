using MediatR;
using Microsoft.AspNetCore.Mvc;
using Permissions.App.Features.Permission.Commands;
using Permissions.App.Features.Permission.Queries;
using System.Text.Json;

namespace Permissions.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly ILogger<PermissionsController> _logger;
        private readonly IMediator _mediator;

        public PermissionsController(ILogger<PermissionsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("requestPermission")]
        public async Task<IActionResult> RequestPermission(CreatePermissionCommand command)
        {
            _logger.LogTrace($"RequestPermission: {JsonSerializer.Serialize(command)}");
            var requestedPermission = await _mediator.Send(command);
            return StatusCode(201, new { message = "Permission requested successfully", data = requestedPermission });
        }

        [HttpDelete("DeletePermission/{Id}")]
        public async Task<IActionResult> DeletePermission([FromRoute] DeletePermissionCommand command)
        {
            _logger.LogTrace($"DeletePermission: {JsonSerializer.Serialize(command)}");
            var deletedPermission = await _mediator.Send(command);
            return Ok(new { message = "Permission deleted successfully", data = deletedPermission });

        }

        [HttpPut("modifyPermission")]
        public async Task<IActionResult> ModifyPermission([FromBody] UpdatePermissionCommand command)
        {
             _logger.LogTrace($"ModifyPermission: {JsonSerializer.Serialize(command)}");
             var modifiedPermission = await _mediator.Send(command);
             return Ok(new { message = "Permission modified successfully", data = modifiedPermission });
        }

        [HttpGet("getPermissions")]
        public async Task<IActionResult> GetPermissions()
        {
            _logger.LogTrace($"GetPermissions");
            return Ok(await _mediator.Send(new GetAllPermissionsQuery()));
        }

        [HttpGet("getPermission/{Id}")]
        public async Task<IActionResult> GetPermission([FromRoute] GetPermissionByIdQuery query)
        {
            _logger.LogTrace($"GetPermissionById: {JsonSerializer.Serialize(query)}");
            return Ok(await _mediator.Send(query));
        }

    }
}