using AutoMapper;
using MediatR;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using Permissions.App.Features.Permission.Commands;
using Permissions.App.Presenters;
using PermissionEntity = Permissions.Domain.Entities.Permission;
using Permissions.Domain.Exceptions;
using Permissions.Domain.Services;
using System.Text.Json;

namespace Permissions.App.Features.Permission.CommandHandlers
{
    public class UpdatePermissionHandler : IRequestHandler<UpdatePermissionCommand, PermissionPresenter>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdatePermissionHandler> _logger;
        private readonly IElasticSearchService _elasticSearchService;
        private readonly IKafkaProducerService _kafkaProducerService;

        public UpdatePermissionHandler(IUnitOfWork unitOfWork, IMapper mapper, 
            ILogger<UpdatePermissionHandler> logger,IElasticSearchService elasticSearchService, IKafkaProducerService kafkaProducerService) { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _elasticSearchService = elasticSearchService;
            _kafkaProducerService = kafkaProducerService;
        }
        public async Task<PermissionPresenter> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var permission = _mapper.Map<PermissionEntity>(request);

                // Se valida la existencia del permiso por ID
                _ = await _unitOfWork.PermissionRepository.GetById(permission.Id) ??
                    throw new PermissionNotFoundException("Error modifying permission", permission.Id);

                // Se valida la existencia de tipo de permiso del request por ID
                var permissionType = await _unitOfWork.PermissionTypeRepository.GetById(permission.PermissionType) ??
                    throw new PermissionTypeNotFoundException("Error modifying permission", permission.PermissionType);

                // Se modifica el permiso en la base de datos
                var updatedPermission = await _unitOfWork.PermissionRepository.Update(permission);
                await _unitOfWork.SaveChangesAsync();

                // Se actualiza permiso en ElasticSearch
                await _elasticSearchService.UpdatePermission(updatedPermission);

                // Se Produce mensaje a topic "Permission"
                await _kafkaProducerService.ProduceMessageAsync(topic: "Permission", operation: "modify");

                // Se agrega el tipo de permiso para mostrarlo en el response
                updatedPermission.PermissionTypeRel = permissionType;

                _logger.LogInformation($"Permiso modificado: {JsonSerializer.Serialize(updatedPermission)}");
                return _mapper.Map<PermissionPresenter>(updatedPermission);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error al modificar el permiso: {JsonSerializer.Serialize(request)}");
                _logger.LogError($"StackTrace: {ex}");
                throw;
            }
        }
    }
}