using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Uav.Application.Common;
using Uav.Application.Repositories;
using Uav.Infrastructure.Persistence;
using Uav.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UavDbContext>(options =>
    options.UseNpgsql(connectionString));

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson(); 
var dataSource = dataSourceBuilder.Build();

builder.Services.AddDbContext<UavDbContext>(options =>
    options.UseNpgsql(dataSource));

builder.Services.AddScoped<IDroneRepository, DroneRepository>();
builder.Services.AddScoped<IMissionRepository, MissionRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();