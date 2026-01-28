using System.IO;
using System.Text.Json;
using AplicaciónWebTest1.Models;
using Microsoft.Extensions.Configuration;

namespace AplicaciónWebTest1.Services;

public class ConfigService
{
    private const string ConfigFileName = "appsettings.json";
    private readonly string _configPath;

    public AppConfig Settings { get; }

    public ConfigService()
    {
        _configPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName);
        Settings = LoadSettings();
    }

    private AppConfig LoadSettings()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                throw new FileNotFoundException($"No se encontró {_configPath}.");
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigFileName, optional: false, reloadOnChange: false)
                .Build();

            var settings = new AppConfig();
            configuration.Bind(settings);

            if (string.IsNullOrWhiteSpace(settings.CadenaConexion))
            {
                throw new InvalidOperationException("Cadena de conexión vacía en appsettings.json.");
            }

            return settings;
        }
        catch
        {
            var defaults = CreateDefaultSettings();
            SaveSettings(defaults);
            return defaults;
        }
    }

    private static AppConfig CreateDefaultSettings() => new()
    {
        CadenaConexion = "Server=.\\SQLSERVER2022;Database=TiendaDAM;User Id=app_dam;Password=Vegetta777;TrustServerCertificate=True;",
        SpListarProductos = "dbo.ListarProductos",
        SpInsertarProducto = "dbo.InsertarProducto",
        SpActualizarProducto = "dbo.ActualizarProducto",
        SpBorrarProducto = "dbo.BorrarProducto",
        PageSizeDefault = 10,
        PageSizeMax = 50
    };

    private void SaveSettings(AppConfig settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_configPath, json);
    }
}
