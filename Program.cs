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




builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDetailService, DetailService>();
builder.Services.AddControllers();
builder.Services.AddScoped<IImageService, ImageService>();

builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.Configure<ShoutOutSmsConfiguration>(
    builder.Configuration.GetSection("ShoutOutSmsConfiguration")
);
builder.Services.AddHttpClient<ISmsService, SmsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAnyOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "assets")),
    RequestPath = "/assets"
});
