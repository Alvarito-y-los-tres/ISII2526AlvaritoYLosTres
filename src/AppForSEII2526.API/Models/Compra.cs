namespace AppForSEII2526.API.Models
{
    public class Compra
    {
        public Compra()
        {
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string DireccionEnvio { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Date), Display(Name = "Fecha Compra")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCompra { get; set; }


        [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Display(Name = "Price For Renting")]
        [Precision(5, 2)]
        public decimal PrecioTotal { get; set; }


        [Required]
        public TiposMetodosPago MetodoPago { get; set; }

        public List<CompraItem> CompraItems { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        public Compra(string direccionEnvio, DateTime fechaCompra, decimal precioTotal, TiposMetodosPago metodoPago, List<CompraItem> compraItems, ApplicationUser applicationUser)
        {
            
            DireccionEnvio = direccionEnvio;
            FechaCompra = fechaCompra;
            PrecioTotal = precioTotal;
            MetodoPago = metodoPago;
            CompraItems = compraItems;
            ApplicationUser = applicationUser;
        }
    }
}
