using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs
{
	public class CrearReparacionDTO
	{
		[Required(ErrorMessage="el nombre es obligatorio")]
		public string NombreCliente { get; set; }
        [Required(ErrorMessage = "el apellido es obligatorio")]
        public string ApellidoCliente { get; set; }
        [Required(ErrorMessage = "La fecha de entrega es obligatoria")]
        public DateTime FechaEntrega { get; set; }
        [Range(0, 2, ErrorMessage = "El método de pago es obligatorio")]
        public int MetodoPago { get; set; } // "TarjetaCredito", "PayPal" o "Metalico"
		public string? Telefono { get; set; }
		public IList<ReparacionItemDTO> Items { get; set; }
	
		public CrearReparacionDTO()
		{
			Items = new List<ReparacionItemDTO>();
			MetodoPago = -1;
		}	
	}
   
}