using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.OfertasController_test
{
    public class GetDetalleParaOferta_test : AppForSEII25264SqliteUT
    {
        public GetDetalleParaOferta_test()
        {
            var fabricante = new List<Fabricante>()
            {
                new Fabricante ("Ferretería López"),
                new Fabricante ("Ferretería García"),
                new Fabricante ("Ferretería Ruiz")
            };

            var herramienta = new List<Herramienta>()
            {
                new Herramienta ("Martillo", "Madera", 5, 3, fabricante[0]),
                new Herramienta ("Taladro", "Acero", 30, 14, fabricante[1]),
                new Herramienta ("Alicates", "Hierro", 4, 5, fabricante[2])
            };

            var usuario = new ApplicationUser("Álvaro", "Cano", "660111222", "alvaro.cano@example.com");

            var oferta = new Oferta(DateTime.Today.AddDays(2), DateTime.Today.AddDays(3), DateTime.Today, new List<OfertaItem>(), TiposMetodosPago.Paypal, TiposDirigidaOferta.Clientes, usuario);
            oferta.OfertaItems.Add(new OfertaItem(herramienta[0], oferta, 20, 4));

            _context.Fabricantes.AddRange(fabricante);
            _context.AddRange(herramienta);
            _context.Add(oferta);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit testing")]
        public async Task GetOfertaDetalle_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<OfertasController>>();
            ILogger<OfertasController> logger = mock.Object;

            var controller = new OfertasController(_context, logger);

            // Act
            var result = await controller.GetOfertaDetalle(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit testing")]
        public async Task GetOfertaDetalle_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<OfertasController>>();
            ILogger<OfertasController> logger = mock.Object;
            var controller = new OfertasController(_context, logger);

            var expectedOferta = new OfertaDetalleDTO(DateTime.Today.AddDays(2), DateTime.Today.AddDays(3),
                        DateTime.Today, TiposDirigidaOferta.Clientes, TiposMetodosPago.Paypal,
                        new List<OfertaItemDTO>());
            expectedOferta.Items.Add(new OfertaItemDTO("Martillo", "Madera", "Ferretería López", 5, 4, 20, 1));

            // Act 
            var result = await controller.GetOfertaDetalle(1);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var ofertaDTOActual = Assert.IsType<OfertaDetalleDTO>(okResult.Value);

            Assert.Equal(expectedOferta, ofertaDTOActual);
        }
    }
}