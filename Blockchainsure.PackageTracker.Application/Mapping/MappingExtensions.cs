using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Blockchainsure.PackageTracker.Application.Mapping
{
    public static class MappingExtensions
    {
        public static void AddMappings(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingsProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
