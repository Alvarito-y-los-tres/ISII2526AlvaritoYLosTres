using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web {
    public class ReparacionStateContainer {

        //we create an instance of Rental when an instance of RentalStateContainer is created
        public CrearReparacionDTO Reparacion { get; private set; } = new CrearReparacionDTO() {
            Items = new List<ReparacionItemDTO>()
        };

        // aqui calcula el precio pero no lo voy a poner de momento pq no se para que es


        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();


        
        public void AddMItemReparacion(ReparacionItemDTO item) {
            //before adding a movie we checked whether it has been already added
            if (!Reparacion.Items.Any(ri => ri.NombreHerramienta == item.NombreHerramienta))
                //we add it if it is not in the list
                Reparacion.Items.Add(new ReparacionItemDTO() {
                    NombreHerramienta = item.NombreHerramienta,
					DescripcionProblema = item.DescripcionProblema,
					Cantidad = item.Cantidad
                }
            );

        }

        //to delete movies from the list of selected movies
        public void RemoveItemReparacion(ReparacionItemDTO item) {
            Reparacion.Items.Remove(item);

        }

        //we eliminate all the movies from the list
        public void ClearALL() {
            Reparacion.Items.Clear();

        }

        //we have already finished the process of renting, thus, we create a new Rental 
        public void RentalProcessed() {
            //we have finished the rental process so we create a new object without data
            Reparacion = new CrearReparacionDTO() {
                Items = new List<ReparacionItemDTO>()
            };
        }
    }
}