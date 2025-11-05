namespace AppForSEII2526.API.DTOs
{
    public class CrearAlquilerDTO
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DireccionEnvio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int MetodoPago { get; set; }
        public List<AlquilerItemDTO> AlquilerItems { get; set; }
        public CrearAlquilerDTO()
        {
            AlquilerItems = new List<AlquilerItemDTO>();
        }
    }
}
