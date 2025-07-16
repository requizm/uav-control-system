using Microsoft.EntityFrameworkCore;
using Uav.Application.Repositories;
using Uav.Infrastructure.Persistence;
using Uav.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UavDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Add Repositories (Dependency Injection)
//    When a constructor asks for IDroneRepository, give it a DroneRepository.
//    Scoped means one instance per HTTP request.
builder.Services.AddScoped<IDroneRepository, DroneRepository>();
builder.Services.AddScoped<IMissionRepository, MissionRepository>();


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();