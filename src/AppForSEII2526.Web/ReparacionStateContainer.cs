using AppForSEII2526.Web.API;
using System.Collections.Generic;
using System;
using System.Linq;

namespace AppForSEII2526.Web 
{
    public class ReparacionStateContainer 
    {
        // Inicializamos la lista para evitar NullReferenceException
        public CrearReparacionDTO Reparacion { get; private set; } = new CrearReparacionDTO() {
            Items = new List<ReparacionItemDTO>()
        };

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddItemReparacion(HerramientaRepararDTO item) 
        {
            // Verificamos si ya existe para no duplicar
            if (!Reparacion.Items.Any(ri => ri.NombreHerramienta == item.Nombre))
            {
                Reparacion.Items.Add(new ReparacionItemDTO() {
                    NombreHerramienta = item.Nombre,
                    DescripcionProblema = "nosesabe",
                    Cantidad = 1
                });
                NotifyStateChanged();
            }
        }

        public void RemoveItemReparacion(ReparacionItemDTO item) 
        {
            Reparacion.Items.Remove(item);
            NotifyStateChanged();
        }

        public void ClearALL() 
        {
            Reparacion.Items.Clear();
            NotifyStateChanged();
        }

        // Reseteamos el estado después de procesar
        public void ReparacionProcessed() 
        {
            Reparacion = new CrearReparacionDTO() {
                Items = new List<ReparacionItemDTO>()
            };
            NotifyStateChanged();
        }
    }
}