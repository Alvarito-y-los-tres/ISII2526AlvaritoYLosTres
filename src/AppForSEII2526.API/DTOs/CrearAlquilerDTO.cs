using System;
using System.Collections.Generic;
// ES IMPRESCINDIBLE ESTA LÍNEA PARA QUE FUNCIONEN LOS [Required]
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs
{
    public class CrearAlquilerDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.\n")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.\n")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "La dirección de envío es obligatoria.\n")]
        public string DireccionEnvio { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        
        [Required(ErrorMessage = "Debes seleccionar un método de pago.\n")]
        public int MetodoPago { get; set; }
        public string? numTelefono { get; set; }
        public string? CorreoElectronico { get; set; }


        [Required]
        [MinLength(1, ErrorMessage = "Debes seleccionar al menos una herramienta para alquilar.")]
        public List<AlquilerItemDTO> AlquilerItems { get; set; }

        public CrearAlquilerDTO()
        {
            AlquilerItems = new List<AlquilerItemDTO>();
            
            FechaInicio = DateTime.Now;
            FechaFin = DateTime.Now.AddDays(1);
        }

        public CrearAlquilerDTO(string nombre, string apellido, string direccionEnvio, DateTime fechaInicio, DateTime fechaFin, int metodoPago, List<AlquilerItemDTO> alquilerItems)
        {
            Nombre = nombre;
            Apellido = apellido;
            DireccionEnvio = direccionEnvio;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            MetodoPago = metodoPago;
            AlquilerItems = alquilerItems;
        }
    }
}