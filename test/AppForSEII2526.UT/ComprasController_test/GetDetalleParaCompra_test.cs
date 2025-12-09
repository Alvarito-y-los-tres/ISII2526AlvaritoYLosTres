using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;

namespace AppForSEII2526.UT.ComprasController_test
{
    public class GetDetalleParaCompra_test : AppForSEII25264SqliteUT
    {
        public GetDetalleParaCompra_test()
        {
            var fabricante = new Fabricante("Ferretería López");

            var herramientas = new List<Herramienta>()
            {
                new Herramienta ("Martillo", "Madera", 35, 3, fabricante),
                new Herramienta ("Alicates", "Acero", 12, 1, fabricante),
            };

            ApplicationUser usuario = new ApplicationUser("Ana", "López", "642892748", "ana@gmail.com");

            var compra = new Compra("Calle Falsa 123", DateTime.Today, 47, TiposMetodosPago.TarjetaCredito, new List<CompraItem>(), usuario);

            compra.CompraItems.Add(new CompraItem(3, "grande", 35, herramientas[0], compra));

            _context.Add(fabricante);
            _context.AddRange(herramientas);
            _context.Add(compra);
            _context.SaveChanges();

        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit testing")]
        public async Task GetCompraDetalle_NotFound_test()
        {
            //arange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;

            var controller = new ComprasController(_context, logger);

            //act
            var result = await controller.GetCompraDetalle(0);

            //assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit testing")]
        public async Task GetCompraDetalle_Found_test()
        {
            //arrange
            var mock = new Mock<ILogger<ComprasController>>();
            ILogger<ComprasController> logger = mock.Object;
            var controller = new ComprasController(_context, logger);

            var expectedCompra = new CompraDetalleDTO(1,"Ana", "López", "Calle Falsa 123", 47, DateTime.Today, new List<CompraItemDTO>());
            expectedCompra.Items.Add(new CompraItemDTO(1, "Martillo", "Madera", 35, "grande", 3,1));

            //act
            var result = await controller.GetCompraDetalle(1);

            //assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var compraResult = Assert.IsType<CompraDetalleDTO>(okResult.Value);
            var eq = expectedCompra.Equals(compraResult);

            Assert.Equal(expectedCompra, compraResult);


        }
        }
}
