using MediatR;
using Permissions.App.Features.Permission.Commands;
using Permissions.App.Presenters;
using AutoMapper;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using System.Xml.Linq;
using Permissions.Domain.Exceptions;

namespace Permissions.App.Features.Permission.CommandHandlers
{
    public class DeletePermissionHandler : IRequestHandler<DeletePermissionCommand, PermissionPresenter>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeletePermissionHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PermissionPresenter> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = await _unitOfWork.PermissionRepository.GetById(request.Id) ?? 
                throw new PermissionNotFoundException($"No se encontró el permiso con ID: {request.Id}");

            await _unitOfWork.PermissionRepository.Delete(permission);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PermissionPresenter>(permission);
        }
    }
}
