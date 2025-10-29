
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.API.DTOs
{
    public class AlquilerDetalleDTO
    {
        public string NombreCliente { get; set; }
        public string ApellidoCliente { get; set; }
        public string direccionCliente { get; set; }
        public DateTime fechaAlquiler { get; set; }
        public decimal precioTotal { get; set; }
        public DateTime fechaInicioAlquiler { get; set; }
        public DateTime fechaFinAlquiler { get; set; }
        public IList<AlquilerItemDTO> items { get; set; }

        public AlquilerDetalleDTO(string nombreCliente, string apellidoCliente, string direccionCliente, DateTime fechaAlquiler, decimal precioTotal, DateTime fechaInicioALquiler, DateTime fechaFinAlquiler, IList<AlquilerItemDTO> items)
        {
            NombreCliente = nombreCliente;
            ApellidoCliente = apellidoCliente;
            this.direccionCliente = direccionCliente;
            this.fechaAlquiler = fechaAlquiler;
            this.precioTotal = precioTotal;
            this.fechaInicioAlquiler = fechaInicioALquiler;
            this.fechaFinAlquiler = fechaFinAlquiler;
            this.items = items;
        }

        public override bool Equals(object? obj)
        {
            return obj is AlquilerDetalleDTO dTO &&
                   NombreCliente == dTO.NombreCliente &&
                   ApellidoCliente == dTO.ApellidoCliente &&
                   direccionCliente == dTO.direccionCliente &&
                   fechaAlquiler == dTO.fechaAlquiler &&
                   precioTotal == dTO.precioTotal &&
                   fechaInicioAlquiler == dTO.fechaInicioAlquiler &&
                   fechaFinAlquiler == dTO.fechaFinAlquiler &&
                   EqualityComparer<IList<AlquilerItemDTO>>.Default.Equals(items, dTO.items);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(NombreCliente, ApellidoCliente, direccionCliente, fechaAlquiler, precioTotal, fechaInicioAlquiler, fechaFinAlquiler, items);
        }
    }
}
