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
string connectionString = builder.Configuration.GetConnectionString("connection")?? 
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
var keyBytes = Encoding.ASCII.GetBytes(jwtKey);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new  SymmetricSecurityKey(keyBytes),
            ValidateLifetime = true
        };
    });

//builder.Services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();  //Logout
builder.Services.AddSingleton<AppGlobalVariableService>();
//builder.Services.AddScoped<AppGlobalVariableService>();

builder.Services.AddScoped<JwtService>(sp =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var jwtKey = jwtSettings["Key"];  // Retrieves the JWT Key value
    return new JwtService(jwtKey);
});



// Register other required services
builder.Services.AddSingleton<IDbConnection>(_ => new SqlConnection());
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
var app = builder.Build();



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
