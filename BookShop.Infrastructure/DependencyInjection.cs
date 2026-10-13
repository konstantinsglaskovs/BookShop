using Amazon.Runtime;
using Amazon.S3;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Infrastructure.BackgroundServices;
using BookShop.Infrastructure.Identity;
using BookShop.Infrastructure.Payment;
using BookShop.Infrastructure.Persistance;
using BookShop.Infrastructure.Services;
using BookShop.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stripe;

namespace BookShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

        services.Configure<S3StorageOptions>(configuration.GetSection(S3StorageOptions.SectionName));

        var s3Options = configuration
            .GetSection(S3StorageOptions.SectionName)
            .Get<S3StorageOptions>()
            ?? throw new InvalidOperationException(
                "S3 storage configuration is missing.");

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = s3Options.ServiceUrl,
                ForcePathStyle = true
            };

            return new AmazonS3Client(s3Options.AccessKey,s3Options.SecretKey,config);
        });

        services.AddScoped<IFileStorage, S3FileStorage>();
        services.AddScoped<IIdentityService, Identity.IdentityService>();
        services.AddScoped<RoleSeeder>();
        services.AddScoped<AdminSeeder>();
        services.AddScoped<IUserManagementService, UserManagementService>();

        services.Configure<StripeSettings>(configuration.GetSection("Stripe"));
        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<StripeSettings>>().Value;

            return new StripeClient(settings.SecretKey);
        });
        services.AddScoped<IPaymentService, StripePaymentService>();
        services.AddHostedService<PendingOrderCleanupService>();

        return services;
    }
}
