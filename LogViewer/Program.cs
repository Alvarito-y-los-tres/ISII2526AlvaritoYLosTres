using System;
using System.Collections.Generic;

namespace LogViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== RabbitMQ Log Viewer ===");
                Console.WriteLine("Seleccione el tipo de logs a escuchar:");
                Console.WriteLine("1. Información (Info)");
                Console.WriteLine("2. Errores (Error)");
                Console.WriteLine("3. Ambos (Info + Error)");
                Console.Write("\nOpción: ");

                var input = Console.ReadKey();
                Console.WriteLine("\n");

                // Lista de Routing Keys (claves de enrutamiento)
                string routingKeys = "";

                switch (input.KeyChar)
                {
                    case '1':
                        routingKeys = "information.#";
                        Console.WriteLine(">> Suscribiéndose solo a logs de INFORMACIÓN...");
                        break;
                    case '2':
                        routingKeys = "error.#";
                        Console.WriteLine(">> Suscribiéndose solo a logs de ERROR...");
                        break;
                    case '3':
                        routingKeys = "#";
                        Console.WriteLine(">> Suscribiéndose a AMBOS tipos de logs...");
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Saliendo.");
                        return;
                }

                var subscriber = new Subscriber();

                // Pasamos las claves seleccionadas al método
                subscriber.EmpezarRecibir(routingKeys);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error crítico: {ex.Message}");
            }
        }
    }
}