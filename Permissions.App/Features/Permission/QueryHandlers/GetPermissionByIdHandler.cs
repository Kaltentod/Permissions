using MediatR;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using Permissions.App.Features.Permission.Queries;
using Permissions.Domain.Exceptions;
using System.Text.Json;
using Permissions.App.Presenters;
using AutoMapper;
using Permissions.Domain.Services;

namespace Permissions.App.Features.Permission.QueryHandlers
{
    public class GetPermissionByIdHandler : IRequestHandler<GetPermissionByIdQuery, PermissionPresenter>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPermissionByIdHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IKafkaProducerService _kafkaProducerService;

        public GetPermissionByIdHandler(IUnitOfWork unitOfWork, ILogger<GetPermissionByIdHandler> logger, IMapper mapper, 
            IKafkaProducerService kafkaProducerService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task<PermissionPresenter> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Trae el permiso por ID con su tipo y valida su existencia
                var permission = await _unitOfWork.PermissionRepository.GetById(request.Id) ??
                    throw new PermissionNotFoundException("Error getting permission", request.Id);

                // Se Produce mensaje a topic "Permission"
                await _kafkaProducerService.ProduceMessageAsync(topic: "Permission", operation: "get");

                return _mapper.Map<PermissionPresenter>(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener el permiso: {JsonSerializer.Serialize(request)}");
                _logger.LogError($"StackTrace: {ex}");
                throw;
            }
        }
    }
}
