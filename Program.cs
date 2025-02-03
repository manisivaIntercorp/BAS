using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using WebApi.Services;

//JWT Authentication
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


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
string connectionString = builder.Configuration.GetConnectionString("connection");

// Register UowOrganisation with DI
builder.Services.AddScoped<IUowOrganisation>(sp => new UowOrganisation(connectionString));
// Register UowEmail with DI
builder.Services.AddScoped<IUowEmailTemplate>(sp => new UowEmailTemplate(connectionString));

//Added Email Services with DI
builder.Services.AddScoped<EmailServices>();

//Added Session Services with DI
builder.Services.AddScoped<SessionService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<IDbContextService, DbContextService>();
builder.Services.AddControllers();

// JWT Authendification start
var jwtKey = "123456789123456789123456789123456789123"; // Replace with a strong key
var key = Encoding.ASCII.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});


// JWT Authendification END

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseSession();
}


//Session
app.UseSession();
app.UseRouting();
app.UseAuthentication(); //JWT Authentication
app.UseAuthorization(); 
app.MapControllers();
app.Run();