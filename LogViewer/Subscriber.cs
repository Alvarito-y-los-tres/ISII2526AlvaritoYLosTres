using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LogViewer
{
	public class Subscriber
	{
		
		private readonly string _exchangeName = "logs";
		public Subscriber()
		{

		}

		public void EmpezarRecibir()
		{
			//creamos la conexion
			var factory = new ConnectionFactory() { HostName = "localhost" };

			var connection = factory.CreateConnection();
			var channel = connection.CreateModel();

			//creamos el exchange
			channel.ExchangeDeclare(
				exchange: _exchangeName,
				type: ExchangeType.Fanout,
				durable: true);

			//creamos la cola
			var queueName = channel.QueueDeclare(
				queue: "",
				durable: false,
				exclusive: true,
				autoDelete: true,
				arguments: null).QueueName;

			//bindamos la cola al exchange
			channel.QueueBind(
				queue: queueName,
				exchange: _exchangeName,
				routingKey: "");

			var consumer = new EventingBasicConsumer(channel);

			Console.WriteLine($"[*] Suscrito a '{_exchangeName}'. Cola: {queueName}. Esperando logs...");

			consumer.Received += (model, ea) =>
			{
				var body = ea.Body.ToArray(); //contenido del mensaje (array de bytes) 
				var message = Encoding.UTF8.GetString(body); //se convierte de vuelta a string 
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine($"[LOG RECIBIDO]: {message}");
				Console.ResetColor();
				Console.WriteLine($"Pedido recibido: {message}");
				Console.WriteLine($"[LOG RECIBIDO]: {message}");


			};

			channel.BasicConsume(
			queue: queueName,
			autoAck: true, // Confirmación automática de recepción del mensaje 
			consumer: consumer
			);

			Console.WriteLine(" Presiona [Enter] para salir.");
			Console.ReadLine();

			channel.Close();
			connection.Close();


		}
	}
	}
