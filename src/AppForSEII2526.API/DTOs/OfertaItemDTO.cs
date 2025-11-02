



namespace AppForSEII2526.API.DTOs
{
    public class OfertaItemDTO
    {
        public string NombreHerramienta { get; set; }
        public string MaterialHerramienta { get; set; }
        public string FabricanteHerramienta { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioFinal { get; set; }
        public float Porcentaje { get; set; }
        public int OfertaId { get; set; }

        public OfertaItemDTO(string nombreHerramienta, string materialHerramienta, string fabricanteHerramienta, decimal precio, decimal precioFinal, float porcentaje, int ofertaId)
        {
            NombreHerramienta = nombreHerramienta;
            MaterialHerramienta = materialHerramienta;
            FabricanteHerramienta = fabricanteHerramienta;
            Precio = precio;
            PrecioFinal = precioFinal;
            Porcentaje = porcentaje;
            OfertaId = ofertaId;
        }

        public override bool Equals(object? obj)
        {
            return obj is OfertaItemDTO dTO &&
                   NombreHerramienta == dTO.NombreHerramienta &&
                   MaterialHerramienta == dTO.MaterialHerramienta &&
                   FabricanteHerramienta == dTO.FabricanteHerramienta &&
                   Precio == dTO.Precio &&
                   PrecioFinal == dTO.PrecioFinal &&
                   Porcentaje == dTO.Porcentaje &&
                   OfertaId == dTO.OfertaId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreHerramienta, MaterialHerramienta, FabricanteHerramienta, Precio, PrecioFinal, Porcentaje, OfertaId);
        }
    }
}
