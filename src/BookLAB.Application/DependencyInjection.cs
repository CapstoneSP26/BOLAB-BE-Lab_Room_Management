using System.Reflection;
using BookLAB.Application.Common.Behaviors;
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

        return services;
    }
}