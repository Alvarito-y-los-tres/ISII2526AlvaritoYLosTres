


namespace AppForSEII2526.API.DTOs
{
    public class CompraItemDTO
    {
        public int herramientaId { get; set; }
        public string NombreHerramienta { get; set; }
        public string MaterialHerrramienta { get; set; }
        public decimal PrecioHerramienta { get; set; }
        
        public string? DescripcionHerramienta { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo mayor que 0.")]
        public int Cantidad { get; set; }

        public int CompraId { get; set; }

		public CompraItemDTO(int herramientaId, string nombreHerramienta, string materialHerrramienta, decimal precioHerramienta, string? descripcionHerramienta, int cantidad, int compraId)
		{
			this.herramientaId = herramientaId;
			NombreHerramienta = nombreHerramienta;
			MaterialHerrramienta = materialHerrramienta;
			PrecioHerramienta = precioHerramienta;
			DescripcionHerramienta = descripcionHerramienta;
			Cantidad = cantidad;
			CompraId = compraId;
		}

		public CompraItemDTO()
        {
        }

        public override bool Equals(object? obj)
        {
            return obj is CompraItemDTO dTO &&
                   herramientaId == dTO.herramientaId &&
                   NombreHerramienta == dTO.NombreHerramienta &&
                   MaterialHerrramienta == dTO.MaterialHerrramienta &&
                   PrecioHerramienta == dTO.PrecioHerramienta &&
                   DescripcionHerramienta == dTO.DescripcionHerramienta &&
                   Cantidad == dTO.Cantidad;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(herramientaId, NombreHerramienta, MaterialHerrramienta, PrecioHerramienta, DescripcionHerramienta, Cantidad);
        }
    }
}
