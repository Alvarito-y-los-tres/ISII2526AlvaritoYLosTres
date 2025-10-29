namespace AppForSEII2526.API.Models
{
    public class Fabricante
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string Nombre { get; set; }

        public List<Herramienta> Herramientas { get; set; } 

        public Fabricante(string nombre)
        {
            Nombre = nombre;
        }
    }
}
