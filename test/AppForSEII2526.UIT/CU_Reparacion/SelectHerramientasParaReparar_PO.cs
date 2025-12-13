using OpenQA.Selenium;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.CU_Reparacion
{
    public class SelectHerramientasParaReparar_PO : PageObject
    {
        // IDs extraídos de SelectItemReparacion.razor
        private By _inputNombre = By.Id("inputTitle");
        private By _inputTiempo = By.Id("inputGenre"); // En tu razor se llamaba inputGenre
        private By _btnBuscar = By.Id("searchitems");
        private By _tableReparaciones = By.Id("TableOfRepairs");

        // El botón de pagar no tenía ID en tu código, lo buscamos por el texto del botón
        private By _btnContinuar = By.XPath("//button[contains(text(),'Ir a la modificacion')]");

        public SelectHerramientasParaReparar_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void BuscarHerramienta(string nombre = "", string tiempo = "")
        {
            WaitForBeingVisible(_inputNombre);
            
            if (!string.IsNullOrEmpty(nombre))
            {
                _driver.FindElement(_inputNombre).Clear();
                _driver.FindElement(_inputNombre).SendKeys(nombre);
            }

            if (!string.IsNullOrEmpty(tiempo))
            {
                _driver.FindElement(_inputTiempo).Clear();
                _driver.FindElement(_inputTiempo).SendKeys(tiempo);
            }

            _driver.FindElement(_btnBuscar).Click();
            // Esperamos un poco a que refresque la tabla
            System.Threading.Thread.Sleep(500); 
        }

        public void AgregarHerramienta(string nombreHerramienta)
        {
            // ID dinámico: btnAdd_Martillo
            By btnAddLocator = By.Id($"btnAdd_{nombreHerramienta}");
            
            WaitForBeingClickable(btnAddLocator);
            _driver.FindElement(btnAddLocator).Click();
        }

        public void PulsarContinuar()
        {
            WaitForBeingClickable(_btnContinuar);
            _driver.FindElement(_btnContinuar).Click();
        }
    }
}