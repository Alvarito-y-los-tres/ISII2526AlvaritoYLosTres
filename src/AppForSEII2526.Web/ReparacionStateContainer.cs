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
            Items = new List<ReparacionItemDTO>(),
            MetodoPago=-1
        };

        public event Action? OnChange;
        public Dictionary<string, float> TiemposReparacionAux { get; private set; } = new Dictionary<string, float>();

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void AddItemReparacion(HerramientaRepararDTO item) 
        {
            // Verificamos si ya existe para no duplicar
            if (!Reparacion.Items.Any(ri => ri.NombreHerramienta == item.Nombre))
            {
                Reparacion.Items.Add(new ReparacionItemDTO() {
                    NombreHerramienta = item.Nombre,
                    DescripcionProblema = null,
                    Cantidad = 0,
                    Precio = item.Precio
                });

                //Guardo el tiempo usando el nombre como clave porque mi dto de item no tiene tiempo porqu eno lo pide y el de herramientareparar si 
                if(!TiemposReparacionAux.ContainsKey(item.Nombre))
                { 
                    TiemposReparacionAux.Add(item.Nombre, item.TiempoReparacion); 
                }
                NotifyStateChanged();
            }
        }

        public void RemoveItemReparacion(ReparacionItemDTO item) 
        {
            //limpio el diccionario para no acumular "basura"
            if (TiemposReparacionAux.ContainsKey(item.NombreHerramienta))
            {
                TiemposReparacionAux.Remove(item.NombreHerramienta);
            }
            Reparacion.Items.Remove(item);
            NotifyStateChanged();
        }

        public void ClearALL() 
        {
            Reparacion.Items.Clear();
            TiemposReparacionAux.Clear();
            NotifyStateChanged();

        }

        // Reseteamos el estado después de procesar
        public void ReparacionProcessed() 
        {
            Reparacion = new CrearReparacionDTO() {
                Items = new List<ReparacionItemDTO>()
            };
            TiemposReparacionAux.Clear();
            NotifyStateChanged();
        }
    }
}