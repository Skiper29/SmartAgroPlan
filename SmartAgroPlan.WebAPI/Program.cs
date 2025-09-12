using Scalar.AspNetCore;
using SmartAgroPlan.WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureApplication(builder);

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCustomServices();
builder.Services.ConfigureSerilog(builder);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseHsts();
}

// await app.SeedDataAsync();

app.UseCors();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
