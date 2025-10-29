namespace AppForSEII2526.API.Models
{
    using System.ComponentModel.DataAnnotations;

    [PrimaryKey("ReparacionId", "HerramientaId")]
    public class ReparacionItem
    {

        public ReparacionItem()
        {
        }
        [Required]
        public int Cantidad { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres")]
        public string? Descripcion { get; set; }

        [Precision(8, 2)]
        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }

        // Relaciones
        public int HerramientaId { get; set; }
        public Herramienta Herramienta { get; set; }

        public int ReparacionId { get; set; }
        public Reparacion Reparacion { get; set; }
    }
    

}    