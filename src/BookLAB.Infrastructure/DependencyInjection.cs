using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Infrastructure.Persistence;
using BookLAB.Infrastructure.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Infrastructure.Repositories;
using BookLAB.Infrastructure.Services;

namespace BookLAB.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ===== DATABASE =====
            services.AddDbContext<BookLABDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<BookLABDbContext>(provider =>
                provider.GetRequiredService<BookLABDbContext>());

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ===== IDENTITY =====
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, Identity.CurrentUserService>();

            // ===== SERVICES =====
            services.AddScoped<IBookingService, BookingService>();
            //services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<ICalendarSyncService, GoogleCalendarSyncService>();

            // ===== REPOSITORIES =====
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            return services;
        }
    }
}
