using Relay.Chassis.Hosting;
using Relay.Source.Application;
using Relay.Source.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.AddRelayServiceDefaults();
builder.Services.AddSourceApplication().AddSourceInfrastructure();

var app = builder.Build();
app.MapRelayServiceDefaults();

await app.RunAsync();
