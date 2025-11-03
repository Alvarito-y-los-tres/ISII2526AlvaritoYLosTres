namespace AppForSEII2526.API.DTOs
{
    public class CrearCompraDTO
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DireccionEnvio { get; set; }
        public int MetodoPagoId { get; set; }
        public string? numTelefono { get; set; }
        public string? correoElectronico { get; set; }
        public List<CompraItemDTO> CompraItems { get; set; }


    }
}
