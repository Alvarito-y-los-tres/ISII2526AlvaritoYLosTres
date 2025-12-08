using AppForSEII2526.Web.API;
using System.Collections.Frozen;

namespace AppForSEII2526.Web
{
	public class CompraStateContainer
	{

		public CrearCompraDTO Compra { get; private set; } = new CrearCompraDTO()
		{
			CompraItems = new List<CompraItemDTO>()
		};

	

		

		public decimal PrecioTotal
		{
			get
			{
				return Compra.CompraItems.Sum(item =>
					(decimal)item.PrecioHerramienta * item.Cantidad
				);
			}
		}

		public event Action? OnChange;
		private void NotifyStateChanged() => OnChange?.Invoke();
		public void AddHerramientaToCompra(HerramientasParaComprarDTO herramienta)
		{
			if (!Compra.CompraItems.Any(ci => ci.NombreHerramienta == herramienta.Nombre))


				// Crear un nuevo item de compra
				Compra.CompraItems.Add(new CompraItemDTO
				{

					NombreHerramienta = herramienta.Nombre,
					MaterialHerrramienta = herramienta.Material,
					PrecioHerramienta = herramienta.Precio,
					Cantidad = 1
				});
		}



		public void RemoveHerramientaFromCompra(CompraItemDTO item)
		{
			Compra.CompraItems.Remove(item);
			
		}

		public void ClearCompra()
		{
			Compra.CompraItems.Clear();
			
		}

		public void CompraProcessed()
		{
			Compra = new CrearCompraDTO()
			{
				CompraItems = new List<CompraItemDTO>()
			
			};
			
		}
	}

	

		
}
