using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CU_ComprarHerramientas
{
    public class SelectHerramientasParaComprar_PO : PageObject
    {

        public SelectHerramientasParaComprar_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        By inputMaterial = By.Id("materialInput");
        By inputPrecio = By.Id("precioInput");
        By buttonSearchHerramientas = By.Id("searchHerramientasButton");
        By tablaDeHerramientas = By.Id("herramientasTable");
        By errorShownBy = By.Id("errorMessage");
        By botonComprarHerramienta = By.Id("comprarHerramientasButton");






        public void BuscarHerramientas(string material, string precio)
        {
            WaitForBeingVisible(inputMaterial);
            var mat = _driver.FindElement(inputMaterial);
            mat.Clear();
            mat.SendKeys(material);

            WaitForBeingVisible(inputPrecio);
            var pre = _driver.FindElement(inputPrecio);
            pre.Clear();
            pre.SendKeys(precio);

            _driver.FindElement(buttonSearchHerramientas).Click();

            
            WaitForBeingVisible(tablaDeHerramientas);
        }


        public bool CheckMessageError(string mensajeEsperado)
        {
            try
            {
                WaitForBeingVisible(errorShownBy);
                string textoActual = _driver.FindElement(errorShownBy).Text;
                return textoActual.Contains(mensajeEsperado);
            }
            catch (WebDriverTimeoutException)
            {
                return false; // No apareció el mensaje
            }
        }


        public void AñadirHerramientaToCart(string herramientaNombre)
        {
            By addButton = By.Id("herramientaToCompra_" + herramientaNombre);
            WaitForBeingClickable(addButton);
            _driver.FindElement(addButton).Click();
        }



     


        public void QuitarHerramientaFromCart(string herramientaNombre)
        {
            By removeButton = By.Id("herramientaToRemove_" + herramientaNombre);
            WaitForBeingClickable(removeButton);
            _driver.FindElement(removeButton).Click();
        }

        public void ComprarHerramientas()
        {
            WaitForBeingClickable(botonComprarHerramienta);
            _driver.FindElement(botonComprarHerramienta).Click();
        }

        public bool CompraNotAvailable()
        {
            try
            {
                return _driver.FindElement(botonComprarHerramienta).Displayed == false;
            }
            catch (Exception e)
            {
                return true;
            }
        }

        
        public bool CheckListOfHerramientasInCart(List<string[]> expectedHerramientas)
        {
           
            return CheckBodyTable(expectedHerramientas, tablaDeHerramientas);
        }
        

    }
}
