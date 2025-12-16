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

            // 1. Vamos a la selección
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // 2. Rellenamos el formulario con datos válidos
            // Ojo: Telefono con +34 por validación del controller
            _crearPO.RellenarDatosCliente(nombreUser, apellidoUser, DateTime.Today.AddDays(5), "+34666777888", "1"); 
            _crearPO.RellenarDetalleItem(herramienta, 2, "Mango roto");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            _crearPO.EsperarNavegacionADetalle();

            // 3. Comprobamos que todo se ha guardado bien en el detalle
            Assert.Contains("DetalleReparacion", _driver.Url);
            Assert.Contains(nombreUser, _detallePO.GetNombreCliente());
            // Validamos que el item salga en la tabla final con su precio
            Assert.True(_detallePO.ValidarItemEnTabla(herramienta, 2, "€"), "El item no aparece en la tabla de detalle");
        }

        // --- THEORY: FILTRADO COMPLETO (NOMBRE Y/O TIEMPO) ---
        // Aquí probamos el Flujo Alternativo 0 dándole caña a los filtros
        [Theory(DisplayName = "UC2.AF0 Filtrar herramientas por Nombre y Tiempo")]
        [Trait("Category", "UIT")]
        // Solo Nombre
        [InlineData("Martillo", "", true)]       // Existe
        [InlineData("CosaRara", "", false)]      // No existe
        // Solo Tiempo (asumiendo que tenemos items con estos tiempos en la BD)
        [InlineData("", "5", true)]              // Existe reparación de 5 días
        [InlineData("", "999", false)]           // No existe nada tan largo
        // Combinado: Nombre y Tiempo
        [InlineData("Martillo", "5", true)]      // Coinciden ambos
        [InlineData("Martillo", "999", false)]   // Nombre ok, pero tiempo mal
        public void UC2_AF0_FiltrarHerramientas_Completo_Theory(string nombre, string tiempo, bool debeEncontrarResultados)
        {
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");

            // Usamos el método del PO que acepta los dos filtros
            _selectPO.BuscarHerramienta(nombre, tiempo);
            
            int resultados = _selectPO.ContarResultadosEnTabla();

            if (debeEncontrarResultados)
            {
                Assert.True(resultados > 0, $"Se esperaban resultados para Nombre='{nombre}' y Tiempo='{tiempo}', pero no salió nada.");
            }
            else
            {
                Assert.Equal(0, resultados);
            }
        }

        // --- THEORY: ERRORES DE LÓGICA DE NEGOCIO ---
        // Agrupamos errores que saltan al validar datos (Flujo Alt 1 y validaciones extra)
        [Theory(DisplayName = "UC2.Validaciones Errores de Negocio (Fecha/Teléfono)")]
        [Trait("Category", "UIT")]
        [InlineData(-1, "+34600000000", "fecha de entrega no puede ser en el pasado")] // Fecha ayer
        [InlineData(1, "666777888", "debe empezar por +34")]                           // Teléfono sin prefijo
        public void UC2_Validaciones_Negocio_Theory(int diasFecha, string telefono, string mensajeErrorEsperado)
        {
            string herramienta = "Taladro"; 
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            
            // Preparamos el carrito para llegar al formulario
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Rellenamos datos intentando colar el error
            _crearPO.RellenarDatosCliente("Test", "User", DateTime.Today.AddDays(diasFecha), telefono, "1"); 
            _crearPO.RellenarDetalleItem(herramienta, 1, "Problema test");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            // Tiene que salir el error box con el mensaje
            bool hayError = _crearPO.CheckErrorMessage(mensajeErrorEsperado);
            Assert.True(hayError, $"Fallo: No apareció el mensaje de error '{mensajeErrorEsperado}'");
        }

        // --- THEORY: CAMPOS OBLIGATORIOS VACÍOS ---
        // Probamos el Flujo Alternativo 4 (validación de cliente en el front)
        [Theory(DisplayName = "UC2.AF4 Campos Obligatorios Vacíos")]
        [Trait("Category", "UIT")]
        [InlineData("", "ApellidoTest", "Name")]    // Me dejo el Nombre
        [InlineData("NombreTest", "", "Surname")]   // Me dejo el Apellido
        public void UC2_AF4_DatosObligatorios_Theory(string nombre, string apellido, string idCampoError)
        {
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Dejamos vacío el campo que toque probar
            _crearPO.RellenarDatosCliente(nombre, apellido, DateTime.Today.AddDays(2), "+34600000000", "1");
            _crearPO.RellenarDetalleItem(herramienta, 1, "desc");

            _crearPO.PulsarCrear();
            
            // Seguimos en la misma página porque no valida
            Assert.Contains("CrearReparacion", _driver.Url);
            
            // El input tiene que ponerse rojo (clase invalid)
            Assert.True(_crearPO.HayMensajesDeValidacionCampo(idCampoError), $"El campo {idCampoError} debería estar en rojo");
        }

        // --- THEORY: MÉTODOS DE PAGO ---
        // Probamos que el desplegable funcione con todas las opciones
        [Theory(DisplayName = "UC2.Extra Métodos de Pago Funcionan")]
        [Trait("Category", "UIT")]
        [InlineData("0")] // Tarjeta
        [InlineData("1")] // PayPal
        [InlineData("2")] // Efectivo
        public void UC2_Extra_MetodosPago_Theory(string metodoPagoValue)
        {
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Probamos el pago correspondiente
            _crearPO.RellenarDatosCliente("Pago", "Test", DateTime.Today.AddDays(4), "+34600222333", metodoPagoValue); 
            _crearPO.RellenarDetalleItem(herramienta, 1, "Pago test");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();
            _crearPO.EsperarNavegacionADetalle();

            Assert.Contains("DetalleReparacion", _driver.Url);
        }

        // --- TEST EXTRA: DESCRIPCIÓN OPCIONAL ---
        // Probamos que si no pongo descripción, también funciona (porque es opcional)
        [Fact(DisplayName = "UC2.Extra Descripción Vacía (Campo Opcional)")]
        [Trait("Category", "UIT")]
        public void UC2_Extra_DescripcionVacia_Funciona()
        {
            string herramienta = "Taladro"; 
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            _crearPO.RellenarDatosCliente("Luisa", "Gómez", DateTime.Today.AddDays(3), "+34600111222", "0"); 
            // Pasamos string vacío en la descripción
            _crearPO.RellenarDetalleItem(herramienta, 1, ""); 

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();
            _crearPO.EsperarNavegacionADetalle();

            Assert.Contains("DetalleReparacion", _driver.Url);
            // El item debe estar aunque no tenga descripción
            Assert.True(_detallePO.ValidarItemEnTabla(herramienta, 1, "€"), "Debería crearse la reparación sin descripción");
        }

        // --- TEST EXTRA: USUARIO NO EXISTE ---
        // Comprobamos que el sistema no trague usuarios fantasma
        [Fact(DisplayName = "UC2.Extra Usuario No Registrado da Error")]
        [Trait("Category", "UIT")]
        public void UC2_Extra_UsuarioNoExiste_Error()
        {
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Ponemos un usuario inventado
            _crearPO.RellenarDatosCliente("Usuario", "Inventado123", DateTime.Today.AddDays(2), "+34600000000", "1");
            _crearPO.RellenarDetalleItem(herramienta, 1, "User fake");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            // Debería salir error de usuario no encontrado
            bool hayError = _crearPO.CheckErrorMessage("no existe") || _crearPO.CheckErrorMessage("no encontrado");
            Assert.True(hayError, "El sistema debería quejarse si el usuario no existe");
        }

        // --- FLUJO ALTERNATIVO 2 y 3: GESTIÓN DEL CARRITO ---
        [Fact(DisplayName = "UC2.AF2_AF3 Carrito: Botón continuar y Borrar Item")]
        [Trait("Category", "UIT")]
        public void UC2_AF2_AF3_BorrarItem_Y_CarritoVacio()
        {
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");

            // Al principio está vacío, el botón no debería estar
            Assert.False(_selectPO.EsVisibleBotonContinuar(), "El botón continuar no debería verse con carrito vacío");

            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            
            // Ahora sí
            Assert.True(_selectPO.EsVisibleBotonContinuar(), "El botón continuar debería verse tras añadir item");

            // Borramos el item (AF2)
            _selectPO.BorrarDelCarrito(herramienta);

            // Debería volver a ocultarse
            Assert.False(_selectPO.EsVisibleBotonContinuar(), "El botón continuar debería ocultarse tras vaciar carrito");
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

            Assert.True(_crearPO.EstaBotonCrearDeshabilitado(), "El botón Crear debería estar deshabilitado si la cantidad es 0");
        }
    }
}