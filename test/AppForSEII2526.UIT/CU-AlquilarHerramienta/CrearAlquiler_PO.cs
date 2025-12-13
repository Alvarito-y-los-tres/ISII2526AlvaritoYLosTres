using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;
using System;

namespace AppForSEII2526.UIT.CU_Alquiler
{
    public class CrearAlquiler_PO : PageObject
    {
        
        private By _nombre = By.Id("Name");
        private By _apellido = By.Id("Surname");
        private By _telefono = By.Id("numTelefono");         
        private By _email = By.Id("CorreoElectronico");      
        private By _direccion = By.Id("DeliveryAddress");
        private By _metodoPago = By.Id("TiposMetodoPago");
        private By _fechaInicio = By.Id("FechaInicio");
        private By _fechaFin = By.Id("FechaFin");
        private By _submitButton = By.Id("Submit");
        private By _modificarButton = By.Id("ModifyAlquilerItems");
        private By _errorAlert = By.ClassName("alert-danger");

        public CrearAlquiler_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void RellenarDatosCliente(string nombre, string apellido, string direccion, string telefono, string email, string pagoValue)
        {
            WaitForBeingVisible(_nombre);

            _driver.FindElement(_nombre).Clear();
            _driver.FindElement(_nombre).SendKeys(nombre);

            _driver.FindElement(_apellido).Clear();
            _driver.FindElement(_apellido).SendKeys(apellido);

            _driver.FindElement(_direccion).Clear();
            _driver.FindElement(_direccion).SendKeys(direccion);

            //opcional
            if (!string.IsNullOrEmpty(telefono))
            {
                _driver.FindElement(_telefono).Clear();
                _driver.FindElement(_telefono).SendKeys(telefono);
            }
            //opcional tambien
            if (!string.IsNullOrEmpty(email))
            {
                _driver.FindElement(_email).Clear();
                _driver.FindElement(_email).SendKeys(email);
            }

            new SelectElement(_driver.FindElement(_metodoPago)).SelectByValue(pagoValue);
        }

        public void RellenarFechas(DateTime inicio, DateTime fin)
        {
            WaitForBeingVisible(_fechaInicio);

            InputDateInDatePicker(_fechaInicio, inicio);
            InputDateInDatePicker(_fechaFin, fin);
        }

        public void EstablecerCantidad(string nombreHerramienta, int cantidad)
        {
            By inputCantidad = By.Id($"cantidad_{nombreHerramienta}");

            WaitForBeingVisible(inputCantidad);
            var element = _driver.FindElement(inputCantidad);

            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Delete);
            element.SendKeys(cantidad.ToString());
        }

        public void PulsarCrearAlquiler()
        {
            WaitForBeingClickable(_submitButton);
            _driver.FindElement(_submitButton).Click();
        }

        public void PulsarModificarCarrito()
        {
            WaitForBeingClickable(_modificarButton);
            _driver.FindElement(_modificarButton).Click();
        }

        public void ConfirmarModal()
        {
            PressOkModalDialog();
        }

        public bool CheckErrorMessage(string message)
        {
            try
            {
                WaitForBeingVisible(_errorAlert);
                string actualError = _driver.FindElement(_errorAlert).Text;
                return actualError.Contains(message, StringComparison.InvariantCultureIgnoreCase);
            }
            catch (WebDriverTimeoutException)
            {
                return false; 
            }
        }
    }
}