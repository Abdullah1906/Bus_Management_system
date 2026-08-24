using BPS.Application.Interfaces;
using BPS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BPS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
