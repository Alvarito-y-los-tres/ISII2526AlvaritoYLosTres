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
        private readonly DetalleReparacion_PO _detallePO; 

        public CU_Reparacion_UIT(ITestOutputHelper output) : base(output)
        {
            _selectPO = new SelectHerramientasParaReparar_PO(_driver, output);
            _crearPO = new CrearReparacion_PO(_driver, output);
            _detallePO = new DetalleReparacion_PO(_driver, output);
        }

        // --- FLUJO BÁSICO ---
        [Fact(DisplayName = "UC2.1 Flujo Básico: Crear Reparación Correcta")]
        [Trait("Category", "UIT")]
        public void UC2_1_CrearReparacion_CaminoFeliz_OK()
        {
            string herramienta = "Martillo"; 
            string nombreUser = "Martín"; 
            string apellidoUser = "Álvarez";

            // 1. Selección
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // 2. Formulario
            // Importante: Telefono con +34 por validación en Controller 
            _crearPO.RellenarDatosCliente(nombreUser, apellidoUser, DateTime.Today.AddDays(5), "+34666777888", "1"); 
            _crearPO.RellenarDetalleItem(herramienta, 2, "Mango roto");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            // 3. Verificación en Detalle
            Assert.Contains("DetalleReparacion", _driver.Url);
            Assert.Contains(nombreUser, _detallePO.GetNombreCliente());
            // Validamos que el item salga en la tabla final
            Assert.True(_detallePO.ValidarItemEnTabla(herramienta, 2, "€"), "El item no aparece en la tabla de detalle");
        }

        // --- FLUJO ALTERNATIVO 0: FILTRADO ---
        [Fact(DisplayName = "UC2.AF0 Filtrar herramientas")]
        [Trait("Category", "UIT")]
        public void UC2_AF0_FiltrarHerramientas_Funciona()
        {
            // Navegar
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");

            // 1. Filtrar por algo que no exista (ej. "XyzW")
            _selectPO.BuscarHerramienta("XyzWImposible");
            Assert.Equal(0, _selectPO.ContarResultadosEnTabla());

            // 2. Filtrar por algo que exista (ej. "Martillo")
            _selectPO.BuscarHerramienta("Martillo");
            Assert.True(_selectPO.ContarResultadosEnTabla() > 0, "Debería aparecer al menos un martillo");
        }

        // --- FLUJO ALTERNATIVO 1: FECHA PASADA ---
        [Fact(DisplayName = "UC2.AF1 Error fecha entrega pasada")]
        [Trait("Category", "UIT")]
        public void UC2_AF1_FechaPasada_MuestraError()
        {
            string herramienta = "Taladro"; 
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Fecha Ayer 
            _crearPO.RellenarDatosCliente("Martín", "Álvarez", DateTime.Today.AddDays(-1), "+34600000000", "2"); 
            _crearPO.RellenarDetalleItem(herramienta, 1, "Problema motor");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            // Verificar error
            bool hayError = _crearPO.CheckErrorMessage("fecha de entrega no puede ser en el pasado");
            Assert.True(hayError, "No apareció el mensaje de error de fecha pasada.");
        }

        // --- FLUJO ALTERNATIVO 2 y 3: CARRITO VACÍO / BORRAR ---
        [Fact(DisplayName = "UC2.AF2_AF3 Modificar carrito y Carrito Vacio")]
        [Trait("Category", "UIT")]
        public void UC2_AF2_AF3_BorrarItem_Y_CarritoVacio()
        {
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");

            // AF3: Al entrar, el carrito está vacío, el botón continuar NO debe verse
            Assert.False(_selectPO.EsVisibleBotonContinuar(), "El botón continuar no debería verse con carrito vacío [cite: 63]");

            // Añadir item
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            
            // Ahora sí debe verse
            Assert.True(_selectPO.EsVisibleBotonContinuar(), "El botón continuar debería verse tras añadir item");

            // AF2: Borrar item
            _selectPO.BorrarDelCarrito(herramienta);

            // Verificar que volvemos al estado vacío (botón oculto)
            Assert.False(_selectPO.EsVisibleBotonContinuar(), "El botón continuar debería ocultarse tras vaciar carrito");
        }

        // --- FLUJO ALTERNATIVO 4: CAMPOS OBLIGATORIOS (CLIENTE) ---
        [Fact(DisplayName = "UC2.AF4 Campos Obligatorios Cliente Vacíos")]
        [Trait("Category", "UIT")]
        public void UC2_AF4_DatosObligatorios_Faltan()
        {
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Dejamos Nombre vacío
            _crearPO.RellenarDatosCliente("", "ApellidoTest", DateTime.Today.AddDays(2), "+34600000", "1");
            _crearPO.RellenarDetalleItem(herramienta, 1, "desc");

            // Intentamos crear (Click submit)
            _crearPO.PulsarCrear();
            // El modal NO debería aparecer si hay validación de cliente (DataAnnotation required)
            // O si aparece, al confirmar no avanza.
            
            // Verificamos que seguimos en la misma página (URL contiene CrearReparacion)
            Assert.Contains("CrearReparacion", _driver.Url);
            
            // Verificamos que el input Nombre tiene clase invalid (Validación Blazor)
            Assert.True(_crearPO.HayMensajesDeValidacionCampo("Name"), "El campo Nombre debería estar marcado como inválido");
        }

        // --- FLUJO ALTERNATIVO 5: CANTIDAD 0 ---
        [Fact(DisplayName = "UC2.AF5 Cantidad a reparar es 0")]
        [Trait("Category", "UIT")]
        public void UC2_AF5_CantidadCero_DeshabilitaBoton()
        {
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            _crearPO.RellenarDatosCliente("Test", "User", DateTime.Today.AddDays(1), "+34600000", "1");

            // Ponemos cantidad 0
            _crearPO.RellenarDetalleItem(herramienta, 0, "Test");

            //  La lógica en Razor dice: items.Any(item => item.Cantidad <= 0) => disabled = true
            Assert.True(_crearPO.EstaBotonCrearDeshabilitado(), "El botón Crear debería estar deshabilitado si la cantidad es 0");
        }

        // --- TEST EXTRA: VALIDACIÓN TELEFONO ---
        [Fact(DisplayName = "UC2.Extra Teléfono Incorrecto")]
        [Trait("Category", "UIT")]
        public void UC2_Extra_TelefonoFormatoIncorrecto()
        {
            // El controller tiene una validación específica: !dto.Telefono.StartsWith("+34")
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Teléfono sin prefijo internacional
            _crearPO.RellenarDatosCliente("Test", "User", DateTime.Today.AddDays(1), "666777888", "1");
            _crearPO.RellenarDetalleItem(herramienta, 1, "Test");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            // Debería salir el mensaje de error del backend
            bool hayError = _crearPO.CheckErrorMessage("debe empezar por +34");
            Assert.True(hayError, "Debería mostrar error si el teléfono no empieza por +34");
        }
    }
}