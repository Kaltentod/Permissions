using AutoMapper;
using MediatR;
using Permissions.App.Features.Permission.Commands;
using Permissions.App.Presenters;
using Permissions.Domain.Exceptions;
using Permissions.Domain.Services;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using System.Text.Json;
using PermissionEntity = Permissions.Domain.Entities.Permission;

namespace Permissions.App.Features.Permission.CommandHandlers
{
    public class CreatePermissionHandler : IRequestHandler<CreatePermissionCommand, PermissionPresenter>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IElasticSearchService _elasticSearchService;
        private readonly IKafkaProducerService _kafkaProducerService;

        public CreatePermissionHandler(IUnitOfWork unitOfWork, IMapper mapper, 
            ILogger<CreatePermissionHandler> logger, IElasticSearchService elasticSearchService, IKafkaProducerService kafkaProducerService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _elasticSearchService = elasticSearchService;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<PermissionPresenter> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var permission = _mapper.Map<PermissionEntity>(request);

                // Se valida la existencia de tipo de permiso por ID
                var permissionType = await _unitOfWork.PermissionTypeRepository.GetById(permission.PermissionType) ??
                    throw new PermissionTypeNotFoundException("Error requesting permission", permission.PermissionType);

                // Se persiste nuevo permiso en la base de datos
                var permissionCreated = await _unitOfWork.PermissionRepository.Create(permission);
                await _unitOfWork.SaveChangesAsync();

                // Se persiste nuevo permiso en ElasticSearch
                await _elasticSearchService.IndexPermission(permissionCreated);

                // Se Produce mensaje a topic "Permission"
                await _kafkaProducerService.ProduceMessageAsync(topic:"Permission", operation:"request");

                // Se agrega el tipo de permiso para mostrarlo en el response
                permissionCreated.PermissionTypeRel = permissionType;

                _logger.LogInformation($"Se insertó un nuevo Permiso: {JsonSerializer.Serialize(permissionCreated)}");
                return _mapper.Map<PermissionPresenter>(permissionCreated);
            }
            catch(Exception) 
            {
                _logger.LogError($"Error al insertar el permiso: {JsonSerializer.Serialize(request)}");
                throw;
            }
        }
    }
}