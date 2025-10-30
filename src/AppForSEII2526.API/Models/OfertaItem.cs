namespace AppForSEII2526.API.Models
{

    [PrimaryKey("HerramientaId", "OfertaId")]

    public class OfertaItem
    {
        public OfertaItem()
        {
        }
        public int HerramientaId { get; set; }
        public Herramienta Herramienta { get; set; }

        public int OfertaId { get; set; }
        public Oferta Oferta { get; set; }

        [Required]
        public float Porcentaje { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Display(Name = "Precio Final")]
        [Precision(5, 2)]
        public decimal PrecioFinal { get; set; }

        public OfertaItem(float porcentaje, decimal precioFinal)
        {
            Porcentaje = porcentaje;
            PrecioFinal = precioFinal;
        }
    }
}
