﻿using AutoMapper;
using AxelCMS.Application.DTO;
using AxelCMS.Common.Utilities;
using AxelCMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AxelCMS.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<User, UserDto>();
            CreateMap<PageResult<IEnumerable<User>>, PageResult<IEnumerable<UserDto>>>();
            CreateMap<User, UpdateUserDto>().ReverseMap();
            CreateMap<UpdatePhotoDto, UserDto>().ReverseMap();
        }
    }
}
