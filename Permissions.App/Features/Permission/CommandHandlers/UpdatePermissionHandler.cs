using AutoMapper;
using MediatR;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using Permissions.App.Features.Permission.Commands;
using Permissions.App.Presenters;
using PermissionEntity = Permissions.Domain.Entities.Permission;
using Permissions.Domain.Exceptions;
using Permissions.Domain.Entities;

namespace Permissions.App.Features.Permission.CommandHandlers
{
    public class UpdatePermissionHandler : IRequestHandler<UpdatePermissionCommand, PermissionPresenter>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdatePermissionHandler(IUnitOfWork unitOfWork, IMapper mapper) { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<PermissionPresenter> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = _mapper.Map<PermissionEntity>(request);

            _ = await _unitOfWork.PermissionRepository.GetById(request.Id) ??
                throw new PermissionNotFoundException($"No se encontró el permiso con ID: {request.Id}");

            _ = await _unitOfWork.PermissionTypeRepository.GetById(permission.PermissionType) ??
                throw new PermissionTypeNotFoundException($"No se encontró el tipo de permiso con ID: {permission.PermissionType}");

            var updatedPermission = await _unitOfWork.PermissionRepository.Update(permission);
            await _unitOfWork.SaveChangesAsync();

            updatedPermission = await _unitOfWork.PermissionRepository.GetById(updatedPermission.Id);
            return _mapper.Map<PermissionPresenter>(updatedPermission);
        }
    }
}