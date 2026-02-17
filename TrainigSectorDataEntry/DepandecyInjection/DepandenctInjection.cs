using TrainigSectorDataEntry.Models;
using TrainigSectorDataEntry.Interface;
using TrainigSectorDataEntry.Repositery;
using TrainigSectorDataEntry.Services;

namespace TrainigSectorDataEntry.DepandecyInjection
{
    public static  class DepandenctInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
        
            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IEntityImageService, EntityImageService>();

            return services;
        }
    }
}
