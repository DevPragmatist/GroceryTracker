using Azure.Identity;
using FastEndpoints;
using GroceryTracker.Domain;
using GroceryTracker.Infrastructure.Startup;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential()); //todo: Switch to ManagedIdentityCredential when deploying

var services = builder.Services;
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration);

services.AddEndpointsApiExplorer();
services.AddOpenApi();
services.AddDbContext<GroceryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GroceryContext") ??
    throw new InvalidOperationException("Connection string 'GroceryContext' not found.")));
services.AddApplicationServices();

services.AddAuthorization();
services.AddFastEndpoints();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}
app.UseFastEndpoints();
app.UseHttpsRedirection();


using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetRequiredService<GroceryContext>();
context.Database.Migrate();

app.Run();
