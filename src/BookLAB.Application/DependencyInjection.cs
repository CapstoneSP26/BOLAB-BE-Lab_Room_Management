using System.Reflection;
using BookLAB.Application.Common.Behaviors;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Mappings;
using BookLAB.Application.Common.Policies;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace BookLAB.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 1. Đăng ký AutoMapper (Tự động tìm các Profile ánh xạ DTO)
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(typeof(MappingProfile));

        // 2. Đăng ký FluentValidation (Tự động tìm các file Validator cụ thể cho từng Command/Query)
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // 3. Cấu hình MediatR và Pipeline Behaviours
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // Đăng ký Behaviours theo đúng thứ tự thực thi (Middleware-like)
            // Lưu ý: UnhandledException nên ở ngoài cùng để bắt lỗi toàn hệ thống
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));

            // Authorization nên chạy trước khi Validate dữ liệu để tránh tốn tài nguyên
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));

            // Validation chạy trước khi vào Handler để đảm bảo dữ liệu sạch
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // Performance check cuối cùng để đo lường hiệu suất của Handler
            //cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });

        services.AddScoped<IPolicyEngine, PolicyEngine>();

        var assembly = Assembly.GetExecutingAssembly();
        var handlerTypes = assembly.GetTypes()
            .Where(t => typeof(IBookingPolicyHandler).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract);

        foreach (var type in handlerTypes)
        {
            // Register as Scoped so they can inject IUnitOfWork or other services
            services.AddScoped(typeof(IBookingPolicyHandler), type);
        }

        return services;
    }
}