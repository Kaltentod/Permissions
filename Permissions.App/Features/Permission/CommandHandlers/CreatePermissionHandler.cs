using AutoMapper;
using MediatR;
using Permissions.App.Features.Permission.Commands;
using Permissions.App.Presenters;
using Permissions.Domain.Exceptions;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using PermissionEntity = Permissions.Domain.Entities.Permission;

namespace Permissions.App.Features.Permission.CommandHandlers
{
    public class CreatePermissionHandler : IRequestHandler<CreatePermissionCommand, PermissionPresenter>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreatePermissionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PermissionPresenter> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = _mapper.Map<PermissionEntity>(request);

            _ = await _unitOfWork.PermissionTypeRepository.GetById(permission.PermissionType) ??
                throw new PermissionTypeNotFoundException($"No se encontró el tipo de permiso con ID: {permission.PermissionType}");

            var permissionCreated = await _unitOfWork.PermissionRepository.Create(permission);
            await _unitOfWork.SaveChangesAsync();

            permissionCreated =  await _unitOfWork.PermissionRepository.GetById(permissionCreated.Id);
            return _mapper.Map<PermissionPresenter>(permissionCreated);
        }
    }
}
