using System.Security.Policy;

namespace AppForSEII2526.API.Models
{
    public class CompraItem
    {
        

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser un valor positivo")]
        public int Cantidad { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres")]
        public string Descripcion { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Display(Name = "Price For Renting")]
        [Precision(5, 2)]
        public decimal Precio { get; set; }


        [Required]
        public int IdHerramienta { get; set; }
        public Herramienta Herramienta { get; set; } 


        [Key]
        public int IdCompra { get; set; }
        public Compra Compra { get; set; }
    }
}
