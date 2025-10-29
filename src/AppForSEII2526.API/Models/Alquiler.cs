namespace AppForSEII2526.API.Models
{
    public class Alquiler
    {
        [Key]
        public int Id { get; set; }

        

        

        [Required]
        public string DireccionEnvio { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Fecha Alquiler")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaAlquiler { get; set; }

        

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Fecha de Inicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Fecha Fin")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFin { get; set; }

        

        [Range(1, int.MaxValue, ErrorMessage = "El periodo debe ser mayor a 0")]
        public int Periodo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El precio total no puede ser negativo")]
        public decimal PrecioTotal { get; set; }

        public TiposMetodosPago MetodoPago { get; set; }

        public List<AlquilerItem> AlquilerItems { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
