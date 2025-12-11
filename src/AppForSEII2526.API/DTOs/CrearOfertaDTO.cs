namespace AppForSEII2526.API.DTOs
{
    public class CrearOfertaDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaOferta { get; set; }
        public List<OfertaItemDTO> Items { get; set; }
        public int MetodoPago { get; set; }
        public int? ParaSocio { get; set; }
        public string NombreUsuario { get; set; }

        public CrearOfertaDTO()
        {
            Items = new List<OfertaItemDTO>();
        }

        public CrearOfertaDTO(DateTime fechaInicio, DateTime fechaFin, DateTime fechaOferta, List<OfertaItemDTO> items, int metodoPago, int paraSocio, string nombreUsuario)
        {
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            FechaOferta = fechaOferta;
            Items = items;
            MetodoPago = metodoPago;
            ParaSocio = paraSocio;
            NombreUsuario = nombreUsuario;
        }

        public override bool Equals(object? obj)
        {
            return obj is CrearOfertaDTO dTO &&
                   FechaInicio == dTO.FechaInicio &&
                   FechaFin == dTO.FechaFin &&
                   FechaOferta == dTO.FechaOferta &&
                   Items.SequenceEqual(dTO.Items) &&
                   MetodoPago == dTO.MetodoPago &&
                   ParaSocio == dTO.ParaSocio &&
                   NombreUsuario == dTO.NombreUsuario;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FechaInicio, FechaFin, FechaOferta, Items, MetodoPago, ParaSocio, NombreUsuario);
        }
    }
}
