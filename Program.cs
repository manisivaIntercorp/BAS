using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DataAccessLayer.Services;
using System.Data;
using Microsoft.Data.SqlClient;

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

//Added ICS Services with DI
builder.Services.AddScoped<ICS>();
// Register MailServer with DI
builder.Services.AddScoped<MailServer>(sp => new MailServer(connectionString));


builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// JWT Authentication setup
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtKey = jwtSettings["Key"];  // Retrieves the Key value
var keyBytes = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});
builder.Services.AddSingleton<AppGlobalVariableService>();

// Register other required services
builder.Services.AddSingleton<IDbConnection>(_ => new SqlConnection());
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Note: Remove the middleware registration from DI.
// builder.Services.AddScoped<ConnectionStringMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Make sure session is available before invoking middleware that relies on it.
app.UseSession();

// The middleware is added here via the framework extension method,
// which will supply the required RequestDelegate automatically.
app.UseMiddleware<ConnectionStringMiddleware>();

app.UseRouting();
app.UseAuthentication(); // JWT Authentication
app.UseAuthorization();
app.MapControllers();
app.Run();
