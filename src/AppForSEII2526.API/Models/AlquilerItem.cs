namespace AppForSEII2526.API.Models
{
    [PrimaryKey("AlquilerId", "HerramientaId")]
    public class AlquilerItem
    {
        

        
        public int HerramientaId { get; set; }
        public Herramienta Herramienta { get; set; }

       
        public int AlquilerId{ get; set; }
       

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
