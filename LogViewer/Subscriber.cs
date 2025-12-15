using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace LogViewer
{
    public class Subscriber
    {
        private readonly string _exchangeName = "logs_topic";

        public Subscriber()
        {
        }

        public void EmpezarRecibir(string subTopic)
        {
            var factory = new ConnectionFactory() { HostName = "10.22.116.128" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // 1. Declarar Exchange (Topic)
                channel.ExchangeDeclare(
                    exchange: _exchangeName,
                    type: ExchangeType.Topic,
                    durable: true);

                // 2. Crear cola temporal
                var queueName = channel.QueueDeclare().QueueName;

                // 3. Bind (Unión) usando el patrón que viene del Menú (ej: "information.#")
                channel.QueueBind(
                    queue: queueName,
                    exchange: _exchangeName,
                    routingKey: subTopic);

                Console.WriteLine($"[*] Conectado a '{_exchangeName}'.");
                Console.WriteLine($"[*] Filtro activo: '{subTopic}'");
                Console.WriteLine("[*] Esperando logs... Presione [Enter] para salir.");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;

                    // Cambio de color según el tipo de mensaje recibido
                    if (routingKey.StartsWith("error"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                    else if (routingKey.StartsWith("information"))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    }

                    // Imprimimos una sola vez de forma limpia
                    Console.WriteLine($"[{routingKey.ToUpper()}] {message}");
                    Console.ResetColor();
                };

                channel.BasicConsume(
                    queue: queueName,
                    autoAck: true,
                    consumer: consumer
                );

                Console.ReadLine();
            }
        }
    }
}