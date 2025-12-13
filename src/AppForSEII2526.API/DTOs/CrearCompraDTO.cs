
namespace AppForSEII2526.API.DTOs
{
    public class CrearCompraDTO
    {
        [Required(ErrorMessage = "El Nombre es obligatorio.")]
        public string NombreCliente { get; set; }
        [Required(ErrorMessage = "El Apellido es obligatorio.")]
        public string ApellidoCliente { get; set; }
        [Required(ErrorMessage = "La DireccionEnvio es obligatoria.")]
        public string DireccionEnvio { get; set; }
        public int MetodoPagoId { get; set; }
        public string? NumTelefono { get; set; }
        public string? CorreoElectronico { get; set; }
        public decimal PrecioTotal { get; set; }
        public List<CompraItemDTO> CompraItems { get; set; }

        public CrearCompraDTO()
        {
            CompraItems = new List<CompraItemDTO>();
        }

        public CrearCompraDTO(string nombreCliente, string apellidoCliente, string direccionEnvio, int metodoPagoId, string? numTelefono, string? correoElectronico, decimal precioTotal, List<CompraItemDTO> compraItems)
        {
            NombreCliente = nombreCliente;
            ApellidoCliente = apellidoCliente;
            DireccionEnvio = direccionEnvio;
            MetodoPagoId = metodoPagoId;
            NumTelefono = numTelefono;
            CorreoElectronico = correoElectronico;
            PrecioTotal = precioTotal;
            CompraItems = compraItems;
        }

        public override bool Equals(object? obj)
        {
            return obj is CrearCompraDTO dTO &&
                   NombreCliente == dTO.NombreCliente &&
                   ApellidoCliente == dTO.ApellidoCliente &&
                   DireccionEnvio == dTO.DireccionEnvio &&
                   MetodoPagoId == dTO.MetodoPagoId &&
                   NumTelefono == dTO.NumTelefono &&
                   CorreoElectronico == dTO.CorreoElectronico &&
                   PrecioTotal == dTO.PrecioTotal &&
                   CompraItems.SequenceEqual(dTO.CompraItems);
            
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreCliente, ApellidoCliente, DireccionEnvio, MetodoPagoId, NumTelefono, CorreoElectronico, PrecioTotal, CompraItems);
        }
    }
}
