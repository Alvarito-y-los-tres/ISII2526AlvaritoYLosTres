

namespace AppForSEII2526.API.DTOs
{
    public class OfertaDetalleDTO
    {
        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaOferta { get; set; }
        public TiposDirigidaOferta? ParaSocios { get; set; }
        public TiposMetodosPago MetodoPago { get; set; }
        public IList<OfertaItemDTO> Items { get; set; }

        public OfertaDetalleDTO(int id, DateTime fechaInicio, DateTime fechaFin, DateTime fechaOferta, TiposDirigidaOferta? paraSocios, TiposMetodosPago metodoPago, IList<OfertaItemDTO> items)
        {
            Id = id;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            FechaOferta = fechaOferta;
            ParaSocios = paraSocios;
            MetodoPago = metodoPago;
            Items = items;
        }

        public override bool Equals(object? obj)
        {
            return obj is OfertaDetalleDTO dTO &&
                   FechaInicio == dTO.FechaInicio &&
                   FechaFin == dTO.FechaFin &&
                   FechaOferta == dTO.FechaOferta &&
                   ParaSocios == dTO.ParaSocios &&
                   MetodoPago == dTO.MetodoPago &&
                   Items.SequenceEqual(dTO.Items);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FechaInicio, FechaFin, FechaOferta, ParaSocios, MetodoPago, Items);
        }
    }
}
