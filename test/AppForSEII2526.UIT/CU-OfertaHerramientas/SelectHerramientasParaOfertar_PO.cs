using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppForSEII2526.UIT.CU_OfertaHerramientas
{
    internal class SelectHerramientasParaOfertar_PO : PageObject
    {
        private By inputFabricante = By.Id("inputFabricante");
        private By inputPrecio = By.Id("inputPrecio");
        private By botonBuscar = By.Id("buscarHerraminetas");
        private By tablaHerramientas = By.Id("Tabla de herramientas");
        private By botonContinuar = By.Id("ofertarHerramientasBoton");

        public SelectHerramientasParaOfertar_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void BuscarHerramientas(string? fabricante, double? precio)
        {
            WaitForBeingClickable(inputFabricante);
            if (fabricante != null)
            {
                var fabricanteInputElement = _driver.FindElement(inputFabricante);
                fabricanteInputElement.SendKeys(fabricante);
            }
            if (precio != null)
            {
                var precioInputElement = _driver.FindElement(inputPrecio);
                precioInputElement.Clear();
                precioInputElement.SendKeys(precio.ToString());
            }
            _driver.FindElement(botonBuscar).Click();
        }

        public bool CheckListaHerramientasCargadas(List<string[]> expectedHerramientas)
        {
            return CheckBodyTable(expectedHerramientas, tablaHerramientas);
        }

        public void EliminarHerramienta(string nombreHerramienta)
        {
            By botonEliminarHerramienta = By.Id($"eliminarHerramienta_{nombreHerramienta}");

            WaitForBeingClickable(botonEliminarHerramienta);
            _driver.FindElement(botonEliminarHerramienta).Click();
        }

        public bool CheckCarritoVacio()
        {
            var botonContinuar = _driver.FindElements(By.Id("ofertarHerramientasBoton"));

            if (botonContinuar.Count > 0 && botonContinuar[0].Displayed)
            {
                return false;

                throw new Exception("Error: El carrito debería estar vacío, pero el botón 'Continuar' es visible.");
            }
            return true;
        }

        public void AñadirHerramientaAlCarrito(string nombreHerramienta)
        {
            By btnAddLocator = By.Id($"herramientaParaOferta_{nombreHerramienta}");

            WaitForBeingClickable(btnAddLocator);
            _driver.FindElement(btnAddLocator).Click();

            By btnRemoveLocator = By.Id($"eliminarHerramienta_{nombreHerramienta}");

            WaitForBeingClickable(btnRemoveLocator);
        }

        public void ContinuarConOferta()
        {
            WaitForBeingClickable(botonContinuar);
            _driver.FindElement(botonContinuar).Click();
        }
    }
}
