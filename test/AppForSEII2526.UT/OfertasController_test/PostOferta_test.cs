using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.UT.OfertasController_test
{
    public class PostOferta_test : AppForSEII25264SqliteUT
    {
        public PostOferta_test()
        {
            var fabricante = new Fabricante("Ferretería López");

            var herramienta = new List<Herramienta>()
            {
                new Herramienta ("Martillo", "Madera", 5, 3, fabricante),
                new Herramienta ("Taladro", "Acero", 30, 14, fabricante)
            };

            var usuario = new ApplicationUser("Álvaro", "Cano", "660111222", "alvaro.cano@example.com");

            var oferta = new Oferta(DateTime.Today.AddDays(2), DateTime.Today.AddDays(3), DateTime.Today, new List<OfertaItem>(), TiposMetodosPago.Paypal, TiposDirigidaOferta.Clientes, usuario);
            oferta.OfertaItems.Add(new OfertaItem(herramienta[0], oferta, 20, 4));

            _context.Add(fabricante);
            _context.AddRange(herramienta);
            _context.AddRange(usuario);
            _context.Add(oferta);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> CasosTestPara_CrearOferta()
        {
            var ofertaNoItem = new CrearOfertaDTO
            {
                FechaInicio = DateTime.Today.AddDays(2),
                FechaFin = DateTime.Today.AddDays(10),
                FechaOferta = DateTime.Today,
                Items = new List<OfertaItemDTO>(),
                ParaSocio = 1,
                MetodoPago = 0,
                NombreUsuario = "Álvaro"

            };

            var ofertaItems = new List<OfertaItemDTO>
            {
                new OfertaItemDTO("Martillo", "Madera", "Ferretería López", 5, 4, 20, 1)
            };


            var ofertaFromBeforeToday = new CrearOfertaDTO
            {
                FechaInicio = DateTime.Today.AddDays(-1),
                FechaFin = DateTime.Today.AddDays(10),
                FechaOferta = DateTime.Today,
                Items = ofertaItems,
                ParaSocio = 1,
                MetodoPago = 0,
                NombreUsuario = "Álvaro"
            };

            var ofertaToBeforeFrom = new CrearOfertaDTO
            {
                FechaInicio = DateTime.Today.AddDays(10),
                FechaFin = DateTime.Today.AddDays(3),
                FechaOferta = DateTime.Today,
                Items = ofertaItems,
                ParaSocio = 1,
                MetodoPago = 0,
                NombreUsuario = "Álvaro"
            };

            var ofertaFechaFinInvalida = new CrearOfertaDTO
            {
                FechaInicio = DateTime.Today.AddDays(2),
                FechaFin = DateTime.Today.AddDays(3),
                FechaOferta = DateTime.Today,
                Items = ofertaItems,
                ParaSocio = 1,
                MetodoPago = 0,
                NombreUsuario = "Álvaro"
            };

            var ofertaApplicationUser = new CrearOfertaDTO
            {
                FechaInicio = DateTime.Today.AddDays(2),
                FechaFin = DateTime.Today.AddDays(10),
                FechaOferta = DateTime.Today,
                Items = ofertaItems,
                ParaSocio = 1,
                MetodoPago = 0,
                NombreUsuario = "Roberto"
            };

            var ofertaNoDisponible = new CrearOfertaDTO
            {
                FechaInicio = DateTime.Today.AddDays(2),
                FechaFin = DateTime.Today.AddDays(10),
                FechaOferta = DateTime.Today,
                Items = new List<OfertaItemDTO> { new OfertaItemDTO("Apisonadora", "Madera", "Ferretería López", 5, 4, 20, 1) },
                ParaSocio = 1,
                MetodoPago = 0,
                NombreUsuario = "Álvaro"
            };

            var ofertaMetodoPagoInvalido = new CrearOfertaDTO
            {
                FechaInicio = DateTime.Today.AddDays(2),
                FechaFin = DateTime.Today.AddDays(10),
                FechaOferta = DateTime.Today,
                Items = ofertaItems,
                ParaSocio = 1,
                MetodoPago = 33,
                NombreUsuario = "Álvaro"
            };

            var ofertaTipoInvalido = new CrearOfertaDTO
            {
                FechaInicio = DateTime.Today.AddDays(2),
                FechaFin = DateTime.Today.AddDays(10),
                FechaOferta = DateTime.Today,
                Items = ofertaItems,
                ParaSocio = 33,
                MetodoPago = 0,
                NombreUsuario = "Álvaro"
            };

            var ofertaPorcentajeInvalido = new CrearOfertaDTO
            {
                FechaInicio = DateTime.Today.AddDays(2),
                FechaFin = DateTime.Today.AddDays(10),
                FechaOferta = DateTime.Today,
                Items = new List<OfertaItemDTO> { new OfertaItemDTO("Martillo", "Madera", "Ferretería López", 5, 4, 120, 1) },
                ParaSocio = 1,
                MetodoPago = 0,
                NombreUsuario = "Álvaro"
            };


            var allTests = new List<object[]>
            {
                new object[] { ofertaNoItem, "La oferta debe contener al menos una herramienta." },
                new object[] { ofertaFromBeforeToday, "La fecha de inicio debe ser futura." },
                new object[] { ofertaToBeforeFrom, "La fecha de fin debe ser posterior a la fecha de inicio." },
                new object[] { ofertaFechaFinInvalida, "¡Error!, la oferta debe durar al menos una semana." },
                new object[] { ofertaApplicationUser, "El usuario no existe." },
                new object[] { ofertaNoDisponible, "La herramienta no existe." },
                new object[] { ofertaMetodoPagoInvalido, "El método de pago especificado no es válido. ¡Utilice 0, 1 o 2!" },
                new object[] { ofertaTipoInvalido, "El tipo de oferta especificado no es válido. ¡Utilice el 0 o 1!" },
                new object[] { ofertaPorcentajeInvalido, "El porcentaje debe estar entre 0 y 100." }
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(CasosTestPara_CrearOferta))]
        public async Task CrearOferta_Error_test(CrearOfertaDTO ofertaDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<OfertasController>>();
            ILogger<OfertasController> logger = mock.Object;

            var controller = new OfertasController(_context, logger);

            // Act
            var result = await controller.CrearOferta(ofertaDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual);
        }



        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CrearOferta_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<OfertasController>>();
            ILogger<OfertasController> logger = mock.Object;

            var controller = new OfertasController(_context, logger);

            var ofertaItems = new List<OfertaItemDTO>
            {
                new OfertaItemDTO("Martillo", "Madera", "Ferretería López", 5, 4, 20, 1)
            };

            var oferta = new CrearOfertaDTO
            {
                FechaInicio = DateTime.Today.AddDays(2),
                FechaFin = DateTime.Today.AddDays(10),
                FechaOferta = DateTime.Today,
                Items = ofertaItems,
                ParaSocio = 1,
                MetodoPago = 0,
                NombreUsuario = "Álvaro"
            };

            var expectedOfertaDetalleDTO = new OfertaDetalleDTO(1,DateTime.Today.AddDays(2), DateTime.Today.AddDays(10), DateTime.Today, TiposDirigidaOferta.Clientes, TiposMetodosPago.TarjetaCredito, ofertaItems);

            // Act
            var result = await controller.CrearOferta(oferta);

            //Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualOfertaDetailDTO = Assert.IsType<OfertaDetalleDTO>(createdResult.Value);

            Assert.Equal(expectedOfertaDetalleDTO, actualOfertaDetailDTO);
        }
    }
}