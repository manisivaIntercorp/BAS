using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DataAccessLayer.Services;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.OpenApi.Models;
using WebApi.Services.Implementation;
using WebApi.Services.Interface;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;



var builder = WebApplication.CreateBuilder(args);

// Add session services
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Prevents JavaScript access
    options.Cookie.IsEssential = true; // Ensures session works without tracking consent
});

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor(); // Required for accessing session in services

// Bind the connection string from configuration
string connectionString = builder.Configuration.GetConnectionString("connection") ??
    throw new InvalidOperationException("Database connection string is missing."); ;

// Register your other services (UoWs, EmailServices, etc.)
builder.Services.AddScoped<IUowOrganisation>(sp => new UowOrganisation(connectionString));
builder.Services.AddScoped<IUowEmailTemplate>(sp => new UowEmailTemplate(connectionString));
builder.Services.AddScoped<EmailServices>();
builder.Services.AddScoped<SessionService>();

//Added Encryption and Decryption Services with DI
builder.Services.AddScoped<EncryptedDecrypt>();
// Register MailServer with DI
builder.Services.AddScoped<MailServer>(sp => new MailServer(connectionString));


builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
// Swagger Auth Start
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Add JWT token support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});
//SwaggerDoc Auth End
// JWT Authentication setup
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Key"];  // Retrieves the Key value
var keyBytes = Encoding.ASCII.GetBytes(jwtKey?.ToString() ?? "");


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true
        };
    });

builder.Services.AddScoped<GUID>();
builder.Services.AddSingleton<AppGlobalVariableService>();
//builder.Services.AddScoped<AppGlobalVariableService>();

builder.Services.AddSingleton<TokenBlacklistService>(); // Logout
builder.Services.AddScoped<JwtService>(provider =>
{
    var configuration = builder.Configuration.GetSection("JwtSettings");
    var secretKey = configuration["Key"];

    if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
    {
        throw new ArgumentException("JWT Secret key must be at least 32 characters.");
    }

    var tokenBlacklistService = provider.GetRequiredService<TokenBlacklistService>();
    return new JwtService(secretKey, tokenBlacklistService);
});


// Register other required services
builder.Services.AddSingleton<IDbConnection>(_ => new SqlConnection());
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//AuditLog start
builder.Services.AddScoped<IUowAuditLog, UowAuditLog>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddHttpContextAccessor();
//AuditLog End

builder.Services.AddScoped<IUowTranslation>(provider =>
    new UowTranslation(connectionString)
);


builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
// Register TranslationService
builder.Services.AddScoped<TranslationService>();

//-----------------------
builder.Services.AddControllers()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "en", "fr", "es", "ta-IN" };
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = options.SupportedCultures;
});
//-------------------------
// Localization END


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var translationService = scope.ServiceProvider.GetRequiredService<TranslationService>();
    var uowTranslation = scope.ServiceProvider.GetRequiredService<IUowTranslation>();

    await translationService.GenerateResourceFiles(uowTranslation);
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //SwaggerDoc Auth Start
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
    //SwaggerDoc Auth End
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseRequestLocalization();
// Make sure session is available before invoking middleware that relies on it.
app.UseSession();

// The middleware is added here via the framework extension method,
// which will supply the required RequestDelegate automatically.

// Middleware pipeline
app.UseMiddleware<ConnectionStringMiddleware>();


//app.UseMiddleware<TokenBlacklistMiddleware>();  //Logout
app.UseRouting();

app.UseAuthentication(); // JWT Authentication
app.UseAuthorization();
app.MapControllers();
app.Run();
