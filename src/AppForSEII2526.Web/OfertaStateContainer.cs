using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class OfertaStateContainer
    {
        public CrearOfertaDTO Oferta { get; private set; } = new CrearOfertaDTO()
        {
            Items = new List<OfertaItemDTO>()
        };

        public decimal PrecioFinal 
            {
            get
            {
                return (decimal)Oferta.Items.Sum(i => i.Precio);
            }
        }

        public event Action? OnChange;

        public void NotifyStateChanged() => OnChange?.Invoke();

        public void AñadirHerramientaToOferta(HerramientasParaOfertarDTO herramienta)
        {
            if (!Oferta.Items.Any(i => i.NombreHerramienta == herramienta.Nombre))
            {
                Oferta.Items.Add(new OfertaItemDTO()
                {
                    NombreHerramienta = herramienta.Nombre,
                    MaterialHerramienta = herramienta.Material,
                    FabricanteHerramienta = herramienta.Fabricante,
                    Precio = herramienta.Precio
                });
                NotifyStateChanged();
            }
        }

        public void EliminarHerramientaDeOferta(OfertaItemDTO item)
        {
            Oferta.Items.Remove(item);
        }

        public void LimpiarCarritoOferta()
        {
            Oferta.Items.Clear();
        }

        public void OfertaPreparada()
        {
            Oferta = new CrearOfertaDTO()
            {
                Items = new List<OfertaItemDTO>()
            };
        }
    }
}
