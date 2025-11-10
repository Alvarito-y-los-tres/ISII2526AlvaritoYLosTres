using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;

namespace AppForSEII2526.UT.ReparacionesController_test
{
    public class PostReparacion_test : AppForSEII25264SqliteUT
    {
        public PostReparacion_test()
        {
            // --- Crear fabricantes ---
            var fabricantes = new List<Fabricante>()
            {
                new Fabricante("Ferretería López"),
                new Fabricante("Ferretería García"),
                new Fabricante("Ferretería Ruiz")
            };
            // --- Crear herramientas ---
            var herramientas = new List<Herramienta>()
            {
                new Herramienta("Martillo", "Madera", 5, 3, fabricantes[0]),
                new Herramienta("Taladro", "Acero", 30, 14, fabricantes[1]),
                new Herramienta("Alicates", "Hierro", 4, 5, fabricantes[2])
            };
            // --- Crear usuario ---
            var usuario = new ApplicationUser("Martín", "Álvarez", "660111222", "martin.alvarez@ejemplo.com");
            // --- Guardar en contexto ---
            _context.Fabricantes.AddRange(fabricantes);
            _context.Herramientas.AddRange(herramientas);
            _context.Users.Add(usuario);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit testing")]
        [Trait("Controller", "Reparaciones")]
        public async Task PostReparacion_Valida()
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            var controller = new ReparacionesController(_context, mock.Object);

            var nuevaReparacionDTO = new CrearReparacionDTO
            {
                NombreCliente = "Martín",
                ApellidoCliente = "Álvarez",
                FechaEntrega = DateTime.Today.AddDays(2),
                MetodoPago = 1,
                Items = new List<ReparacionItemDTO>
                {
                    new ReparacionItemDTO
                    (
                        "Martillo",
                        "Mango roto",
                        2
                    ),
                    new ReparacionItemDTO
                    (
                        _context.Herramientas.Skip(1).First().Nombre,
                        "Mango roto",
                        1
                    )
                }
            };
            var precioEsperado = 0;
            foreach (var item in nuevaReparacionDTO.Items)
            {
                var herramienta = _context.Herramientas.FirstOrDefault(h => h.Nombre == item.NombreHerramienta);
                if (herramienta != null)
                {
                    precioEsperado += (int) herramienta.Precio * item.Cantidad;
                }
            }


            var detalleEsperado= new ReparacionDetalleDTO
            (
                nuevaReparacionDTO.NombreCliente,
                nuevaReparacionDTO.ApellidoCliente,
                precioEsperado, // Precio total esperado (2*5 + 1*30)
                nuevaReparacionDTO.FechaEntrega,
                nuevaReparacionDTO.FechaEntrega.AddDays(nuevaReparacionDTO.Items.Count), // FechaRecogida por defecto
                nuevaReparacionDTO.Items
            );

            // Act
            var result = await controller.CrearReparacion(nuevaReparacionDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var reparacionCreada = Assert.IsType<ReparacionDetalleDTO>(okResult.Value);

            Assert.Equal(detalleEsperado, reparacionCreada);
        }

        //casos para el theory de fallos
        public static IEnumerable<object[]> CasosParaPostReparacion_Fallos()
        {
            var items = new List<ReparacionItemDTO>
            {
                new ReparacionItemDTO
                (
                    "Martillo",
                    "Mango roto",
                    2
                ),
                new ReparacionItemDTO
                (
                    "Taladro",
                    "Mango roto",
                    1
                )
            };

            // Caso 1: Cliente no existe
            var UsuNoExiste = new CrearReparacionDTO
            {
                NombreCliente = "NoExiste",
                ApellidoCliente = "Trujillo",
                FechaEntrega = DateTime.Today,
                MetodoPago = 1,
                Items = items
            };
            yield return new object[] { UsuNoExiste, $"El usuario '{UsuNoExiste.NombreCliente} {UsuNoExiste.ApellidoCliente}' no existe." };

            // Caso 2: dto de entrada es nulo
            yield return new object[] { null, "El objeto no puede ser nulo." };

            // Caso 3: NombreCliente es vacío
            var NombreVacio = new CrearReparacionDTO
            {
                NombreCliente = "",
                ApellidoCliente = "Álvarez",
                FechaEntrega = DateTime.Today,
                MetodoPago = 1,
                Items = items
            };
            yield return new object[] { NombreVacio, "El nombre del cliente es obligatorio." };

            // Caso 4: Ningun item en la reparacion
            var SinItems = new CrearReparacionDTO
            {
                NombreCliente = "Martín",
                ApellidoCliente = "Álvarez",
                FechaEntrega = DateTime.Today,
                MetodoPago = 1,
                Items = new List<ReparacionItemDTO>()
            };
            yield return new object[] { SinItems, "Debe incluir al menos una herramienta para reparar." };

            // Caso 5: Fecha de entrega en el pasado
            var FechaPasada = new CrearReparacionDTO
            {
                NombreCliente = "Martín",
                ApellidoCliente = "Álvarez",
                FechaEntrega = DateTime.Today.AddDays(-1),
                MetodoPago = 1,
                Items = items
            };
            yield return new object[] { FechaPasada, "La fecha de entrega no puede ser en el pasado." };

            // Caso 6: Metodo de pago inválido
            var MetodoPagoInvalido = new CrearReparacionDTO
            {
                NombreCliente = "Martín",
                ApellidoCliente = "Álvarez",
                FechaEntrega = DateTime.Today,
                MetodoPago = 99, // Valor inválido
                Items = items
            };
            yield return new object[] { MetodoPagoInvalido, "Método de pago inválido." };

            // Caso 7: Herramienta no existe
            var HerramientaNoExiste = new CrearReparacionDTO
            {
                NombreCliente = "Martín",
                ApellidoCliente = "Álvarez",
                FechaEntrega = DateTime.Today,
                MetodoPago = 1,
                Items = new List<ReparacionItemDTO>
                {
                    new ReparacionItemDTO
                    (
                        "HerramientaInexistente",
                        "Mango roto",
                        2
                    )
                }
            };
            yield return new object[] { HerramientaNoExiste, "La herramienta 'HerramientaInexistente' no existe." };



        }

        [Theory]
        [Trait("LevelTesting", "Unit testing")]
        [Trait("Controller", "Reparaciones")]
        [MemberData(nameof(CasosParaPostReparacion_Fallos))]
        public async Task PostReparacion_Fallos( CrearReparacionDTO? nuevaReparacionDTO, string mensajeEsperado)
        {
            // Arrange
            var mock = new Mock<ILogger<ReparacionesController>>();
            var controller = new ReparacionesController(_context, mock.Object);

            // Act
            var Result = await controller.CrearReparacion(nuevaReparacionDTO);

            // Assert
            if(nuevaReparacionDTO == null || Result is BadRequestObjectResult)
            {
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(Result);
                var mensaje = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
                Assert.Contains(mensaje.Errors.Values, lista => lista.Any(m => m.StartsWith(mensajeEsperado)));
            }
            else
            {
                // caso usuario no encontrado retorna not found 
                var notFoundResult = Assert.IsType<NotFoundObjectResult>(Result);
                var mensaje = Assert.IsType<string>(notFoundResult.Value);
                Assert.Equal(mensajeEsperado, mensaje);
            }








        }
    }
}
