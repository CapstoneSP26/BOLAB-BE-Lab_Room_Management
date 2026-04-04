using BookLAB.API.Middlewares;
using BookLAB.Application;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Infrastructure;
using BookLAB.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using QRCoder;
using System.Text;
using BookLAB.Infrastructure.Persistence;
using BookLAB.Infrastructure.Repositories;
using BookLAB.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Yêu cầu đăng nhập bằng JWT Bearer
    //options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/";
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Override lại ChallengeScheme để đăng nhập bằng Cookie (cơ mà trong AuthController.GoogleLogin trả về Result.Challenge với authentication scheme là Google nên sẽ trỏ về bên Google yêu cầu xác nhận bên đó trước, không dùng Cookie này nữa)
    }).AddJwtBearer(x =>
    {
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:SecretKey").Value)),
            ClockSkew = TimeSpan.Zero
        };

        x.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                ctx.Request.Cookies.TryGetValue("accessToken", out var accessToken);
                if (!string.IsNullOrEmpty(accessToken))
                    ctx.Token = accessToken;

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("AcademicOffice",
        policyBuilder => policyBuilder.RequireAssertion(
            context => context.User.HasClaim(claim => claim.Type == "Role") &&
            context.User.FindFirst(claim => claim.Type == "Role").Value == "1"));

    options.AddPolicy("AcademicOffice_LabManager",
        policyBuilder => policyBuilder.RequireAssertion(
            context => context.User.HasClaim(claim => claim.Type == "Role")
            && (context.User.FindFirst(claim => claim.Type == "Role").Value == "1"
            || context.User.FindFirst(claim => claim.Type == "Role").Value == "2")));

    options.AddPolicy("AcademicOffice_LabManager_Lecturer",
        policyBuilder => policyBuilder.RequireAssertion(
            context => context.User.HasClaim(claim => claim.Type == "Role")
            && (context.User.FindFirst(claim => claim.Type == "Role").Value == "1"
            || context.User.FindFirst(claim => claim.Type == "Role").Value == "2"
            || context.User.FindFirst(claim => claim.Type == "Role").Value == "3")));

    options.AddPolicy("AcademicOffice_Lecturer",
        policyBuilder => policyBuilder.RequireAssertion(
            context => context.User.HasClaim(claim => claim.Type == "Role")
            && (context.User.FindFirst(claim => claim.Type == "Role").Value == "1"
            || context.User.FindFirst(claim => claim.Type == "Role").Value == "3")));

    options.AddPolicy("Lecturer",
        policyBuilder => policyBuilder.RequireAssertion(
            context => context.User.HasClaim(claim => claim.Type == "Role")
            && context.User.FindFirst(claim => claim.Type == "Role").Value == "3"));

    options.AddPolicy("Student",
        policyBuilder => policyBuilder.RequireAssertion(
            context => context.User.HasClaim(claim => claim.Type == "Role")
            && context.User.FindFirst(claim => claim.Type == "Role").Value == "4"));
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", options =>
    {
        options.AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials()
               .WithOrigins(
                   "https://localhost:5173",
                   "http://localhost:5173",
                   "http://localhost:5176",
                   "http://localhost:5174",
                   "http://localhost:5175",
                   "http://localhost:3000"
               );
    });
});

builder.Services.AddHttpClient("BackendApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
});

// Application layer
builder.Services.AddApplicationServices();

// Infrastructure layer
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddDbContext<BookLABDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("BookLAB.API"));
});
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<QRCodeGenerator>();
builder.Services.AddScoped<QrManagements>();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
