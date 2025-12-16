using AppForSEII2526.UIT.Shared;
using Xunit;
using Xunit.Abstractions;
using System;
using System.Runtime.InteropServices;

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


        // --- FLUJO BÁSICO ---, sin telefono, ni descripcion de herramientas
        // para comprobar que el telefono y la descripcion son opcionales verdaderamente 
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
            _crearPO.RellenarDetalleItem(herramienta, 2, "");
            _crearPO.RellenarDatosCliente(nombreUser, apellidoUser, DateTime.Today.AddDays(5), "", "1"); 
            

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            _crearPO.EsperarNavegacionADetalle();

            // 3. Comprobamos que todo se ha guardado bien en el detalle
            Assert.Contains("DetalleReparacion", _driver.Url);
            Assert.Equal(nombreUser+" "+apellidoUser, _detallePO.GetNombreCliente());
            Assert.Equal("5€", _detallePO.GetPrecioTotal());
            Assert.Equal(DateTime.Today.AddDays(5).ToString("dd/MM/yyyy"), _detallePO.GetFechaEntrega());
            Assert.Equal(DateTime.Today.AddDays(6).ToString("dd/MM/yyyy"), _detallePO.GetFechaRecogida());
            // Validamos que el item salga en la tabla final con su precio
            Assert.True(_detallePO.ValidarItemEnTabla(herramienta, 2, "5€"), "El item no aparece en la tabla de detalle");
        }


        // --- THEORY: FILTRADO COMPLETO (NOMBRE Y/O TIEMPO) ---
        // Aquí probamos el Flujo Alternativo 0 al paso 2 
        [Theory(DisplayName = "UC2.AF0 Filtrar herramientas por Nombre y Tiempo")]
        [Trait("Category", "UIT")]
        // Solo Nombre
        [InlineData("Martillo", "", true)]       // Existe
        [InlineData("CosaRara", "", false)]      // No existe
        [InlineData("", "5", true)]              // Existe reparación de menos de 5 días
        [InlineData("", "0", false)]           // No existe nada tan corto
        // Combinado: Nombre y Tiempo
        [InlineData("Martillo", "5", true)]      // Coinciden ambos
        [InlineData("Martillo", "0", false)]   // Nombre ok, pero tiempo mal
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
        // Agrupamos errores que saltan al validar datos , flujo alternativo 1 al paso 5 (fecha anterior a hoy)
        // y modificacion del sprint 2 (telefono sin prefijo +34)
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


        //modificar carrito flujo alternativo 2 al paso 5
        [Fact(DisplayName = "UC2.AF6 Modificar Carrito desde Crear Reparación")]
        [Trait("Category", "UIT")]
        public void UC2_AF6_ModificarCarrito_DesdeCrearReparacion()
        {
            string herramienta = "Martillo";
            string herramienta2 = "Taladro";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.BuscarHerramienta(herramienta2);
            _selectPO.AgregarHerramienta(herramienta2);
            _selectPO.PulsarContinuar();

            // Desde la página de Crear Reparación, pulsamos Modificar
            _crearPO.PulsarModificar();

            // Volvemos a la página de selección
            Assert.Contains("SelectItemReparacion", _driver.Url);

            int resultados = _selectPO.ContarResultadosEnTabla();
            Assert.True(resultados == 2, "Deberíamos ver el martillo y el taladro");

            _selectPO.BorrarDelCarrito(herramienta);
            _selectPO.PulsarContinuar();
            // Volvemos a Crear Reparación y comprobamos que solo queda el taladro
            Assert.Contains("CrearReparacion", _driver.Url);
            _crearPO.RellenarDatosCliente("Martín", "Álvarez", DateTime.Today.AddDays(2), "+34600000000", "1");
            _crearPO.RellenarDetalleItem(herramienta2, 1, "desc");
            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();
            _crearPO.EsperarNavegacionADetalle();
            Assert.Contains("DetalleReparacion", _driver.Url);
            Assert.True(_detallePO.ValidarItemEnTabla(herramienta2, 1, "30€"), "Debería crearse la reparación solo con el taladro");
            Assert.False(_detallePO.ValidarItemEnTabla(herramienta, 1, "5€"), "El martillo no debería estar en la reparación");
        }


        // flujo alternativo 3 al paso 4
        [Fact(DisplayName = "UC2.AF2_AF3 Carrito: Botón continuar inactivo")]
        [Trait("Category", "UIT")]
        public void UC2_AF3_CarritoVacio()
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


        // --- THEORY: CAMPOS OBLIGATORIOS VACÍOS ---
        // Probamos el Flujo Alternativo 4 al paso 6 (validación de campos obligatorios)
        [Theory(DisplayName = "UC2.AF4 Campos Obligatorios Vacíos")]
        [Trait("Category", "UIT")]
        [InlineData("", "ApellidoTest","1", "Name")]    // Me dejo el Nombre
        [InlineData("NombreTest", "", "1", "Surname")]   // Me dejo el Apellido
        [InlineData("NombreTest", "ApellidoTest", "", "PaymentMethod")] // Me dejo el metodo de pago
        public void UC2_AF4_DatosObligatorios_Theory(string nombre, string apellido,string metodo, string idCampoError)
        {
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Dejamos vacío el campo que toque probar
            _crearPO.RellenarDatosCliente(nombre, apellido, DateTime.Today.AddDays(2), "+34600000000", metodo);
            _crearPO.RellenarDetalleItem(herramienta, 1, "desc");

            _crearPO.PulsarCrear();
            
            // Seguimos en la misma página porque no valida
            Assert.Contains("CrearReparacion", _driver.Url);
            
            // El input tiene que ponerse rojo (clase invalid)
            Assert.True(_crearPO.HayMensajesDeValidacionCampo(idCampoError), $"El campo {idCampoError} debería estar en rojo");
        }


        // flujo alternativo 5 al paso 6 (cantidad 0 de algun item)
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
            _crearPO.RellenarDatosCliente("Martín", "Álvarez", DateTime.Today.AddDays(4), "+34600222333", metodoPagoValue); 
            _crearPO.RellenarDetalleItem(herramienta, 1, "Pago test");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();
            _crearPO.EsperarNavegacionADetalle();

            Assert.Contains("DetalleReparacion", _driver.Url);
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

    }
}