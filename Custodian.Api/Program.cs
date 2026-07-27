using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Custodian.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<CustodianDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultCOnnection")));
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Custodian API")
               .WithTheme(ScalarTheme.Mars);
    });
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
