using MediatR;
using Permissions.App.Features.Permission.Commands;
using Permissions.App.Presenters;
using AutoMapper;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using Permissions.Domain.Exceptions;
using System.Text.Json;
using Permissions.Domain.Services;

namespace Permissions.App.Features.Permission.CommandHandlers
{
    public class DeletePermissionHandler : IRequestHandler<DeletePermissionCommand, PermissionPresenter>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DeletePermissionHandler> _logger;
        private readonly IElasticSearchService _elasticSearchService;
        private readonly IKafkaProducerService _kafkaProducerService;

        public DeletePermissionHandler(IUnitOfWork unitOfWork, IMapper mapper, 
            ILogger<DeletePermissionHandler> logger, IElasticSearchService elasticSearchService, IKafkaProducerService kafkaProducerService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _elasticSearchService = elasticSearchService;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<PermissionPresenter> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Se valida la existencia del permiso por ID
                var permission = await _unitOfWork.PermissionRepository.GetById(request.Id) ??
                    throw new PermissionNotFoundException("Error deleting permission", request.Id);

                // Se elimina el permiso de la base de datos
                await _unitOfWork.PermissionRepository.Delete(permission);
                await _unitOfWork.SaveChangesAsync();

                // Se elimina de ElasticSearch
                await _elasticSearchService.DeletePermission(permission);

                // Se Produce mensaje a topic "Permission"
                await _kafkaProducerService.ProduceMessageAsync(topic: "Permission", operation: "delete");

                _logger.LogInformation($"Se eliminó el permiso: {JsonSerializer.Serialize(permission)}");
                return _mapper.Map<PermissionPresenter>(permission);
            }
            catch (Exception)
            {
                _logger.LogError($"Error al eliminar el permiso con ID: {request.Id}");
                throw;
            }
        }
    }
}
