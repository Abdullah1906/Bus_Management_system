using BPS.Application.Interfaces;
using BPS.Application.Services;
using BPS.Infrastructure.Data;
using BPS.Infrastructure.Repositories;
using BPS.Infrastructure.Security;
using BPS.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using BPS.Infrastructure.BackgroundServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddSingleton(
                new SqlConnectionFactory(connectionString));

            // Repositories
            services.AddScoped<IPlaceRepository, PlaceRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITripRepository, TripRepository>();
            services.AddScoped<IReportRepository,ReportRepository>();
            services.AddScoped<ITripScheduleRepository,TripScheduleRepository>();
            services.AddScoped<IBookingRepository,BookingRepository>();
            services.AddScoped<IBusRepository,BusRepository>();
            services.AddScoped<IBusSeatRepository,BusSeatRepository>();

            // Security
            services.AddScoped<IPasswordHasher, PasswordHasherService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            // Services
            services.AddScoped<IUserSeeder, UserSeeder>();
            services.AddHostedService<SeatLockCleanupWorker>();

            return services;
        }
    }
}
