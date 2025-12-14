using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.CU_AlquilarHerramienta
{
    public class SelectHerramientaParaAlquilar_PO : PageObject
    {
        private By inputNombre = By.Id("inputNombre");
        private By inputMaterial = By.Id("inputPrecio");
        private By btnBuscar = By.Id("buscarHerramientas");
        private By tableHerramientas = By.Id("Tabla de herramientas");
        private By btnAlquilar = By.Id("alquilarHerramientasBoton");
        public SelectHerramientaParaAlquilar_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }
        public void Buscarherramientas(string nombre, string material)
        {
            System.Threading.Thread.Sleep(1000);
            WaitForBeingClickable(inputNombre);

            var nombreBox = _driver.FindElement(inputNombre);
            nombreBox.Clear();

            if (!string.IsNullOrEmpty(nombre))
            {
                nombreBox.SendKeys(nombre);
            }

            var materialBox = _driver.FindElement(inputMaterial);
            materialBox.Clear();

            if (!string.IsNullOrEmpty(material))
            {
                materialBox.SendKeys(material);
            }

            WaitForBeingClickable(btnBuscar);
            _driver.FindElement(btnBuscar).Click();
        }

        public void AddHerramientaToAlquilerCart(string nombreHerramienta)
        {
            By btnAddLocator = By.Id($"herramientaParaAlquilar_{nombreHerramienta}");

            WaitForBeingClickable(btnAddLocator);
            _driver.FindElement(btnAddLocator).Click();

            By btnRemoveLocator = By.Id($"eliminarHerramienta_{nombreHerramienta}");
            WaitForBeingClickable(btnRemoveLocator);
        }
        public void RemoveHerramientaFromAlquilerCart(string nombreHerramienta)
        {
            By btnRemoveLocator = By.Id($"eliminarHerramienta_{nombreHerramienta}");
            WaitForBeingClickable(btnRemoveLocator);
            _driver.FindElement(btnRemoveLocator).Click();

            By btnAddLocator = By.Id($"herramientaParaAlquilar_{nombreHerramienta}");
            WaitForBeingClickable(btnAddLocator);
        }
        public void PressAlquilar()
        {
             WaitForBeingClickable(btnAlquilar);
            _driver.FindElement(btnAlquilar).Click();
        }
        public bool CheckCartVisibility(bool shouldisVisible)
        {
            Thread.Sleep(1000);
            var botonesAlquilar = _driver.FindElements(btnAlquilar);
            bool isVisible = botonesAlquilar.Count > 0 && botonesAlquilar[0].Displayed;
            return isVisible == shouldisVisible;
        }
        public void BorrarHerramientasDelCarrito(string nombreHerramienta)
        {
            By btnEliminar = By.Id($"eliminarHerramienta_{nombreHerramienta}");

            WaitForBeingClickable(btnEliminar);
            _driver.FindElement(btnEliminar).Click();

            System.Threading.Thread.Sleep(500);
        }
        // --- MÉTODOS A AÑADIR ---

        public bool HayResultadosEnTabla()
        {
            var filas = _driver.FindElements(By.CssSelector("table tbody tr"));
            return filas.Count > 0;
        }

        public bool VerificarTextoEnTabla(string texto)
        {
            
            return _driver.PageSource.Contains(texto);
        }

    }
}
