using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using AppForSEII2526.UIT.Shared;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_AlquilarHerramienta
{
    public class DetalleAlquiler_PO : PageObject
    {
        public DetalleAlquiler_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }
        public bool CheckDetalleAlquiler(List<string[]> expectedDetails)
        {
            return CheckBodyTable(expectedDetails, By.Id("HerramientasAlquiladas") );
        }
    }
}