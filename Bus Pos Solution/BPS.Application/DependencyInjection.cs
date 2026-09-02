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
            services.AddScoped<ITripService, TripService>();
            services.AddScoped<IPlaceService, PlaceService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ITripScheduleService,TripScheduleService>();
            services.AddScoped<IBookingService,BookingService>();
            services.AddScoped<IBusService,BusService>();
            services.AddScoped<IBusSeatService,BusSeatService>();
            services.AddScoped<IRouteService,RouteService>();

            return services;
        }
    }
}
