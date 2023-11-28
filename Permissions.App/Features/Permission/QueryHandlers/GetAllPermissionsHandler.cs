using MediatR;
using PermissionEntity = Permissions.Domain.Entities.Permission;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using Permissions.App.Features.Permission.Queries;
using Permissions.App.Presenters;
using System.Text.Json;
using AutoMapper;

namespace Permissions.App.Features.Permission.QueryHandlers
{
    public class GetAllPermissionsHandler : IRequestHandler<GetAllPermissionsQuery, List<PermissionPresenter>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllPermissionsHandler> _logger;

        public GetAllPermissionsHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllPermissionsHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<List<PermissionPresenter>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            try
            { 
                // Se obtienen todos los permisos
                var permissionsList = await _unitOfWork.PermissionRepository.GetAll();

                return _mapper.Map<List<PermissionEntity>, List<PermissionPresenter>>(permissionsList);
            }
            catch(Exception ex) 
            {
                _logger.LogError($"Error al obtener todos los permiso");
                _logger.LogError($"StackTrace: {ex}");
                throw;
            }
        }
    }
}
