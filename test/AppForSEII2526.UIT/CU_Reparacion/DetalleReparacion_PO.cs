using OpenQA.Selenium;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;
using System.Linq;

namespace AppForSEII2526.UIT.CU_Reparacion
{
    public class DetalleReparacion_PO : PageObject
    {

        private By _nombreCompleto = By.Id("NameSurname");
        private By _fechaEntrega = By.Id("fechaentrega");
        private By _fechaRecogida = By.Id("fecharecogida"); 
        private By _precioTotal = By.Id("TotalPrice");
        private By _tablaItems = By.Id("ReparacionItems");

        public DetalleReparacion_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public string GetNombreCliente()
        {
            WaitForBeingVisible(_nombreCompleto);
            return _driver.FindElement(_nombreCompleto).Text;
        }

        public string GetPrecioTotal()
        {
            WaitForBeingVisible(_precioTotal);
            return _driver.FindElement(_precioTotal).Text;
        }

        public string GetFechaEntrega()
        {
            WaitForBeingVisible(_fechaEntrega);
            return _driver.FindElement(_fechaEntrega).Text;
        }
        public string GetFechaRecogida()
        {
            WaitForBeingVisible(_fechaRecogida);
            return _driver.FindElement(_fechaRecogida).Text;
        }
        

        public bool ValidarItemEnTabla(string nombreHerramienta, int cantidad, string precio)
        {
            // Buscamos la fila específica del item 
            try 
            {
                By rowLocator = By.Id($"RentalItem_{nombreHerramienta}");
                WaitForBeingVisible(rowLocator);
                var fila = _driver.FindElement(rowLocator);
                
                // Verificamos que el texto de la fila contenga los datos clave
                string textoFila = fila.Text;
                return textoFila.Contains(nombreHerramienta) && 
                       textoFila.Contains(cantidad.ToString()) && 
                       textoFila.Contains(precio);
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
    }
}