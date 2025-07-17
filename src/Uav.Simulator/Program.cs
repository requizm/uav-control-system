using Uav.Simulator;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient("UavApi", client =>
{
    // Get the API base URL from configuration.
    // We'll add this to appsettings.json next.
    var apiBaseUrl = builder.Configuration["UavApi:BaseUrl"];
    client.BaseAddress = new Uri(apiBaseUrl!);
});
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();