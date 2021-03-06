﻿using AutoMapper;
using Voyage.Models.Entities;

namespace Voyage.Models.Map.Profiles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<ApplicationRole, RoleModel>()
                .ForMember(_ => _.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(_ => _.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}
