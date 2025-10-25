

namespace AppForSEII2526.API.DTOs
{
    public class CompraItemDTO
    {
        public int HerramientaId { get; set; }
        public string NombreHerramienta { get; set; }
        public string MaterialHerrramienta { get; set; }
        public decimal PrecioHerramienta { get; set; }
        public string DescripcionHerramienta { get; set; }
        public int Cantidad { get; set; }

        public CompraItemDTO(int herramientaId, string nombreHerramienta, string materialHerrramienta, decimal precioHerramienta, string descripcionHerramienta, int cantidad)
        {
            HerramientaId = herramientaId;
            NombreHerramienta = nombreHerramienta;
            MaterialHerrramienta = materialHerrramienta;
            PrecioHerramienta = precioHerramienta;
            DescripcionHerramienta = descripcionHerramienta;
            Cantidad = cantidad;
        }

        public override bool Equals(object? obj)
        {
            return obj is CompraItemDTO dTO &&
                   HerramientaId == dTO.HerramientaId &&
                   NombreHerramienta == dTO.NombreHerramienta &&
                   MaterialHerrramienta == dTO.MaterialHerrramienta &&
                   PrecioHerramienta == dTO.PrecioHerramienta &&
                   DescripcionHerramienta == dTO.DescripcionHerramienta &&
                   Cantidad == dTO.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HerramientaId, NombreHerramienta, MaterialHerrramienta, PrecioHerramienta, DescripcionHerramienta, Cantidad);
        }
    }
}
