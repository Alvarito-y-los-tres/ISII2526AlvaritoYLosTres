namespace AppForSEII2526.API.DTOs
{
    public class ReparacionItemDTO
    {
        // nombre precio descripcionProblema cantidad
        public string NombreHerramienta { get; set; }
        public decimal PrecioReparacion { get; set; }
        public string DescripcionProblema { get; set; }
        public int Cantidad { get; set; }

        public ReparacionItemDTO(string nombreHerramienta, decimal precioReparacion, string descripcionProblema, int cantidad)
        {
            NombreHerramienta = nombreHerramienta;
            PrecioReparacion = precioReparacion;
            DescripcionProblema = descripcionProblema;
            Cantidad = cantidad;
        }

        public override bool Equals(object? obj)
        {
            return obj is ReparacionItemDTO dTO &&
                   NombreHerramienta == dTO.NombreHerramienta &&
                   PrecioReparacion == dTO.PrecioReparacion &&
                   DescripcionProblema == dTO.DescripcionProblema &&
                   Cantidad == dTO.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreHerramienta, PrecioReparacion, DescripcionProblema, Cantidad);
        }

    }
}    