using BookShop.Application;
using BookShop.Infrastructure;
using BookShop.Infrastructure.Identity;
using BookShop.Web.Validators.Images;
using FileSignatures;
using FileSignatures.Formats;
using Mapster;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMapster();

builder.Services.AddControllersWithViews();
builder.Services.AddApplication();
builder.Services.AddScoped<ImageFileValidator>();
builder.Services.AddInfrastructure(builder.Configuration);

var imageFormats = FileFormatLocator.GetFormats().OfType<Image>();
var fileFormatInspector = new FileFormatInspector(imageFormats);
builder.Services.AddSingleton<IFileFormatInspector>(fileFormatInspector);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleSeeder =scope.ServiceProvider.GetRequiredService<RoleSeeder>();

    await roleSeeder.SeedAsync();

    var adminSeeder =scope.ServiceProvider.GetRequiredService<AdminSeeder>();

    await adminSeeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
