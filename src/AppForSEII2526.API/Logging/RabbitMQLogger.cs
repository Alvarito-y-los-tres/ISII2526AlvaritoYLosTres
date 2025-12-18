using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace TodoApi.Logging;

public class RabbitMQLogger : ILogger, IDisposable
{
    private readonly string _name;
    private readonly RabbitMQLoggerConfiguration _config;
    private readonly IConnection _connection; //conexión TCP con RabbitMQ
    private readonly IModel _channel; //canal por el que se publican los mensajes
    private readonly IBasicProperties _properties; //propiedades del mensaje

    public RabbitMQLogger(string name, RabbitMQLoggerConfiguration config)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _config = config ?? throw new ArgumentNullException(nameof(config));

        // Validación básica
        if (string.IsNullOrEmpty(_config.HostName)) throw new ArgumentException("HostName required");


        //Preparar para cómo conectarse a RabbitMQ
        var factory = new ConnectionFactory
        {
            HostName = _config.HostName,
            Port = _config.Port,
            UserName = _config.UserName,
            Password = _config.Password
        };

        _connection = factory.CreateConnection();  
        _channel = _connection.CreateModel(); //abrir canal

        // IMPORTANTE: Para que coincida con tu Subscriber, usamos el nombre "logs_topic"
        // Si prefieres usar _config.Exchange, asegúrate de poner "logs_topic" en el appsettings.json
        string exchangeName = "logs_topic";

        //declarar exchange
        _channel.ExchangeDeclare(
            exchange: exchangeName,
            type: ExchangeType.Topic, // OBLIGATORIO: Topic
            durable: true);

        _properties = _channel.CreateBasicProperties();
        _properties.Persistent = true; //el mensaje se guarda en disco
        _properties.ContentType = "application/json";
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        try
        {
            //crear objeto log
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                LogLevel = logLevel.ToString(),
                Category = _name,
                Message = formatter(state, exception),
                Exception = exception?.ToString()
            };

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(logEntry)); //convierto el objeto a JSON y luego a bytes

            // 1. Obtenemos la clave compatible con el menú (information, error)
            string routingKey = GetRoutingKey(logLevel);

            // 2. Publicamos el mensaje al exchange "logs_topic"
            _channel.BasicPublish(
                exchange: "logs_topic", // Debe coincidir con el Subscriber
                routingKey: routingKey,
                basicProperties: _properties,
                body: body
            );
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error publishing to RabbitMQ: {ex.Message}");
        }
    }

   
    
    private string GetRoutingKey(LogLevel logLevel)
    {
        return logLevel switch
        {
            // Si es Crítico, lo mandamos como "error.critical" para que el filtro "error.#" lo capture
            LogLevel.Critical => "error.critical",

            // Si es Error, "error"
            LogLevel.Error => "error",

            // Si es Info, "information" (para que coincida con "information.#")
            LogLevel.Information => "information",

            LogLevel.Warning => "warning",
            LogLevel.Debug => "debug",
            _ => "information"
        };
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}