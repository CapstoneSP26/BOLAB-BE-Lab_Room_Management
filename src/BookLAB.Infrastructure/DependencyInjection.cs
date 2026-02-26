using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using BookLAB.Application.Common.Interfaces.Persistence;
using BookLAB.Infrastructure.Persistence;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Infrastructure.Repositories;


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

            services.AddScoped<IBookLABDbContext>(provider =>
                provider.GetRequiredService<BookLABDbContext>());

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ===== IDENTITY =====
            //services.AddHttpContextAccessor();
            //services.AddScoped<ICurrentUserService, CurrentUserService>();

            // ===== SERVICES =====
            //services.AddScoped<IDateTime, DateTimeService>();
            //services.AddScoped<IEmailService, EmailService>();
            // services.AddScoped<IFileStorageService, LocalFileStorageService>();

            return services;
        }
    }
}
