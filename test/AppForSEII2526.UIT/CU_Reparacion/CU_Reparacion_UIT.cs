using AppForSEII2526.UIT.Shared;
using Xunit;
using Xunit.Abstractions;
using System;

namespace AppForSEII2526.UIT.CU_Reparacion
{
    public class CU_Reparacion_UIT : UC_UIT
    {
        private readonly SelectHerramientasParaReparar_PO _selectPO;
        private readonly CrearReparacion_PO _crearPO;

        public CU_Reparacion_UIT(ITestOutputHelper output) : base(output)
        {
            _selectPO = new SelectHerramientasParaReparar_PO(_driver, output);
            _crearPO = new CrearReparacion_PO(_driver, output);
        }

        [Fact]
        [Trait("Category", "UIT")]
        public void UC2_1_CrearReparacion_CaminoFeliz_OK()
        {
            // 1. ARRANGE
            // Asegúrate de usar una herramienta y usuario que EXISTAN en tu BD de pruebas
            string herramienta = "Martillo"; 
            string nombreUser = "Martín"; // Usuario que existe en tu seed/BD
            string apellidoUser = "Álvarez";

            // 2. ACT - Selección
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // 2. ACT - Formulario
            // Importante: Telefono con +34 para que no falle tu validación nueva
            _crearPO.RellenarDatosCliente(nombreUser, apellidoUser, DateTime.Today.AddDays(5), "+34666777888", "1"); // 1 = Paypal
            
            // Rellenar la cantidad (obligatoria) y descripción
            _crearPO.RellenarDetalleItem(herramienta, 2, "Mango roto");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            // 3. ASSERT
            // Esperamos redirección a DetalleReparacion
            System.Threading.Thread.Sleep(2000);
            bool urlCorrecta = _driver.Url.Contains("/Reparacion/DetalleReparacion");
            Assert.True(urlCorrecta, $"Fallo: No redirigió al detalle. URL: {_driver.Url}");
        }

        [Fact]
        [Trait("Category", "UIT")]
        public void UC2_2_FechaPasada_MuestraError()
        {
            // 1. ARRANGE
            string herramienta = "Taladro"; 

            // 2. ACT
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Ponemos fecha de AYER
            _crearPO.RellenarDatosCliente("Martín", "Álvarez", DateTime.Today.AddDays(-1), "", "2"); // 2 = Cash
            _crearPO.RellenarDetalleItem(herramienta, 1, "Problema motor");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            // 3. ASSERT
            // Verificamos que sale el mensaje que acabamos de arreglar en el backend/frontend
            bool hayError = _crearPO.CheckErrorMessage("fecha de entrega no puede ser en el pasado");
            
            Assert.True(hayError, "No apareció el mensaje de error de fecha pasada.");
            Assert.Contains("/Reparacion/CrearReparacion", _driver.Url); // Seguimos en la misma página
        }

        [Fact]
        [Trait("Category", "UIT")]
        public void UC2_3_UsuarioNoExiste_MuestraError()
        {
             // 1. ARRANGE
            string herramienta = "Alicates"; 

            // 2. ACT
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Usuario inventado
            _crearPO.RellenarDatosCliente("Usuario", "Inventado", DateTime.Today.AddDays(2), "", "0"); 
            _crearPO.RellenarDetalleItem(herramienta, 1, "Oxidado");

            _crearPO.PulsarCrear();
            try { _crearPO.ConfirmarModal(); } catch { }

            // 3. ASSERT
            bool hayError = _crearPO.CheckErrorMessage("no existe");
            Assert.True(hayError, "El sistema debería avisar de que el usuario no existe.");
        }
    }
}