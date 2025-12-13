using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CU_ComprarHerramientas
{
    public class DetalleCompra_PO : PageObject
    {

        private By _labelNombreCompleto = By.Id("NameSurname");
        private By _labelDireccion = By.Id("DeliveryAddress");
        private By _labelPrecioTotal = By.Id("TotalPrice");
        private By _tablaItems = By.Id("Herramientas");
        
        public DetalleCompra_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        private By _tableOfItems = By.Id("ComprasHerramientas");

        public bool CheckDetalleCompra(string nombre, string apellido, string direccion, DateTime fecha, int precioTotal)
        {
            WaitForBeingVisible(_tableOfItems);

            bool resultado = true;
            var nombreYApellido = nombre + " " + apellido;
            resultado = resultado && _driver.FindElement(By.Id("NombreApellido")).Text.Contains(nombreYApellido);
            resultado = resultado && _driver.FindElement(By.Id("DireccionCompra")).Text.Contains(direccion);
            resultado = resultado && _driver.FindElement(By.Id("FechaCompra")).Text.Contains(fecha.ToString("dd/MM/yyyy"));
            resultado = resultado && _driver.FindElement(By.Id("PrecioTotal")).Text.Contains(precioTotal.ToString());
            return resultado;
        }

        public bool CheckListHerramienta(List<string[]> expectedHerramientas)
        {
            return CheckBodyTable(expectedHerramientas, _tableOfItems);
        }
        

       
    }
}

