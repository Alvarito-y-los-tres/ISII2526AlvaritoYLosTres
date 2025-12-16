using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace AppForSEII2526.UIT.CU_OfertaHerramientas
{
    public class DetalleOferta_PO : PageObject
    {
        private By herramientasOfertaTable = By.Id("HerramientasEnOferta");
        private By botonPrecioTotal = By.Id("TotalPrice");

        public DetalleOferta_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public bool CheckOfertaCreadaCorrectamente(List<string[]> expectedDetalles)
        {
            return CheckBodyTable(expectedDetalles, herramientasOfertaTable);
        }

        public bool CheckOfertaDetalles(DateTime fechaInicio, DateTime fechaFin, DateTime fechaOferta, int? dirigidaA, int metodoPago)
        {
            WaitForBeingVisible(botonPrecioTotal);
            bool result = true;

            result = result && _driver.FindElement(By.Id("FechaInicio")).Text.Contains(fechaInicio.ToString("dd/MM/yyyy"));
            result = result && _driver.FindElement(By.Id("FechaFinal")).Text.Contains(fechaFin.ToString("dd/MM/yyyy"));
            result = result && _driver.FindElement(By.Id("FechaOferta")).Text.Contains(fechaOferta.ToString("dd/MM/yyyy"));

            string _dirigidoA = _driver.FindElement(By.Id("TipoDirigida")).Text;
            if (!dirigidaA.HasValue){
                result = result && string.IsNullOrEmpty(_dirigidoA);
            }
            else if (dirigidaA.Value == 0)
            {
                result = result && _dirigidoA.Contains("Socios");
            }
            else if (dirigidaA.Value == 1)
            {
                result = result && _dirigidoA.Contains("Clientes");
            }

            string _metodoPago = "";
            switch (metodoPago)
            {
                case 0: _metodoPago = "TarjetaCredito"; break;
                case 1: _metodoPago = "Paypal"; break;
                case 2: _metodoPago = "Cash"; break;
            }

            result = result && _driver.FindElement(By.Id("MetodoPago")).Text.Contains(_metodoPago);

            return result;
        }

    }
}
