using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Infrastructure.Identity;
using BookLAB.Infrastructure.Persistence;
using BookLAB.Infrastructure.Repositories;
using BookLAB.Infrastructure.Services;
using BookLAB.Infrastructure.Settings;
using Hangfire;
using Hangfire.PostgreSql;
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

            // ===== HANGFIRE =====
            services.AddHangfire(config =>
                config.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection"),
                new PostgreSqlStorageOptions
                {
                    SchemaName = "hangfire"
                }));
            services.AddHangfireServer();

            // Ánh xạ cấu hình từ appsettings.json vào class EmailSettings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ===== IDENTITY =====
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, Identity.CurrentUserService>();

            // ===== SERVICES =====
            services.AddScoped<IBookingService, BookingService>();
            //services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IScheduleImportService, ScheduleImportService>();
            services.AddScoped<ICalendarSyncService, GoogleCalendarSyncService>();
            services.AddScoped<QRCodeGenerator>();
            services.AddScoped<IQrManagements, QrManagements>();
            services.AddTransient<IEmailService, EmailService>();

            // ===== REPOSITORIES =====
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            // ===== BACKGROUND JOBS =====
            services.AddScoped<IBackgroundJobService, HangfireJobService>();

            return services;
        }
    }
}
