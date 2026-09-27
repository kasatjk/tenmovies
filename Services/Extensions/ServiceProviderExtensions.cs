using tenmovies.Services;
using tenmovies.Services.Interfaces;

namespace tenmovies.Services.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IMovieService, MovieService>();
            return services;
        }
    }
}