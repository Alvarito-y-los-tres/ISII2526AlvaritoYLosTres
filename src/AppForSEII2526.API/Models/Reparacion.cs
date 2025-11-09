namespace AppForSEII2526.API.Models
{ 
    using System.ComponentModel.DataAnnotations;


    public class Reparacion
    {
        public Reparacion()
        {
            
        }
        public int Id { get; set; }

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
        
        public ApplicationUser ApplicationUser { get; set; }

        public Reparacion(ApplicationUser applicationUser, DateTime fechaEntrega, DateTime fechaRecogida)
        {
            ApplicationUser = applicationUser;
            FechaEntrega = fechaEntrega;
            FechaRecogida = fechaRecogida;
            ItemsReparacion = new List<ReparacionItem>();
        }
    }
}