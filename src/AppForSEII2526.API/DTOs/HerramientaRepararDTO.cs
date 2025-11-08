namespace AppForSEII2526.API.DTOs
{
    public class HerramientaRepararDTO
    {
        public string Nombre { get; set; }
        public string Material { get; set; }
        public string NombreFabricante { get; set; }
        public decimal Precio { get; set; }

        public float TiempoReparacion { get; set; }


        public HerramientaRepararDTO(string nombre, string material, string fabricante, decimal precio, float diasHabilesReparacion)
        {
            Nombre = nombre;
            Material = material;
            NombreFabricante = fabricante;
            Precio = precio;
            TiempoReparacion = diasHabilesReparacion;
        }

        public override bool Equals(object? obj)
        {
            return obj is HerramientaRepararDTO dTO &&
                   Nombre == dTO.Nombre &&
                   Material == dTO.Material &&
                   NombreFabricante == dTO.NombreFabricante &&
                   Precio == dTO.Precio &&
                   TiempoReparacion == dTO.TiempoReparacion;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Nombre, Material, NombreFabricante, Precio, TiempoReparacion);
        }
    }
}