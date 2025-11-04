namespace AppForSEII2526.API.DTOs
{
    public class CrearCompraDTO
    {
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
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
    }
}
