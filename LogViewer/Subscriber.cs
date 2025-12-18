using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace LogViewer
{
    public class Subscriber
    {
        private readonly string _exchangeName = "logs_topic"; //nombre del exchange al que nos vamos a conectar (debe de coincidir)

        public Subscriber()
        {
        }

        public void EmpezarRecibir(string subTopic) //subTopic: patrón por el que filtro (binding key)
        {
            var factory = new ConnectionFactory() { HostName = "172.20.10.3" };

            using (var connection = factory.CreateConnection()) //abrir conexión
            using (var channel = connection.CreateModel()) //abrir canal
            {
                // 1. Declarar Exchange (Topic)
                channel.ExchangeDeclare(
                    exchange: _exchangeName, //logs_topic
                    type: ExchangeType.Topic, //enrutamiento por patrones
                    durable: true); //exchange persiste a reinicios del Rabbit

                // 2. Crear cola temporal
                var queueName = channel.QueueDeclare().QueueName; //permite que cada logviewer tenga su propia cola

                // 3. Bind: unir cola + exchange usando el patrón que viene del Menú (ej: "information.#")
                channel.QueueBind(
                    queue: queueName,
                    exchange: _exchangeName,
                    routingKey: subTopic);

                Console.WriteLine($"[*] Conectado a '{_exchangeName}'.");
                Console.WriteLine($"[*] Filtro activo: '{subTopic}'");
                Console.WriteLine("[*] Esperando logs... Presione [Enter] para salir.");

                var consumer = new EventingBasicConsumer(channel); //creamos consumidor asociado al canal

                //Callback
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray(); //mensaje en bytes
                    var message = Encoding.UTF8.GetString(body); //convertimos a string
                    var routingKey = ea.RoutingKey; //(error, informacion...)

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

                //Empezar a consumir de la cola
                channel.BasicConsume(
                    queue: queueName,
                    autoAck: true, //mensaje procesado
                    consumer: consumer
                );

                Console.ReadLine();
            }
        }
    }
}