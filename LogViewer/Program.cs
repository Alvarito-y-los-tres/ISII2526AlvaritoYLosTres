namespace LogViewer;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var subscriber = new Subscriber();
            subscriber.EmpezarRecibir();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
		}

	}
}
