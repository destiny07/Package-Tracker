using AutoMapper;
using Blockchainsure.PackageTracker.Application.Dto;
using Blockchainsure.PackageTracker.Domain.Models;

namespace Blockchainsure.PackageTracker.Application.Mapping
{
    public class MappingsProfile : Profile
    {
        public MappingsProfile()
        {
            CreateMap<Package, PackageDto>();
            CreateMap<PackageDto, Package>();
        }
    }
}
