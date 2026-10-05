using Amazon.Runtime;
using Amazon.S3;
using BookShop.Application.Сommon.Interfaces;
using BookShop.Infrastructure.Identity;
using BookShop.Infrastructure.Persistance;
using BookShop.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BookShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

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
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<RoleSeeder>();

        return services;
    }
}
