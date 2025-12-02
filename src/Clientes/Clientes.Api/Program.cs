using System.IO;
using System.Reflection;
using Clientes.Application.Clientes.Commands.CreateCliente;
using Clientes.Application.Clientes.Interfaces;
using Clientes.Infrastructure;
using Clientes.Infrastructure.Database;
using Clientes.Infrastructure.Repositories;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://challengeintuit.torancio.com")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddMediatR(typeof(CreateClienteCommand).Assembly);
builder.Services.AddSingleton<INpgsqlConnectionFactory, NpgsqlConnectionFactory>();
builder.Services.AddScoped<IClienteWriteRepository, ClienteWriteRepository>();
builder.Services.AddScoped<IClienteReadRepository, ClienteReadRepository>();

var app = builder.Build();

Log.Information("Iniciando Clientes API");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    //await DatabaseSeeder.SeedAsync(services);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Aplicación arrancando...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación terminó por una excepción no controlada");
}
finally
{
    Log.CloseAndFlush();
}
