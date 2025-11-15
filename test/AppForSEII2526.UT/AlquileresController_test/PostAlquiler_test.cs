using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit; // Asegúrate de tener este using para los atributos de xUnit
using Microsoft.AspNetCore.Mvc; // <-- CAMBIO: Necesitarás este using para ValidationProblemDetails

namespace AppForSEII2526.UT.AlquileresController_test
{
    public class PostAlquiler_test : AppForSEII25264SqliteUT
    {
        public PostAlquiler_test()
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
            var usuario = new ApplicationUser("Daniel", "de la Cruz", "671222333", "daniel.cruz3@alu.uclm.es");
            var alquiler = new Alquiler("Calle Tejares", DateTime.Today, DateTime.Today.AddDays(5), 20, TiposMetodosPago.TarjetaCredito, new List<AlquilerItem>(), usuario);
            var alquilerItems = new List<AlquilerItem>() { new AlquilerItem(1, "antiguo", 3, herramientas[0], alquiler) };
            _context.AddRange(fabricante);
            _context.AddRange(herramientas);
            _context.Add(usuario);
            _context.Add(alquiler);
            _context.AddRange(alquilerItems);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CrearAlquiler()
        {
            // --- CAMBIO: Usamos fechas futuras para que pasen la validación de fecha ---
            var fechaInicioValida = DateTime.Today.AddDays(1);
            var fechaFinValida = DateTime.Today.AddDays(5);
            // --------------------------------------------------------------------------

            var alquilerItems = new List<AlquilerItemDTO>() { new AlquilerItemDTO("Martillo", "Madera", 35, 3) };

            // --- CAMBIO: Se usan las fechas válidas en todos los DTOs de prueba ---
            var alquilerNoItem = new CrearAlquilerDTO("Daniel", "de la Cruz", "Calle Tejares", fechaInicioValida, fechaFinValida, 0, new List<AlquilerItemDTO>());
            var alquilerNoNombre = new CrearAlquilerDTO(null, "de la Cruz", "Calle Tejares", fechaInicioValida, fechaFinValida, (int)TiposMetodosPago.TarjetaCredito, alquilerItems);
            var alquilerNoApellido = new CrearAlquilerDTO("Daniel", null, "Calle Tejares", fechaInicioValida, fechaFinValida, (int)TiposMetodosPago.TarjetaCredito, alquilerItems);
            var alquilerNoDireccion = new CrearAlquilerDTO("Daniel", "de la Cruz", null, fechaInicioValida, fechaFinValida, (int)TiposMetodosPago.TarjetaCredito, alquilerItems);
            var alquilerCantidadCero = new CrearAlquilerDTO("Daniel", "de la Cruz", "Calle Tejares", fechaInicioValida, fechaFinValida, (int)TiposMetodosPago.TarjetaCredito, new List<AlquilerItemDTO>() { new AlquilerItemDTO("Martillo", "Madera", 35, 0) });
            var alquilerHerramienteInexistente = new CrearAlquilerDTO("Daniel", "de la Cruz", "Calle Tejares", fechaInicioValida, fechaFinValida, (int)TiposMetodosPago.TarjetaCredito, new List<AlquilerItemDTO>() { new AlquilerItemDTO("LLave inglesa", "Hierro", 25, 4) });

            // Este se deja con las fechas mal a propósito para probar esa regla
            var alquilerFechaInvalida = new CrearAlquilerDTO("Daniel", "de la Cruz", "Calle Tejares", DateTime.Today.AddDays(5), DateTime.Today.AddDays(1), (int)TiposMetodosPago.TarjetaCredito, alquilerItems);

            // --- CAMBIO: Mensajes de error ajustados a los mensajes REALES del validador ---
            var allTest = new List<object[]>
            {
                // El error real es "El alquiler debe contener al menos una herramienta"
                new object[] { alquilerNoItem, "El alquiler debe contener al menos una herramienta." }, 
                
                new object[] { alquilerNoNombre, "El nombre del cliente es obligatorio." }, 
             
                new object[] { alquilerNoApellido, "El apellido del cliente es obligatorio." },
              
                new object[] { alquilerNoDireccion, "La dirección de envío es obligatoria." }, 
                
                new object[] { alquilerCantidadCero, "Error: La cantidad de una herramienta en el alquiler debe ser mayor a 0." }, 
               
                new object[] { alquilerHerramienteInexistente, "La herramienta no existe." }, 
                
                new object[] { alquilerFechaInvalida, "La fecha de fin debe ser posterior a la fecha de inicio." },
            };
            return allTest;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_CrearAlquiler))]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CrearAlquiler_ErrorDatosInvalidos_ReturnsBadRequest(CrearAlquilerDTO alquilerDTO, string errorMessage)
        {
            var morck = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = morck.Object;
            // Arrange
            var controller = new AlquileresController(_context, logger);

            // Act
            var result = await controller.CrearAlquiler(alquilerDTO);

            // --- CAMBIO: Bloque Assert TOTALMENTE REESCRITO ---
            // 1. Verificamos que es un BadRequest
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            // 2. Comprobamos si el error viene del validador automático (ValidationProblemDetails)
            //    o de tu lógica de negocio (un string)
            if (badRequestResult.Value is ValidationProblemDetails problemDetails)
            {
                // 3. Obtenemos el PRIMER mensaje de error de la PRIMERA propiedad que falló
                var errorActual = problemDetails.Errors.Values.First().First();

                // 4. Comparamos el mensaje de error esperado con el mensaje de error real
                Assert.Equal(errorMessage, errorActual);
            }
            else
            {
                // Si es un string (viene de tu lógica de negocio, ej: "Herramienta no existe")
                var errorActual = Assert.IsType<string>(badRequestResult.Value);
                Assert.Equal(errorMessage, errorActual);
            }
            // --------------------------------------------------
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CrearAlquiler_Success_test()
        {
            var morck = new Mock<ILogger<AlquileresController>>();
            ILogger<AlquileresController> logger = morck.Object;

            var controller = new AlquileresController(_context, logger);
            var alquilerItems = new List<AlquilerItemDTO>() { new AlquilerItemDTO("Martillo", "Madera", 35, 2) };
            var alquilerDTO = new CrearAlquilerDTO("Daniel", "de la Cruz", "Calle Tejares", DateTime.Today.AddDays(1), DateTime.Today.AddDays(5), (int)TiposMetodosPago.TarjetaCredito, alquilerItems);
            var ExpectedAlquiler = new AlquilerDetalleDTO("Daniel", "de la Cruz", "Calle Tejares", DateTime.Now, 70, DateTime.Today.AddDays(1), DateTime.Today.AddDays(5), alquilerItems);
            // Act
            var result = await controller.CrearAlquiler(alquilerDTO);
            // Assert
            var okResult = Assert.IsType<CreatedAtActionResult>(result);
            var alquilerResult = Assert.IsType<AlquilerDetalleDTO>(okResult.Value);
            Assert.Equal(ExpectedAlquiler, alquilerResult);

        }
    }
}
