using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using WebApi.Services;


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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//Session
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();