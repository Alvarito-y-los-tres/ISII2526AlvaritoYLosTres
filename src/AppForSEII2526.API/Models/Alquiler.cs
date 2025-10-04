namespace AppForSEII2526.API.Models
{
    public class Alquiler
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string ApellidoCliente { get; set; }

        [EmailAddress]
        public string CorreoElectronico { get; set; }

        [Required]
        public string DireccionEnvio { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Fecha Alquiler")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaAlquiler { get; set; }

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string NombreCliente { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Fecha de Inicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Fecha Fin")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFin { get; set; }

        [Required, StringLength(9, ErrorMessage = "El teléfono no puede tener más de 9 caracteres")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "El teléfono debe contener 9 dígitos")]
        public string NumeroTelefono { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El periodo debe ser mayor a 0")]
        public int Periodo { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El precio total no puede ser negativo")]
        public int PrecioTotal { get; set; }

        public TiposMetodosPago MetodoPago { get; set; }

        public List<AlquilerItem> AlquilerItem { get; set; }
    }
}
