namespace AppForSEII2526.API.Models
{
    public class Fabricante
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public List<Herramienta> Herramientas { get; set; }

    }
}
