using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Events;
using TrainigSectorDataEntry.DataContext;
using TrainigSectorDataEntry.DepandecyInjection;
using TrainigSectorDataEntry.Logging;
using TrainigSectorDataEntry.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Read the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// You can now use it to configure your services, e.g., EF Core
builder.Services.AddDbContext<TrainingSectorDbContext>(options =>
    options.UseSqlServer(connectionString));


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(
        "logs/errors-.txt",
        restrictedToMinimumLevel: LogEventLevel.Error,
        rollingInterval: RollingInterval.Day,
       outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}"
    )
    .CreateLogger();


builder.Host.UseSerilog(); // Use Serilog as the logging provider

// Register the logger repository
builder.Services.AddScoped<ILoggerRepository, LoggerRepository>();

//Mapping
builder.Services.AddAutoMapper(typeof(MappingProfile));


// Increase the max request size
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100_000_000; // 100 MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 100_000_000; // 100 MB
});





// Register your repositories and services
builder.Services.AddRepositories();

var app = builder.Build();


//var basePath = builder.Configuration["FileStorage:BasePath"];

//app.UseStaticFiles(); // wwwroot

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(basePath),
//    RequestPath = "/Storage"
//});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
