using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Clientes.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Clientes.Tests.Clientes.Infrastructure;

[TestFixture]
public class DatabaseSeederTests
{
    [Test]
    public async Task SeedAsync_CuandoNoHayConfiguration_LoggeaWarningYRetorna()
    {
        var loggerProvider = new ListLoggerProvider();
        var services = new ServiceCollection();
        services.AddSingleton<ILoggerFactory>(new LoggerFactory(new[] { loggerProvider }));
        var provider = services.BuildServiceProvider();

        await DatabaseSeeder.SeedAsync(provider);

        loggerProvider.Messages.Should().ContainSingle(m => m.LogLevel == LogLevel.Warning && m.Message.Contains("Configuration service is not available"));
    }

    [Test]
    public async Task SeedAsync_CuandoConnectionStringEsVacia_LoggeaWarningYRetorna()
    {
        var settings = new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = "" };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var loggerProvider = new ListLoggerProvider();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<ILoggerFactory>(new LoggerFactory(new[] { loggerProvider }));
        var provider = services.BuildServiceProvider();

        await DatabaseSeeder.SeedAsync(provider);

        loggerProvider.Messages.Should().ContainSingle(m => m.LogLevel == LogLevel.Warning && m.Message.Contains("Connection string 'DefaultConnection' is not configured"));
    }

    [Test]
    public async Task SeedAsync_CuandoEnsureCreatedFalla_LoggeaErrorYPropaga()
    {
        var settings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Host=invalidhost;Database=missing;Username=none;Password=none;Timeout=1"
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var loggerProvider = new ListLoggerProvider();
        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<ILoggerFactory>(new LoggerFactory(new[] { loggerProvider }));
        var provider = services.BuildServiceProvider();

        var action = async () => await DatabaseSeeder.SeedAsync(provider);

        await action.Should().ThrowAsync<Exception>();
        loggerProvider.Messages.Any(m => m.LogLevel == LogLevel.Error && m.Message.Contains("An error occurred while seeding the database")).Should().BeTrue();
    }

    private class ListLoggerProvider : ILoggerProvider
    {
        public List<LogEntry> Messages { get; } = new();

        public ILogger CreateLogger(string categoryName) => new ListLogger(Messages);

        public void Dispose()
        {
        }

        public record LogEntry(LogLevel LogLevel, string Message, Exception? Exception);

        private class ListLogger : ILogger
        {
            private readonly List<LogEntry> _messages;

            public ListLogger(List<LogEntry> messages)
            {
                _messages = messages;
            }

            public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                _messages.Add(new LogEntry(logLevel, formatter(state, exception), exception));
            }
        }
    }

    private class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
