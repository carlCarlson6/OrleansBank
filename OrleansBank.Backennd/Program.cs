using OrleansBank.Backennd.Accounts;
using OrleansBank.Backennd.Infrastructure.Orleans;
using OrleansBank.Backennd.Infrastructure.RavenDb;

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddOrleans();

builder.Services
    .AddRavenDb(builder.Configuration)
    .AddRavenDbGrainStateStorage();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapOpenAccountEndpoint();
app.MapGetAccountStatusEndpoint();
app.MapMakeDepositStatusEndpoint();

app.Run();
