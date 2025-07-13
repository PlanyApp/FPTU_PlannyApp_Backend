using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using DotNetEnv;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PlanyApp.API.Middleware;
using PlanyApp.Repository.Interfaces;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.Repositories;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Service.Interfaces;
using PlanyApp.Service.Mapping;
using PlanyApp.Service.Services;
using System.Collections.Generic;
using System.Reflection;
using System.Text;


Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure JWT settings from environment variables
var jwtKey = builder.Configuration["JWT:Key"];
var jwtIssuer = builder.Configuration["JWT:Issuer"];
var jwtAudience = builder.Configuration["JWT:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration is missing or invalid in environment variables");
}

// Add JWT Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
// Add Hangfire services

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Plany App API",
        Description = "API for Plany App"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.EnableAnnotations();
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register custom services and repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IChallengeService, ChallengeService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IChallengeReviewService, ChallengeReviewService>();
builder.Services.AddScoped<IPersonalChallengeApprovalService, PersonalChallengeApprovalService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddSingleton<IConversationService, ConversationService>();
builder.Services.AddScoped<IChatService>(provider => 
    new ChatService(
        provider.GetRequiredService<IConfiguration>(),
        provider.GetRequiredService<IPlanService>(),
        provider.GetRequiredService<IItemService>(),
        provider.GetRequiredService<IConversationService>()
    ));

// Add S3 client and ImageService depending on configuration
var s3Settings = builder.Configuration.GetSection("S3Settings");
var accessKey = s3Settings["AccessKey"];
var secretKey = s3Settings["SecretKey"];
var serviceUrl = s3Settings["ServiceURL"];

if (string.IsNullOrEmpty(accessKey) || accessKey == "YOUR_ACCESS_KEY" ||
    string.IsNullOrEmpty(secretKey) || secretKey == "YOUR_SECRET_KEY" ||
    string.IsNullOrEmpty(serviceUrl))
{
    Console.WriteLine("S3 settings are not configured. Using disabled image service.");
    builder.Services.AddScoped<IImageService, DisabledImageService>();
}
else
{
    var s3Config = new AmazonS3Config
    {
        ServiceURL = serviceUrl,
        ForcePathStyle = true,
        UseHttp = false,
    };

    var credentials = new BasicAWSCredentials(accessKey, secretKey);
    builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(credentials, s3Config));
    builder.Services.AddScoped<IImageService, ImageService>();
}

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IUserPackageService, UserPackageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IUserChallengeProofService, UserChallengeProofService>();
builder.Services.AddScoped<IUserChallengeProgressService, UserChallengeProgressService>();
builder.Services.AddHttpContextAccessor();

// Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new PlanyApp.API.Converters.TimeOnlyJsonConverter());
    });

// Update DbContext registration to use environment variables
builder.Services.AddDbContext<PlanyDBContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Database connection string is missing in environment variables");
    }
    options.UseSqlServer(connectionString);
});

// CORS Configuration (example: allow any origin for development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", // Define a policy name
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader();
        });
});

var app = builder.Build();
app.UseHangfireDashboard();
RecurringJob.AddOrUpdate<IInvoiceService>(
    "cancel-expired-invoices-daily",
    service => service.CancelExpiredUnpaidInvoicesAsync(),
    Cron.Daily(1) // chạy mỗi ngày lúc 01:00 sáng (UTC)
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts(); // Important for security
}

// Add the exception middleware BEFORE authentication
app.UseExceptionMiddleware();

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

// Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// after builder initialization
// Set global S3 config to disable checksum validation (and payload signing)
Amazon.AWSConfigsS3.DisableDefaultChecksumValidation = true;

app.Run();
