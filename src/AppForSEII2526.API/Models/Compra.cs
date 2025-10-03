namespace AppForSEII2526.API.Models
{
    public class Compra
    {

        [Key]
        public int Id { get; set; }

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string ApellidoCLiente { get; set; }

        [EmailAddress]
        public string CorreoElectronico { get; set; }

        [Required]
        public string DireccionEnvio { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Release Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCompra { get; set; }

       

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string NombreCliente { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Display(Name = "Price For Renting")]
        [Precision(5, 2)]
        public decimal PrecioTotal { get; set; }

        [Phone]
        public int Telefono { get; set; }

        public List<CompraItem> CompraItems { get; set; }
    }
}
