using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Infrastructure.Identity;
using BookLAB.Infrastructure.Persistence;
using BookLAB.Infrastructure.Repositories;
using BookLAB.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QRCoder;

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

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ===== IDENTITY =====
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, Identity.CurrentUserService>();

            // ===== SERVICES =====
            services.AddScoped<IBookingService, BookingService>();
            //services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IScheduleImportService, ScheduleService>();
            services.AddScoped<ICalendarSyncService, GoogleCalendarSyncService>();
            services.AddScoped<QRCodeGenerator>();
            services.AddScoped<IQrManagements, QrManagements>();

            // ===== REPOSITORIES =====
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IBookingRepository, BookingRepository>();



            return services;
        }
    }
}
