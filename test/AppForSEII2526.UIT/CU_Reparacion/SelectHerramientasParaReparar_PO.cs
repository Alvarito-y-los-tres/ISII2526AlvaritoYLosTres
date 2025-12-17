using OpenQA.Selenium;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;
using System;
using System.Threading;

namespace AppForSEII2526.UIT.CU_Reparacion
{
    public class SelectHerramientasParaReparar_PO : PageObject
    {
        
        private By _inputNombre = By.Id("filtroNombre");
        private By _inputTiempo = By.Id("filtroTiempo");
        private By _btnBuscar = By.Id("searchitems");

        // El botón de continuar
        private By _btnContinuar = By.Id("continuar");

        public SelectHerramientasParaReparar_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void BuscarHerramienta(string nombre = "", string tiempo = "")
        {
            // Envolvemos las interacciones en AccionSegura para evitar StaleElementReferenceException
            if (nombre != null)
            {
                AccionSegura(() =>
                {
                    WaitForBeingClickable(_inputNombre);
                    var elem = _driver.FindElement(_inputNombre);
                    elem.Click();
                    elem.Clear();
                    elem.SendKeys(nombre);
                });
            }

            if (!string.IsNullOrEmpty(tiempo))
            {
                AccionSegura(() =>
                {
                    WaitForBeingClickable(_inputTiempo);
                    var elem = _driver.FindElement(_inputTiempo);
                    elem.Click();
                    elem.Clear();
                    elem.SendKeys(tiempo);
                });
            }

            AccionSegura(() =>
            {
                WaitForBeingClickable(_btnBuscar);
                _driver.FindElement(_btnBuscar).Click();
            });

            // Esperamos a que la tabla refresque sus datos tras el clic
            Thread.Sleep(1000);
        }

        public void AgregarHerramienta(string nombreHerramienta)
        {
            By btnAddLocator = By.Id($"btnAdd_{nombreHerramienta}");

            // También protegemos este clic, ya que la tabla se acaba de repintar
            AccionSegura(() =>
            {
                WaitForBeingClickable(btnAddLocator);
                _driver.FindElement(btnAddLocator).Click();
            });
        }

        public void PulsarContinuar()
        {
            AccionSegura(() =>
            {
                WaitForBeingClickable(_btnContinuar);
                _driver.FindElement(_btnContinuar).Click();
            });
        }

        // --- MÉTODO HELPER PARA REINTENTAR SI BLAZOR REFRESCA LA PÁGINA ---
        private void AccionSegura(Action accion)
        {
            int intentos = 0;
            while (intentos < 5) // Intentamos hasta 5 veces
            {
                try
                {
                    accion();
                    return; // Si funciona, salimos
                }
                catch (StaleElementReferenceException)
                {
                    // El elemento cambió (Blazor repintó), esperamos y reintentamos
                    intentos++;
                    Thread.Sleep(500);
                }
                catch (ElementClickInterceptedException)
                {
                    // Algo tapó el click, esperamos y reintentamos
                    intentos++;
                    Thread.Sleep(500);
                }
                catch (WebDriverTimeoutException)
                {
                    // Si es timeout, lanzamos el error porque no va a aparecer mágicamente
                    throw;
                }
            }
            // Si falló 5 veces, ejecutamos una última vez para que lance la excepción original
            accion();
        }
        // Añade estos métodos a tu clase SelectHerramientasParaReparar_PO existente

        public int ContarResultadosEnTabla()
        {
            //  La tabla tiene id "TableOfRepairs"
            try
            {
                // Esperamos un momento a que renderice
                System.Threading.Thread.Sleep(500); 
                var filas = _driver.FindElements(By.XPath("//table[@id='TableOfRepairs']/tbody/tr"));
                return filas.Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public void BorrarDelCarrito(string nombreHerramienta)
        {
            // El botón de borrar está dentro del foreach del carrito
            // Buscamos el botón rojo dentro del div del carrito
            By btnBorrar = By.XPath($"//div[contains(., '{nombreHerramienta}')]//button[contains(text(),'Borrar')]");
            
            WaitForBeingClickable(btnBorrar);
            _driver.FindElement(btnBorrar).Click();
            
            // Esperar a que Blazor actualice el DOM
            System.Threading.Thread.Sleep(500);
        }

        public bool EsVisibleBotonContinuar()
        {
            // Intentamos verificar la visibilidad con reintentos para evitar StaleElementReference
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var btn = _driver.FindElement(_btnContinuar);
                    return btn.Displayed && btn.Enabled;
                }
                catch (StaleElementReferenceException)
                {
                    // El DOM cambió justo mientras consultábamos. Esperamos y reintentamos.
                    System.Threading.Thread.Sleep(500);
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            }
            return false; // Si tras los intentos sigue fallando, asumimos que no está
        }

       

        public int ContarItemsEnCarrito()
        {
            try
            {
                // Esperamos un poco para asegurar que el DOM se ha actualizado tras la navegación
                System.Threading.Thread.Sleep(500);

                // XPath: Busca cualquier div con clase 'col-2' y dentro cuenta los botones de 'Borrar'
                // Es la forma más fiable de saber cuántos items hay agregados.
                var items = _driver.FindElements(By.XPath("//div[contains(@class, 'col-2')]//button[contains(text(),'Borrar')]"));
                return items.Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }
    }
}