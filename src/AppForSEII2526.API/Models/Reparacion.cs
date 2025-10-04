namespace AppForSEII2526.API.Models
{ 
    using System.ComponentModel.DataAnnotations;
    public class Reparacion
    {
        
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-\s]*$", ErrorMessage = "El nombre debe empezar por mayúscula")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "El apellido del cliente es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-\s]*$", ErrorMessage = "El apellido debe empezar por mayúscula")]
        public string ApellidoCliente { get; set; }

        [Phone(ErrorMessage = "Número de teléfono no válido")]
        public string? NumTelefono { get; set; }

        [Required(ErrorMessage = "La fecha de entrega es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Entrega")]
        public DateTime FechaEntrega { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Recogida")]
        public DateTime FechaRecogida { get; set; }

        [Precision(8, 2)]
        [DataType(DataType.Currency)]
        public decimal PrecioTotal { get; set; }

        [Required]
        public TiposMetodosPago MetodoPago { get; set; }

        // Relación con los ítems de reparación
        public List<ReparacionItem> ItemsReparacion { get; set; }
    }
}