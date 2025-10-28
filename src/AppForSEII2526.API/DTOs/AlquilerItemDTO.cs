namespace AppForSEII2526.API.DTOs
{
    public class AlquilerItemDTO
    {

        public string NombreHerramienta { get; set; }
        public string MaterialHerrramienta { get; set; }
        public decimal PrecioHerramienta { get; set; }
        public string DescripcionHerramienta { get; set; }
        public int Cantidad { get; set; }

        public AlquilerItemDTO(string nombreHerramienta, string materialHerrramienta, decimal precioHerramienta, string descripcionHerramienta, int cantidad)
        {

            NombreHerramienta = nombreHerramienta;
            MaterialHerrramienta = materialHerrramienta;
            PrecioHerramienta = precioHerramienta;
            DescripcionHerramienta = descripcionHerramienta;
            Cantidad = cantidad;
        }

        public override bool Equals(object? obj)
        {
            return obj is AlquilerItemDTO dTO &&
                   NombreHerramienta == dTO.NombreHerramienta &&
                   MaterialHerrramienta == dTO.MaterialHerrramienta &&
                   PrecioHerramienta == dTO.PrecioHerramienta &&
                   DescripcionHerramienta == dTO.DescripcionHerramienta &&
                   Cantidad == dTO.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreHerramienta, MaterialHerrramienta, PrecioHerramienta, DescripcionHerramienta, Cantidad);
        }
    }
}
