using DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = $"Data Source={FileHelper.BasePath}app.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

//builder.Services.AddScoped<IConfigRepository, ConfigRepositoryJson>();
builder.Services.AddScoped<IConfigRepository, ConfigRepositoryDb>();
//builder.Services.AddScoped<IGameRepository, GameRepositoryJson>();
builder.Services.AddScoped<IGameRepository, GameRepositoryDb>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddSession();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
} else {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();
