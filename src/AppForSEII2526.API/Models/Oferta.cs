namespace AppForSEII2526.API.Models
{
    public class Oferta
    {
        public Oferta()
        {
            
        }
        [Key]
        public int Id { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Fecha Inicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Fecha Final")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFin { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Fecha Oferta")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaOferta { get; set; }

        public List<OfertaItem> OfertaItems { get; set; }

        [Required]
        public TiposMetodosPago MetodoPago { get; set; }

        public TiposDirigidaOferta? ParaSocio { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public Oferta(DateTime fechaInicio, DateTime fechaFin, DateTime fechaOferta, List<OfertaItem> ofertaItems, TiposMetodosPago metodoPago, TiposDirigidaOferta paraSocio, ApplicationUser applicationUser)
        {
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            FechaOferta = fechaOferta;
            OfertaItems = ofertaItems;
            MetodoPago = metodoPago;
            ParaSocio = paraSocio;
            ApplicationUser = applicationUser;
        }
    }
}
