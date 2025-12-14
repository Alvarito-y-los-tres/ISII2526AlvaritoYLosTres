using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;
using System;

namespace AppForSEII2526.UIT.CU_OfertaHerramientas
{
    public class CrearOferta_PO : PageObject
    {
        private By _fechaInicio = By.Id("FechaInicio");
        private By _fechaFin = By.Id("FechaFin");
        private By _metodoPago = By.Id("MetodoPago");
        private By _nombreUsuario = By.Id("NombreUsuario");
        private By _paraSocio = By.Id("DirigidaA");
        private By _botonCrearOferta = By.Id("CrearOferta");
        private By _botonVaciarOferta = By.Id("LimpiarCarrito");
        private By _errores = By.Id("ErrorsShown");
        private By _botonModificar = By.Id("ModificarHerramientas");

        public CrearOferta_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void RellenarDatos(DateTime fechaInicio, DateTime fechaFin, int metodoPago, string nombreUsuario, int? paraSocio)
        {
            WaitForBeingVisible(_fechaInicio);
            InputDateInDatePicker(_fechaInicio, fechaInicio);
            InputDateInDatePicker(_fechaFin, fechaFin);

            new SelectElement(_driver.FindElement(_metodoPago)).SelectByValue(metodoPago.ToString());

            _driver.FindElement(_nombreUsuario).Clear();
            _driver.FindElement(_nombreUsuario).SendKeys(nombreUsuario);

            new SelectElement(_driver.FindElement(By.Id("DirigidaA")));
            if (paraSocio.HasValue)
            {
                new SelectElement(_driver.FindElement(_paraSocio)).SelectByValue(paraSocio.Value.ToString());
            }
            else
            {
                new SelectElement(_driver.FindElement(_paraSocio)).SelectByValue("");
            }
        }

        public void EstablecerPorcentaje(string nombreHerramienta, int porcentaje)
        {
            By porcentajeInput = By.Id($"Porcentaje_{nombreHerramienta}");

            WaitForBeingVisible(porcentajeInput);
            var element = _driver.FindElement(porcentajeInput);

            element.SendKeys(Keys.Control + "a");
            element.SendKeys(Keys.Delete);
            element.SendKeys(porcentaje.ToString());
        }

        public void PulsarCrearOferta()
        {
            WaitForBeingClickable(_botonCrearOferta);
            _driver.FindElement(_botonCrearOferta).Click();
        }

        public void VaciarCarrito()
        {
            WaitForBeingClickable(_botonVaciarOferta);
            _driver.FindElement(_botonVaciarOferta).Click();
        }

        public void ModificarHerramientas()
        {
            WaitForBeingClickable(_botonModificar);
            _driver.FindElement(_botonModificar).Click();
        }

        public void ConfirmarModal()
        {
            PressOkModalDialog();
        }
        public bool EstaActivoBotonCrear()
        {
            WaitForBeingVisible(_botonCrearOferta);
            return _driver.FindElement(_botonCrearOferta).Enabled;
        }

        public bool CheckErroresMensaje(string message)
        {
            try
            {
                WaitForBeingVisible(_errores);
                string actualError = _driver.FindElement(_errores).Text;
                return actualError.Contains(message);
            }
            catch (WebDriverTimeoutException)
            {
                return false; //No apareció msj de error
            }
        }
    }
}