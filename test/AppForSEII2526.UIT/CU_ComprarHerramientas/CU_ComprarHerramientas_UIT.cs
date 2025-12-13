
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.UIT.CU_ComprarHerramientas
{
    public class CU_ComprarHerramientas_UIT : UC_UIT
    {

        private SelectHerramientasParaComprar_PO selectHerramientasParaComprar_PO;
        private CrearCompra_PO crearCompra_PO;
        private DetalleCompra_PO detalleCompra_PO;

        private const string herramienta1Id = "1";
        private const string herramienta1Nombre = "Martillo";
        private const string herramienta1Material = "Madera";
        private const string herramienta1Precio = "5";
        private const string herramienta1Fabricante = "Ferretería López";


        private const string herramienta2Id = "2";
        private const string herramienta2Nombre = "Taladro";
        private const string herramienta2Material = "Acero";
        private const string herramienta2Precio = "30";
        private const string herramienta2Fabricante = "Ferretería García";

        private const string herramienta3Id = "3";
        private const string herramienta3Nombre = "Alicates";
        private const string herramienta3Material = "Hierro";
        private const string herramienta3Precio = "4";
        private const string herramienta3Fabricante = "Ferretería Ruiz";

        public CU_ComprarHerramientas_UIT(ITestOutputHelper output) : base(output)
        {
            selectHerramientasParaComprar_PO = new SelectHerramientasParaComprar_PO(_driver, _output);
            crearCompra_PO = new CrearCompra_PO(_driver, _output);
            detalleCompra_PO = new DetalleCompra_PO(_driver, _output);
        }


        private void InitialStepsForCompraHerramientas()
        {
            Initial_step_opening_the_web_page();

            
            By id = By.Id("CrearCompra");
            selectHerramientasParaComprar_PO.WaitForBeingVisible(id);

            Thread.Sleep(500);

            _driver.FindElement(id).Click();
        }

        [Theory]
        [InlineData(herramienta2Id, herramienta2Nombre, herramienta2Material, herramienta2Precio, herramienta2Fabricante, "Acero", "")]
        [InlineData(herramienta3Id, herramienta3Nombre, herramienta3Material, herramienta3Precio, herramienta3Fabricante, "", "4")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC1_2_3_AFO_filteringPorMaterialPrecio(string herramientaId, string herramientaNombre, string herramientaMaterial, string herramientaPrecio, string herramientaFabricante, string filtroMaterial, string filtroPrecio)
        {
            //Arrange
            InitialStepsForCompraHerramientas();
            var expectedHerramientas = new List<string[]> { new string[] { herramientaNombre, herramientaMaterial, herramientaPrecio, herramientaFabricante }, };



            //Act
            selectHerramientasParaComprar_PO.BuscarHerramientas(filtroMaterial, filtroPrecio);

            Thread.Sleep(500);

            //Assert
            Assert.True(selectHerramientasParaComprar_PO.CheckListOfHerramientasInCart(expectedHerramientas));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_5_FA2_ModificarCarritoCompra()
        {

            //Arrange
            InitialStepsForCompraHerramientas();
            selectHerramientasParaComprar_PO.BuscarHerramientas("", "0");
            Thread.Sleep(500);

            //Act
            selectHerramientasParaComprar_PO.AñadirHerramientaToCart(herramienta1Nombre);
            Thread.Sleep(500);
            selectHerramientasParaComprar_PO.AñadirHerramientaToCart(herramienta2Nombre);
            Thread.Sleep(500);
            selectHerramientasParaComprar_PO.ComprarHerramientas();
            Thread.Sleep(500);
            crearCompra_PO.ModificarCompra();
            Thread.Sleep(500);
            selectHerramientasParaComprar_PO.QuitarHerramientaFromCart(herramienta2Nombre);
            Thread.Sleep(500);
            selectHerramientasParaComprar_PO.ComprarHerramientas();
            Thread.Sleep(500);
            var expectedHerramientas = new List<string[]>
            {
            new[]
              {
                 $"{herramienta1Nombre} {herramienta1Material}{Environment.NewLine}{herramienta1Precio}"
                }
            };

            Assert.True(crearCompra_PO.CheckListHerramientasEnCompra(expectedHerramientas));



        }

        //Flujo Alternativo 3 al paso 4
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_4_FA3_NotHerramientas()
        {

            //Arrange
            InitialStepsForCompraHerramientas();
            selectHerramientasParaComprar_PO.BuscarHerramientas("", "0");
            Thread.Sleep(500);

            //Act
            
            Thread.Sleep(500);

            //Assert
            Assert.True(selectHerramientasParaComprar_PO.CompraNotAvailable());


        }
        //Flujo Básico
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_FB_CompraHerramienta()
        {

            //Arrange
            InitialStepsForCompraHerramientas();
            selectHerramientasParaComprar_PO.BuscarHerramientas("", "0");
            Thread.Sleep(500);
            var expectedHerramientas = new List<string[]>
            {
                new string[] { herramienta1Nombre, herramienta1Material, "1", "5 €", "Nueva" }

            };

            //Act
            selectHerramientasParaComprar_PO.AñadirHerramientaToCart(herramienta1Nombre);
            Thread.Sleep(500);
            selectHerramientasParaComprar_PO.ComprarHerramientas();
            Thread.Sleep(500);
            crearCompra_PO.RellenarFormulario("Ana", "López", "ana.lopez@gmail.com", "Calle Feria", "PayPal", "Nueva");
            Thread.Sleep(500);
            crearCompra_PO.SubmitCompra();
            Thread.Sleep(500);
            crearCompra_PO.ConfirmCompra();
            Thread.Sleep(500);

            //Assert

            Assert.True(detalleCompra_PO.CheckDetalleCompra("Ana", "López", "Calle Feria", DateTime.Today, 5), "Falla CheckDetalleCompra");

            Assert.True(detalleCompra_PO.CheckListHerramienta(expectedHerramientas), "Falla CheckListHerramienta");


        }


        //Flujo Alternativo 4 al paso 6
        [Theory]
        [InlineData("", "López", "Calle Feria", "Nombre")]
        [InlineData("Ana", "", "Calle Feria", "Apellido")]
        [InlineData("Ana", "López", "", "Direccion")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_6_FA5_camposNoValidos(string nombre, string apellido, string direccion, string error)
        {

            //Arrange
            InitialStepsForCompraHerramientas();
            selectHerramientasParaComprar_PO.BuscarHerramientas("", "0");
            Thread.Sleep(500);

            //Act
            selectHerramientasParaComprar_PO.AñadirHerramientaToCart(herramienta1Nombre);
            Thread.Sleep(500);
            selectHerramientasParaComprar_PO.ComprarHerramientas();
            Thread.Sleep(500);
            crearCompra_PO.RellenarFormulario(nombre, apellido, correo: "ana.lopez@gmail.com", direccion, "PayPal", "Nueva");
            Thread.Sleep(500);
            crearCompra_PO.SubmitCompra();
            Thread.Sleep(500);

            //Assert
            Assert.True(crearCompra_PO.CheckError(error));

        }

        //Flujo Alternativo 5 al paso 6
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void CU1_6_FA5_noHayCantidad()
        {

            //Arrange
            InitialStepsForCompraHerramientas();
            selectHerramientasParaComprar_PO.BuscarHerramientas("", "0");
            Thread.Sleep(500);

            //Act
            selectHerramientasParaComprar_PO.AñadirHerramientaToCart(herramienta1Nombre);
            Thread.Sleep(500);
            selectHerramientasParaComprar_PO.ComprarHerramientas();
            Thread.Sleep(500);
            crearCompra_PO.RellenarFormulario("Ana", "López", "ana.lopez@gmail.com", "Calle Feria", "PayPal", "Nueva");
            Thread.Sleep(500);
            crearCompra_PO.EstablecerCantidadPorNombre(herramienta1Nombre, "0");
            Thread.Sleep(500);
            crearCompra_PO.SubmitCompra();
            Thread.Sleep(500);
            crearCompra_PO.ConfirmCompra();
            Thread.Sleep(500);

            _output.WriteLine("URL actual: " + _driver.Url);

            //Assert
            Assert.True(crearCompra_PO.CheckError("Cantidad"));



        }
    }
}


        


