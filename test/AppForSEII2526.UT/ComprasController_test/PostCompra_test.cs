using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.ComprasController_test
{
    public class PostCompra_test : AppForSEII25264SqliteUT
    {
        public PostCompra_test()
        {
            var fabricante = new List<Fabricante>()
            {
                new Fabricante("Herramientas Trujillo"),
                new Fabricante("Herramientas López"),
            };

            var herramientas = new List<Herramienta>()
            {
                new Herramienta("Martillo","Madera",35,3, fabricante[0]),
                new Herramienta("Alicates", "Acero",12,1, fabricante[1]),
            };

            var usuario = new ApplicationUser("Ana", "López", "671222333", "ana.lopez@web.es");

            var compra = new Compra("Calle Tejares", DateTime.Today, 35, TiposMetodosPago.Paypal, new List<CompraItem>(), usuario);

            

           compra.CompraItems.Add(new CompraItem(2, "antiguo", 35, herramientas[0], compra));

            _context.AddRange(fabricante);
            _context.AddRange(herramientas);
            _context.Add(usuario);
            _context.Add(compra);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CrearCompra()
        {
            var compraNoItem = new CrearCompraDTO("Ana", "López", "Calle Tejares", 0, "671222333", "ana.lopez@web.es", 70, new List<CompraItemDTO>());

            var compraItems = new List<CompraItemDTO>() { new CompraItemDTO(1, "Martillo", "Madera", 35, "antigo", 2,1) };

            var compraNoNombre = new CrearCompraDTO(null, "López", "Calle Tejares", (int)TiposMetodosPago.Paypal, "671222333", "ana.lopez@web.es", 70, compraItems);

			var compraNoApellido = new CrearCompraDTO("Ana", null, "Calle Tejares", 0, "671222333", "ana.lopez@web.es", 70, compraItems);

            var compraNoDireccion = new CrearCompraDTO("Ana", "Lopez", null, 0, "671222333", "ana.lopez@web.es", 70, compraItems);

            var compraCantidadCero = new CrearCompraDTO("Ana", "López", "Calle Tejares", (int)TiposMetodosPago.Paypal, "671222333", "ana.lopez@web.es",70,  new List<CompraItemDTO>() { new CompraItemDTO(1, "Martillo", "Madera", 35, "antigo", 0,1) });
       
            var compraHerramienteInexistente = new CrearCompraDTO("Ana", "López", "Calle Tejares", (int)TiposMetodosPago.Paypal, "671222333", "ana.lopez@web.es",70, new List<CompraItemDTO>() { new CompraItemDTO(3, "LLave inglesa", "Hierro", 25, "nuevo", 4,1) });

            var compraNoDescripcionCantidad3 = new CrearCompraDTO("Ana", "López", "Calle Tejares", (int)TiposMetodosPago.Paypal, "671222333", "ana.lopez@web.es", 70, new List<CompraItemDTO>() { new CompraItemDTO(1, "Martillo", "Madera", 35, null,3,1) });

            var compraNoDescripcion = new CrearCompraDTO("Ana", "López", "Calle Tejares", (int)TiposMetodosPago.Paypal, "671222333", "ana.lopez@web.es",70, new List<CompraItemDTO>() { new CompraItemDTO(1, "Martillo", "Madera", 35, null, 2,1) });

            

            var allTest = new List<object[]>
            {
                new object[] { compraNoItem, "Error: La compra debe contener al menos un ítem." },
                new object[] { compraNoNombre, "Error: El nombre del cliente no puede ser nulo, es obligatorio." },
				new object[] { compraNoApellido, "Error: El apellido del cliente no puede ser nulo, es obligatorio." },
                new object[] { compraNoDireccion, "Error: La dirección de envío no puede ser nula, es obligatoria." },
                new object[] { compraCantidadCero, "La cantidad de cada item debe ser mayor que cero." },
                new object[] { compraHerramienteInexistente, "La cantidad de cada item debe ser mayor que cero." },
                new object[] { compraNoDescripcionCantidad3, "¡Error!.Estás comprando demasiadas herramientas sin descripción." },
                new object[] { compraNoDescripcion, "La descripción de cada item es obligatoria." }
               
            };

            return allTest;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CrearCompra))]
        public async Task CrearCompra_Error_test(CrearCompraDTO crearCompraDTO, string expectedMessage)
        {
            // Arrange
            var morck = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = morck.Object;

            
            var controller = new ComprasController(_context, logger);

            // Act
            var result = await controller.CrearCompra(crearCompraDTO);


            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(expectedMessage, errorActual); 
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CrearCompra_Success_test()
        {
            //Arrange
            var morck = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = morck.Object;

            var controller = new ComprasController(_context, logger);

            var compraItems = new List<CompraItemDTO>() { new CompraItemDTO(1, "Martillo", "Madera", 35, "antigo", 2,1) };

            var compraDTO = new CrearCompraDTO("Ana", "López", "Calle Tejares", 0, "671222333", "ana.lopez@web.es", 70, new List<CompraItemDTO>() { new CompraItemDTO(1, "Martillo", "Madera", 35, "antigo", 2,1) });

            var expectedCompraDetailDTO = new CompraDetalleDTO(1,"Ana", "López", "Calle Tejares", 70, DateTime.Today, new List<CompraItemDTO>() { new CompraItemDTO(1, "Martillo", "Madera", 35, "antigo", 2,1) });

            //Act
            var result = await controller.CrearCompra(compraDTO);

            //Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualCompraDetailDTO = Assert.IsType<CompraDetalleDTO>(createdResult.Value);

            Assert.Equal(expectedCompraDetailDTO, actualCompraDetailDTO);
        }





    }
    }

