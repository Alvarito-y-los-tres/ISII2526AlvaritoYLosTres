using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;
using System;

namespace AppForSEII2526.UIT.CU_Reparacion
{
    public class CrearReparacion_PO : PageObject
    {
        
        private By _nombreCliente = By.Id("Name");
        private By _apellidoCliente = By.Id("Surname");
        private By _fechaEntrega = By.Id("DeliveryAddress"); 
        private By _telefono = By.Id("tlf");
        private By _metodoPago = By.Id("PaymentMethod");
        private By _btnCrear = By.Id("Submit");
        private By _errorBox = By.Id("ErrorsShown");
        private By _btnModificar = By.Id("ModifyMovies");

        public CrearReparacion_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void RellenarDatosCliente(string nombre, string apellido, DateTime fecha, string telefono, string metodoPagoValue)
        {
            WaitForBeingVisible(_nombreCliente);

            _driver.FindElement(_nombreCliente).SendKeys(nombre);
            _driver.FindElement(_apellidoCliente).SendKeys(apellido);

            // Método heredado para poner fechas
            InputDateInDatePicker(_fechaEntrega, fecha);

            if (!string.IsNullOrEmpty(telefono))
            {
                _driver.FindElement(_telefono).SendKeys(telefono);
            }

            // Seleccionar método de pago
            new SelectElement(_driver.FindElement(_metodoPago)).SelectByValue(metodoPagoValue);
        }

        // Método específico para rellenar la fila de la tabla de ítems
        public void RellenarDetalleItem(string nombreHerramienta, int cantidad, string problema)
        {
            // IDs dinámicos: cantidad_Martillo, description_Martillo
            By inputCantidad = By.Id($"cantidad_{nombreHerramienta}");
            By inputDesc = By.Id($"description_{nombreHerramienta}");

            WaitForBeingVisible(inputCantidad);

            var elemCant = _driver.FindElement(inputCantidad);
            elemCant.Clear(); // Importante borrar el 0 inicial si lo hay
            elemCant.SendKeys(cantidad.ToString());

            if (!string.IsNullOrEmpty(problema))
            {
                _driver.FindElement(inputDesc).SendKeys(problema);
            }
        }

        public void PulsarCrear()
        {
            WaitForBeingClickable(_btnCrear);
            _driver.FindElement(_btnCrear).Click();
        }

        public void ConfirmarModal()
        {
            PressOkModalDialog();
        }

        public bool CheckErrorMessage(string textoEsperado)
        {
            try
            {
                WaitForBeingVisible(_errorBox);
                string textoActual = _driver.FindElement(_errorBox).Text;
                return textoActual.Contains(textoEsperado, StringComparison.InvariantCultureIgnoreCase);
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }
        

        public bool EstaBotonCrearDeshabilitado()
        {
            // El botón tiene disabled="@reparacionButtonDisabled"
            var btn = _driver.FindElement(_btnCrear);
            // En HTML, si el atributo disabled está presente, Enabled será false en Selenium
            return !btn.Enabled; 
        }

        public bool HayMensajesDeValidacionCampo(string campoId)
        {
            // Validaciones de DataAnnotations suelen aparecer cerca del input o en el ValidationSummary
            // Aquí buscamos si el campo tiene clase 'invalid' o si hay un mensaje asociado
            try
            {
                // Blazor suele poner border red o clase modified invalid
                var input = _driver.FindElement(By.Id(campoId));
                return input.GetAttribute("class").Contains("invalid");
            }
            catch
            {
                return false;
            }
        }
       
        public void EsperarNavegacionADetalle()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            try
            {
                wait.Until(d => d.Url.Contains("DetalleReparacion"));
            }
            catch (WebDriverTimeoutException)
            {
                // Si salta el timeout, el test fallará luego en el Assert, 
                // pero esto nos da tiempo a que la API responda.
            }
        }
        public void PulsarModificar()
        {
            WaitForBeingClickable(_btnModificar);
            _driver.FindElement(_btnModificar).Click();
        }
    }
}