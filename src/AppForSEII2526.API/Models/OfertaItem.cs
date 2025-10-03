namespace AppForSEII2526.API.Models
{
    public class OfertaItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdHerramienta { get; set; }
        public Herramienta Herramienta { get; set; }

        [Required]
        public int IdOferta { get; set; }
        public Oferta Oferta { get; set; }

        [Required]
        public float Porcentaje { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Display(Name = "Precio Final")]
        [Precision(5, 2)]
        public decimal PrecioFinal { get; set; }

        
        
       

    }
}
