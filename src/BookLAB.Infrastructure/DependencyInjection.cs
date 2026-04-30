using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Integration;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Jobs.Bookings;
using BookLAB.Infrastructure.BackgroundJobs;
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

            services.AddHangfireServer(options =>
            {
                // Với dự án BookLAB, chỉ nên để từ 5-10 workers
                options.WorkerCount = Environment.ProcessorCount;

                // Đặt tên server để dễ quản lý trên Dashboard
                options.ServerName = "BookLAB_Background_Server";
            });

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
            services.AddScoped<IUserImportService, UserImportService>();
            services.AddScoped<ICalendarSyncService, GoogleCalendarSyncService>();
            services.AddScoped<QRCodeGenerator>();
            services.AddScoped<IQrManagements, QrManagements>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<INotificationService, SignalRNotificationService>();
            services.AddScoped<IAIBookingService, AdvancedAIBookingService>();


            // ===== REPOSITORIES =====
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();

            // ===== BACKGROUND JOBS =====
            services.AddScoped<AutoRejectBookingJob>();

            services.AddScoped<RecurringJobScheduler>();

            services.AddScoped<IBackgroundJobService, HangfireJobService>();

            services.AddHostedService<JobHostedService>();

            return services;
        }
    }
}
