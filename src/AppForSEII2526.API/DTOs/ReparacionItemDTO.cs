
using System.ComponentModel;

namespace AppForSEII2526.API.DTOs
{
    
    public class ReparacionItemDTO
    {
        // nombre precio descripcionProblema cantidad
        public string NombreHerramienta { get; set; }
        public string DescripcionProblema { get; set; }
        public int Cantidad { get; set; }
        
        public decimal Precio { get; set; }
       
        public int ReparacionId { get; set; }

        public ReparacionItemDTO(string nombreHerramienta, string descripcionProblema, int cantidad, decimal precio, int reparacionId)
        {
            NombreHerramienta = nombreHerramienta;
            DescripcionProblema = descripcionProblema;
            Cantidad = cantidad;
            Precio = precio;
            ReparacionId = reparacionId;
        }

        public override bool Equals(object? obj)
        {
            return obj is ReparacionItemDTO dTO &&
                   NombreHerramienta == dTO.NombreHerramienta &&
                   DescripcionProblema == dTO.DescripcionProblema &&
                   Cantidad == dTO.Cantidad &&
                   Precio == dTO.Precio &&
                   ReparacionId == dTO.ReparacionId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreHerramienta, DescripcionProblema, Cantidad, Precio,ReparacionId);
        }

    }
}    