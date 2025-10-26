


namespace AppForSEII2526.API.DTOs
{
    public class OfertaDetalleDTO
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime FechaOferta { get; set; }
        public TiposDirigidaOferta ParaSocios { get; set; }
        public TiposMetodosPago MetodoPago { get; set; }
        public IList<OfertaItemDTO> OfertaItems { get; set; }

        public OfertaDetalleDTO(DateTime fechaInicio, DateTime fechaFin, DateTime fechaOferta, TiposDirigidaOferta paraSocios, TiposMetodosPago metodoPago, IList<OfertaItemDTO> ofertaItems)
        {
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            FechaOferta = fechaOferta;
            ParaSocios = paraSocios;
            MetodoPago = metodoPago;
            OfertaItems = ofertaItems;
        }

        public override bool Equals(object? obj)
        {
            return obj is OfertaDetalleDTO dTO &&
                   FechaInicio == dTO.FechaInicio &&
                   FechaFin == dTO.FechaFin &&
                   FechaOferta == dTO.FechaOferta &&
                   ParaSocios == dTO.ParaSocios &&
                   MetodoPago == dTO.MetodoPago &&
                   EqualityComparer<IList<OfertaItemDTO>>.Default.Equals(OfertaItems, dTO.OfertaItems);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FechaInicio, FechaFin, FechaOferta, ParaSocios, MetodoPago, OfertaItems);
        }
    }
}
