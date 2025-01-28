using DataAccessLayer;
using DataAccessLayer.Uow.Implementation;
using DataAccessLayer.Uow.Interface;
using NLog.Extensions.Logging;
using NLog.Web;


var builder = WebApplication.CreateBuilder(args);

// Bind the connection string from configuration
string connectionString = builder.Configuration.GetConnectionString("connection");

// Register UowOrganisation with DI
builder.Services.AddScoped<IUowOrganisation>(sp => new UowOrganisation(connectionString));

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();


