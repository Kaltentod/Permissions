using MediatR;
using PermissionEntity = Permissions.Domain.Entities.Permission;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using Permissions.App.Features.Permission.Queries;
using Permissions.Domain.Exceptions;

namespace Permissions.App.Features.Permission.QueryHandlers
{
    public class GetPermissionByIdHandler : IRequestHandler<GetPermissionByIdQuery, PermissionEntity>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPermissionByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PermissionEntity> Handle(GetPermissionByIdQuery request, CancellationToken cancellationToken)
        {
            _ = await _unitOfWork.PermissionRepository.GetById(request.Id) ??
                throw new PermissionNotFoundException($"No se encontró el permiso con ID: {request.Id}");

            return await _unitOfWork.PermissionRepository.GetById(request.Id);
        }
    }
}
