


namespace AppForSEII2526.API.DTOs
{
    public class HerramientasParaComprarDTO
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; }

        
        public string Material { get; set; }

      
        public decimal Precio { get; set; }
        public string Fabricante { get; set; }

        public HerramientasParaComprarDTO(int id, string nombre, string material, decimal precio, string fabricante)
        {
            Id = id;
            Nombre = nombre;
            Material = material;
            Precio = precio;
            Fabricante = fabricante;
        }

        public override bool Equals(object? obj)
        {
            return obj is HerramientasParaComprarDTO dTO &&
                  // Id == dTO.Id &&
                   Nombre == dTO.Nombre &&
                   Material == dTO.Material &&
                   Precio == dTO.Precio &&
                   Fabricante == dTO.Fabricante;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Nombre, Material, Precio, Fabricante);
        }
    }
}
