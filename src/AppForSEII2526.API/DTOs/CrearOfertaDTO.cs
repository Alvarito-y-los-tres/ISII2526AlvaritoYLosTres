namespace AppForSEII2526.API.DTOs
{
    public class CrearOfertaDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaOferta { get; set; }
        public List<OfertaItemDTO> Items { get; set; }
        public int MetodoPago { get; set; }
        public int ParaSocio { get; set; }
        public string nombreUsuario { get; set; }

        public CrearOfertaDTO()
        {
            Items = new List<OfertaItemDTO>();
        }
    }
}
