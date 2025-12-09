



namespace AppForSEII2526.API.DTOs
{
    public class CompraDetalleDTO
    {
        public int Id { get; set; }
		public string NombreCLiente { get; set; }
        public string ApellidoCliente { get; set; }

        public string DireccionEnvio { get; set; }
        public decimal PrecioTotal { get; set; }
        public DateTime FechaCompra { get; set; }


        public IList<CompraItemDTO> Items { get; set; }

		public CompraDetalleDTO(int id, string nombreCLiente, string apellidoCliente, string direccionEnvio, decimal precioTotal, DateTime fechaCompra, IList<CompraItemDTO> items)
		{
			Id = id;
			NombreCLiente = nombreCLiente;
			ApellidoCliente = apellidoCliente;
			DireccionEnvio = direccionEnvio;
			PrecioTotal = precioTotal;
			FechaCompra = fechaCompra;
			Items = items;
		}

		public CompraDetalleDTO()
        {

        }

        protected bool CompareDate(DateTime date1, DateTime date2)
        {
            return (date1.Subtract(date2) < new TimeSpan(0, 1, 0));
        }

		public override bool Equals(object? obj)
		{
			return obj is CompraDetalleDTO dTO &&
				   //Id == dTO.Id &&
				   NombreCLiente == dTO.NombreCLiente &&
				   ApellidoCliente == dTO.ApellidoCliente &&
				   DireccionEnvio == dTO.DireccionEnvio &&
				   PrecioTotal == dTO.PrecioTotal &&
				   FechaCompra == dTO.FechaCompra &&
				   Items.SequenceEqual(dTO.Items);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(NombreCLiente, ApellidoCliente, DireccionEnvio, PrecioTotal, FechaCompra, Items);
		}
	}
}
    
