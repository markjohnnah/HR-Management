﻿using AutoMapper;
using Business.Extensions;
using Business.Resources.CategoryPerson;

namespace Business.Mapping.CategoryPerson
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<CreateCategoryPersonResource, Domain.Models.CategoryPerson>()
                .ForMember(x => x.Technology, opt => opt.MapFrom(src => src.Technology.ConcatenateWithComma()));

            CreateMap<UpdateCategoryPersonResource, Domain.Models.CategoryPerson>()
                .ForMember(x => x.Technology, opt => opt.MapFrom(src => src.Technology.ConcatenateWithComma()));
        }
    }
}