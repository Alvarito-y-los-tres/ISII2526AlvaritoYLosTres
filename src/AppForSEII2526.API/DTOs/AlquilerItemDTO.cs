namespace AppForSEII2526.API.DTOs
{
    public class AlquilerItemDTO
    {

        public string NombreHerramienta { get; set; }
        public string MaterialHerrramienta { get; set; }
        public decimal PrecioHerramienta { get; set; }
        
        public int Cantidad { get; set; }

        public AlquilerItemDTO(string nombreHerramienta, string materialHerrramienta, decimal precioHerramienta, int cantidad)
        {

            NombreHerramienta = nombreHerramienta;
            MaterialHerrramienta = materialHerrramienta;
            PrecioHerramienta = precioHerramienta;
            Cantidad = cantidad;
        }

        public override bool Equals(object? obj)
        {
            return obj is AlquilerItemDTO dTO &&
                   NombreHerramienta == dTO.NombreHerramienta &&
                   MaterialHerrramienta == dTO.MaterialHerrramienta &&
                   PrecioHerramienta == dTO.PrecioHerramienta &&
                   Cantidad == dTO.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreHerramienta, MaterialHerrramienta, PrecioHerramienta, Cantidad);
        }
    }
}
