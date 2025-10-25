

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


        public IList<CompraItemDTO> CompraItemDTO { get; set; }

        public CompraDetalleDTO(int id, string nombreCLiente, string apellidoCliente, string direccionEnvio, decimal precioTotal, DateTime fechaCompra, IList<CompraItemDTO> compraItemDTO)
        {
            Id = id;
            NombreCLiente = nombreCLiente;
            ApellidoCliente = apellidoCliente;
            DireccionEnvio = direccionEnvio;
            PrecioTotal = precioTotal;
            FechaCompra = fechaCompra;
            CompraItemDTO = compraItemDTO;
        }

        public override bool Equals(object? obj)
        {
            return obj is CompraDetalleDTO dTO &&
                   Id == dTO.Id &&
                   NombreCLiente == dTO.NombreCLiente &&
                   ApellidoCliente == dTO.ApellidoCliente &&
                   DireccionEnvio == dTO.DireccionEnvio &&
                   PrecioTotal == dTO.PrecioTotal &&
                   FechaCompra == dTO.FechaCompra &&
                   EqualityComparer<IList<CompraItemDTO>>.Default.Equals(CompraItemDTO, dTO.CompraItemDTO);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, NombreCLiente, ApellidoCliente, DireccionEnvio, PrecioTotal, FechaCompra, CompraItemDTO);
        }
    }
}
    
