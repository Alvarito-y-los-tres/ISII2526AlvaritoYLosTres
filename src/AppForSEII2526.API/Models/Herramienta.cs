namespace AppForSEII2526.API.Models
{
    public class Herramienta
    {
        [Key]
        public int Id { get; set; }
        public int Nombre { get; set; }
        public string Material { get; set; }
        public float Precio { get; set; }
        public float TiempoReparacion { get; set; }
        
        public Fabricante Fabricante { get; set; }

        public List<OfertaItem> OfertaItems { get; set; }
    }
}
