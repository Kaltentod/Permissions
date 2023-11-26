using MediatR;
using PermissionEntity = Permissions.Domain.Entities.Permission;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using Permissions.App.Features.Permission.Queries;
using Permissions.App.Presenters;
using AutoMapper;

namespace Permissions.App.Features.Permission.QueryHandlers
{
    public class GetAllPermissionsHandler : IRequestHandler<GetAllPermissionsQuery, List<PermissionPresenter>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllPermissionsHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<PermissionPresenter>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissionsList = await _unitOfWork.PermissionRepository.GetAll();

            return _mapper.Map<List<PermissionEntity>, List<PermissionPresenter>>(permissionsList);
        }
    }
}
