using vue_ts;
using Microsoft.EntityFrameworkCore;
using vue_ts.Services.DetailService;
using vue_ts.Services.ImageService;
using vue_ts.Services;
using vue_ts.Models;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});


builder.Services.AddScoped<IDetailService, DetailService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.Configure<ShoutOutSmsConfiguration>(
    builder.Configuration.GetSection("ShoutOutSmsConfiguration")
);
builder.Services.AddHttpClient<ISmsService, SmsService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy( policy =>
    {
        policy.WithOrigins("http://localhost:5173")  
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseCors();

app.UseAuthorization();

app.MapControllers();


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "assets")
    ),
    RequestPath = "/assets"
});

app.Run();