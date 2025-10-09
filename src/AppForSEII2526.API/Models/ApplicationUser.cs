using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{

    [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
    [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres")]
    [RegularExpression(@"^[A-Z]+[a-zA-Z''-\s]*$", ErrorMessage = "El nombre debe empezar por mayúscula")]
    public string NombreCliente { get; set; }

    [Required(ErrorMessage = "El apellido del cliente es obligatorio")]
    [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres")]
    [RegularExpression(@"^[A-Z]+[a-zA-Z''-\s]*$", ErrorMessage = "El apellido debe empezar por mayúscula")]
    public string ApellidoCliente { get; set; }

    [Phone(ErrorMessage = "Número de teléfono no válido")]
    public string? NumTelefono { get; set; }


    [EmailAddress]
    public string CorreoElectronico { get; set; }

    public List<Reparacion> Reparaciones { get; set; }

    public List<Oferta> Ofertas { get; set; }

    public List<Compra> Compras { get; set; }
}