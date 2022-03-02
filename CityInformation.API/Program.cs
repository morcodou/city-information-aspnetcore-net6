using CityInformation.API;
using CityInformation.API.DbContexts;
using CityInformation.API.Interfaces;
using CityInformation.API.Repositories;
using CityInformation.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfoe.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.

builder
    .Services
    .AddControllers(options =>
    {
        options.ReturnHttpNotAcceptable = true;
    })
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var path = Path.Combine(AppContext.BaseDirectory, "CityInformation.API.xml");
    options.IncludeXmlComments(path);
});
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddTransient<CitiesDataStore>();
builder.Services.AddDbContext<CityInformationContext>(
    options => options
    .UseSqlite(builder
                .Configuration["ConnectionStrings:CityInformationDb"])
);
builder.Services.AddScoped<ICityRepository, CityRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        var secret = builder.Configuration["Authentification:SecretForKey"];
        var issuer = builder.Configuration["Authentification:Issuer"];
        var audience = builder.Configuration["Authentification:Audience"];
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = securityKey

        };
    });


#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromLaval", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Laval");
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

//app.MapControllers();

app.Run();
