
using AppForSEII2526.Web.API;


namespace AppForSEII2526.Web
{
    public class AlquilerStateContainer
    {
        public CrearAlquilerDTO Alquiler { get; private set; } = new CrearAlquilerDTO()
        {
            AlquilerItems = new List<AlquilerItemDTO>()
        };

        public decimal PrecioFinal
        {
            get
            {
                return (decimal)Alquiler.AlquilerItems.Sum(i => i.PrecioHerramienta);
            }
        }

        public event Action? OnChange;

        public void NotifyStateChanged() => OnChange?.Invoke();

        public void AñadirHerramientaToAlquiler(HerramientaAlquilarDTO herramienta)
        {
            if (!Alquiler.AlquilerItems.Any(i => i.NombreHerramienta == herramienta.Nombre))
            {
                Alquiler.AlquilerItems.Add(new AlquilerItemDTO()
                {
                    NombreHerramienta = herramienta.Nombre,
                    MaterialHerrramienta = herramienta.Material,
                    PrecioHerramienta = herramienta.Precio
                });
                NotifyStateChanged();
            }
        }

        public void EliminarHerramientaDeAlquiler(AlquilerItemDTO item)
        {
            Alquiler.AlquilerItems.Remove(item);
        }

        public void LimpiarCarritoAlquiler()
        {
            Alquiler.AlquilerItems.Clear();
        }

        public void AlquilerPreparado()
        {
            Alquiler = new API.CrearAlquilerDTO()
            {
                AlquilerItems = new List<AlquilerItemDTO>()
            };
        }
    }
}