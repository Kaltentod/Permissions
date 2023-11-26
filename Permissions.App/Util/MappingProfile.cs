using AutoMapper;
using Permissions.App.Features.Permission.Commands;
using Permissions.App.Presenters;
using Permissions.Domain.Entities;

namespace Permissions.App.Util
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreatePermissionCommand, Permission>();
            CreateMap<UpdatePermissionCommand, Permission>();
            CreateMap<Permission, PermissionPresenter>()
                .ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => src.PermissionTypeRel));
        }
    }
}