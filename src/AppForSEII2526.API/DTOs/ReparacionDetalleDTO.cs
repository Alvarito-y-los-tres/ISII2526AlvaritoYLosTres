


namespace AppForSEII2526.API.DTOs
{
    public class ReparacionDetalleDTO
    {
       
        public string NombreCLiente { get; set; }
        public string ApellidoCliente { get; set; }
        public decimal PrecioTotal { get; set; }
        public DateTime FechaEntrega { get; set; }
        public DateTime FechaRecogida { get; set; }


        public IList<ReparacionItemDTO> items { get; set; }

        public ReparacionDetalleDTO(string nombreCLiente, string apellidoCliente, decimal precioTotal, DateTime fechaEntrega, DateTime fechaRecogida, IList<ReparacionItemDTO> items)
        {
            NombreCLiente = nombreCLiente;
            ApellidoCliente = apellidoCliente;
            PrecioTotal = precioTotal;
            FechaEntrega = fechaEntrega;
            FechaRecogida = fechaRecogida;
            this.items = items;
        }

        public override bool Equals(object? obj)
        {
            return obj is ReparacionDetalleDTO dTO &&
                   NombreCLiente == dTO.NombreCLiente &&
                   ApellidoCliente == dTO.ApellidoCliente &&
                   PrecioTotal == dTO.PrecioTotal &&
                   FechaEntrega == dTO.FechaEntrega &&
                   FechaRecogida == dTO.FechaRecogida &&
                   EqualityComparer<IList<ReparacionItemDTO>>.Default.Equals(items, dTO.items);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreCLiente, ApellidoCliente, PrecioTotal, FechaEntrega, FechaRecogida, items);
        }
    }
}
    
