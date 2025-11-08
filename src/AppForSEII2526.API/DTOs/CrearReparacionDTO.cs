using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs
{
	public class CrearReparacionDTO
	{
		public string NombreCliente { get; set; }
		public string ApellidoCliente { get; set; }
		public DateTime FechaEntrega { get; set; }
		public int MetodoPago { get; set; } // "TarjetaCredito", "PayPal" o "Metalico"
		public string? Telefono { get; set; }
		public IList<ReparacionItemDTO> Items { get; set; }
	
		public CrearReparacionDTO()
		{
			Items = new List<ReparacionItemDTO>();
		}	
	}
	
}