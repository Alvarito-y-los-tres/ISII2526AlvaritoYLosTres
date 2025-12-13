using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.UIT.CU_ComprarHerramientas
{
    public class CrearCompra_PO : PageObject
    {

        public CrearCompra_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        private By _nombreCliente = By.CssSelector("input[name$='NombreCliente']");
        private By _apellidoCliente = By.CssSelector("input[name$='ApellidoCliente']");
        private By _direccionEnvio = By.CssSelector("input[name$='DireccionEnvio']");
        private By _correoElectronico = By.CssSelector("input[name$='CorreoElectronico']");
        private By _metodoPago = By.Id("TiposMetodoPago");


        

        private By _comprarHerramientas = By.Id("Submit");
        private By _dialogOk = By.Id("Button_DialogOK");

        private By _errorsShown = By.Id("ErrorsShown");
        private By _modificarCompra = By.Id("ModifyCompraItems");
        private By _tableOfItems = By.Id("TableOfRentalItems");

        private By _listaErroresValidacion = By.ClassName("validation-message"); // Errores campo a campo
        private By _resumenErrores = By.ClassName("validation-summary-errors"); // Resumen arriba
        private By _validationSummary = By.CssSelector(".validation-summary-errors, .text-danger, .alert-danger");

        private By _dialogModal = By.Id("DialogOKSaveDelete");

        public void ConfirmCompra()
        {
            WaitForBeingVisible(_dialogModal);
            WaitForBeingClickable(_dialogOk);
            _driver.FindElement(_dialogOk).Click();
        }


       

        public void EsperarPaginaCrearCompra()
        {
            
            var form = By.CssSelector("form"); 
            WaitForBeingVisible(form);
        }

        public void RellenarFormulario(string nombre, string apellido, string correo, string direccion, string metodoPagoId, string descripcion)
        {
            EsperarPaginaCrearCompra();
            WaitForBeingVisible(_nombreCliente);

            _driver.FindElement(_nombreCliente).Clear();
            _driver.FindElement(_nombreCliente).SendKeys(nombre);

            _driver.FindElement(_apellidoCliente).Clear();
            _driver.FindElement(_apellidoCliente).SendKeys(apellido);

            _driver.FindElement(_correoElectronico).Clear();
            _driver.FindElement(_correoElectronico).SendKeys(correo);

            _driver.FindElement(_direccionEnvio).Clear();
            _driver.FindElement(_direccionEnvio).SendKeys(direccion);


            WaitForBeingVisible(_tableOfItems);
            var filas = _driver.FindElements(By.CssSelector("#TableOfRentalItems tbody tr"));
            foreach (var fila in filas)
            {
                var descInput = fila.FindElement(By.CssSelector("td:last-child input"));
                descInput.Clear();
                descInput.SendKeys(descripcion);
            }



            var select = new SelectElement(_driver.FindElement(_metodoPago));
            try
            {
                select.SelectByValue(metodoPagoId); // por si viene "2"
            }
            catch (NoSuchElementException)
            {
                select.SelectByText(metodoPagoId);  // por si viene "PayPal"
            }


        }




        public void SubmitCompra()
        {
            WaitForBeingClickable(_comprarHerramientas);
            _driver.FindElement(_comprarHerramientas).Click();
        }

        

        public void ModificarCompra()
        {
            WaitForBeingClickable(_modificarCompra);
            _driver.FindElement(_modificarCompra).Click();
        }




        public bool CheckError(string expected)
        {
            var texts = new List<string>();

            texts.AddRange(_driver.FindElements(By.CssSelector(".validation-message"))
                .Select(e => e.Text));

            texts.AddRange(_driver.FindElements(By.CssSelector("form ul li"))
                .Select(e => e.Text));

            texts.AddRange(_driver.FindElements(By.CssSelector(".alert.alert-danger"))
                .Select(e => e.Text));

            var allText = string.Join(" | ", texts.Where(t => !string.IsNullOrWhiteSpace(t)));
            _output.WriteLine($"Errores encontrados: {allText}");

            return allText.Contains(expected, StringComparison.OrdinalIgnoreCase);
        }





        public bool CheckListHerramientasEnCompra(List<string[]> expectedCompraItems)
        {
            return CheckBodyTable(expectedCompraItems, _tableOfItems);
        }

       
        public void EstablecerCantidadPorNombre(string nombreHerramienta, string cantidad)
        {
            WaitForBeingVisible(_tableOfItems);

            var fila = _driver.FindElement(By.CssSelector($"tr#HerramientaData_{nombreHerramienta}"));
            var input = fila.FindElement(By.CssSelector("td:nth-child(3) input"));

            input.Click();
            input.SendKeys(Keys.Control + "a");
            input.SendKeys(Keys.Delete);

            input.SendKeys(cantidad);
            input.SendKeys(Keys.Tab);

            // DEBUG: confirma lo que queda en el input
            var actual = input.GetAttribute("value");
            _output.WriteLine($"Cantidad escrita='{cantidad}', value en input='{actual}'");
        }




    }



}

