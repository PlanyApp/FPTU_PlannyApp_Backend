using DotNetEnv;
using PlanyApp.Repository.Models;
using PlanyApp.Repository.UnitOfWork;
using PlanyApp.Repository.Interfaces;
using PlanyApp.Repository.Repositories;
using PlanyApp.Service.Interfaces;
using PlanyApp.Service.Services;
using PlanyApp.Service.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using PlanyApp.API.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Amazon.S3;
using Amazon.Runtime;
using Amazon;

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

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value",
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

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
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
