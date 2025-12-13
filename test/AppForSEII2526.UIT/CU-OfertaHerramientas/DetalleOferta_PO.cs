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
        private By detallesOfertaTable = By.Id("HerramientasEnOferta");

        public DetalleOferta_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public bool CheckOfertaCreadaCorrectamente(List<string[]> expectedDetalles)
        {
            return CheckBodyTable(expectedDetalles, detallesOfertaTable);
        }
    }
}
