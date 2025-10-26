


namespace AppForSEII2526.API.DTOs
{
    public class CompraDetalleDTO
    {
       
        public string NombreCLiente { get; set; }
        public string ApellidoCliente { get; set; }

        public string DireccionEnvio { get; set; }
        public decimal PrecioTotal { get; set; }
        public DateTime FechaCompra { get; set; }


        public IList<CompraItemDTO> items { get; set; }

        public CompraDetalleDTO(string nombreCLiente, string apellidoCliente, string direccionEnvio, decimal precioTotal, DateTime fechaCompra, IList<CompraItemDTO> items)
        {
            NombreCLiente = nombreCLiente;
            ApellidoCliente = apellidoCliente;
            DireccionEnvio = direccionEnvio;
            PrecioTotal = precioTotal;
            FechaCompra = fechaCompra;
            this.items = items;
        }

        public override bool Equals(object? obj)
        {
            return obj is CompraDetalleDTO dTO &&
                   NombreCLiente == dTO.NombreCLiente &&
                   ApellidoCliente == dTO.ApellidoCliente &&
                   DireccionEnvio == dTO.DireccionEnvio &&
                   PrecioTotal == dTO.PrecioTotal &&
                   FechaCompra == dTO.FechaCompra &&
                   EqualityComparer<IList<CompraItemDTO>>.Default.Equals(items, dTO.items);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreCLiente, ApellidoCliente, DireccionEnvio, PrecioTotal, FechaCompra, items);
        }
    }
}
    
