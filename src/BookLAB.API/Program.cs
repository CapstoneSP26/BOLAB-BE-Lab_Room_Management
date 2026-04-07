using BookLAB.API.Middlewares;
using BookLAB.Application;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Infrastructure;
using BookLAB.Infrastructure.Identity;
using BookLAB.Infrastructure.Persistence;
using BookLAB.Infrastructure.Repositories;
using BookLAB.Infrastructure.Services;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QRCoder;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // API yêu cầu đăng nhập bằng JWT Bearer
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // cần cho Google OAuth sign-in flow
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "BookLab.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;
        options.SlidingExpiration = true;
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.CallbackPath = "/signin-google";
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        options.CorrelationCookie.HttpOnly = true;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.CorrelationCookie.SameSite = SameSiteMode.None;

        options.NonceCookie.HttpOnly = true;
        options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.NonceCookie.SameSite = SameSiteMode.None;

        options.Events.OnRedirectToAuthorizationEndpoint = context =>
        {
            var forcedRedirectUri = builder.Configuration["Authentication:Google:RedirectUri"];
            var targetUri = context.RedirectUri;

            if (!string.IsNullOrWhiteSpace(forcedRedirectUri)
                && Uri.TryCreate(forcedRedirectUri, UriKind.Absolute, out var redirectUriValue))
            {
                targetUri = System.Text.RegularExpressions.Regex.Replace(
                    targetUri,
                    @"redirect_uri=[^&]*",
                    $"redirect_uri={Uri.EscapeDataString(redirectUriValue.ToString())}");
            }
            else
            {
                targetUri = targetUri.Replace("redirect_uri=http%3A%2F%2F", "redirect_uri=https%3A%2F%2F");
            }

            context.Response.Redirect(targetUri);
            return Task.CompletedTask;
        };
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
                   "https://localhost:5174",
                   "http://localhost:5175",
                   "http://localhost:3000",
                   "https://bolab-fe-lab-room-management.vercel.app"
               );
    });
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
    options.HttpOnly = HttpOnlyPolicy.Always;
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

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    RequireHeaderSymmetry = false,
    KnownNetworks = { },
    KnownProxies = { }
});

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseCookiePolicy();
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard();

app.Run();
