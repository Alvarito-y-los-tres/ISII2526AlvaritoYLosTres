namespace AppForSEII2526.API.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ReparacionItem
    {
        public int Id { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres")]
        public string? Descripcion { get; set; }

        [Precision(8, 2)]
        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }

        // Relaciones
        public int IdHerramienta { get; set; }
        public Herramienta Herramienta { get; set; }

        public int IdReparacion { get; set; }
        public Reparacion Reparacion { get; set; }
    }
    

}    