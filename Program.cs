using DataAccessLayer;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using NLog.Extensions.Logging;
using NLog.Web;


var builder = WebApplication.CreateBuilder(args);
//var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

//var logger =  NLog.LogManager.Setup().LoadConfigurationFromAppSettings();

//logger
// Configure NLog for ASP.NET Core
builder.Logging.ClearProviders(); // Clears default logging providers.
builder.Host.UseNLog(); // Use NLog as the logging provider.
// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddScoped( IUowNationality, UowNationality);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
    
    var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(option => {
    option.AllowAnyOrigin();
    option.AllowAnyMethod();
    option.AllowAnyHeader();
});
app.UseAuthorization();

app.MapControllers();

app.Run();

