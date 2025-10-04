namespace AppForSEII2526.API.Models
{
    public class Herramienta
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string Nombre { get; set; }

        [Required, StringLength(30, ErrorMessage = "El material no puede tener más de 30 caracteres")]
        public string Material { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Display(Name = "Price For Renting")]
        [Precision(18, 2)]
        public decimal Precio { get; set; }

        public float TiempoReparacion { get; set; }

        public Fabricante Fabricante { get; set; }


        public List<CompraItem> CompraItems { get; set; }

        public List<OfertaItem> OfertaItems { get; set; }
        
        public List<ReparacionItem> ReparacionItems { get; set; }

        public List<AlquilerItem> AlquilerItems { get; set; }
    }
}
