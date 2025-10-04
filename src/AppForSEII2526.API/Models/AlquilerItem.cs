namespace AppForSEII2526.API.Models
{
    public class AlquilerItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdHerramienta { get; set; }
        public Herramienta Herramienta { get; set; }

        [Required]
        public int IdAlquiler{ get; set; }
       

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Display(Name = "Precio de alquiler")]
        [Precision(5, 2)]
        public decimal Precio { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un valor positivo")]
        public int Cantidad { get; set; }

        public Alquiler Alquiler { get; set; }
    }
}
