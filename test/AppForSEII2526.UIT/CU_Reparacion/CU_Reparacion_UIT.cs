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
            _crearPO.RellenarDatosCliente(nombreUser, apellidoUser, DateTime.Today.AddDays(5), "+34666777888", "1"); 
            _crearPO.RellenarDetalleItem(herramienta, 2, "Mango roto");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            _crearPO.EsperarNavegacionADetalle();

            // 3. Verificación en Detalle
            Assert.Contains("DetalleReparacion", _driver.Url);
            Assert.Contains(nombreUser, _detallePO.GetNombreCliente());
            Assert.True(_detallePO.ValidarItemEnTabla(herramienta, 2, "€"), "El item no aparece en la tabla de detalle");
        }

        // --- THEORY: FILTRADO DE HERRAMIENTAS ---
        // Agrupa el Flujo Alternativo 0 (Filtrar) probando varios casos
        [Theory(DisplayName = "UC2.AF0 Filtrar herramientas (Theory)")]
        [Trait("Category", "UIT")]
        [InlineData("XyzWImposible", false)] // Caso: No existe
        [InlineData("Martillo", true)]      // Caso: Existe
        [InlineData("Taladro", true)]       // Caso: Existe otro
        public void UC2_AF0_FiltrarHerramientas_Theory(string terminoBusqueda, bool debeEncontrarResultados)
        {
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");

            _selectPO.BuscarHerramienta(terminoBusqueda);
            
            int resultados = _selectPO.ContarResultadosEnTabla();

            if (debeEncontrarResultados)
            {
                Assert.True(resultados > 0, $"Se esperaban resultados para '{terminoBusqueda}' pero se encontraron 0.");
            }
            else
            {
                Assert.Equal(0, resultados);
            }
        }

        // --- THEORY: ERRORES DE VALIDACIÓN DE NEGOCIO (Backend/Lógica) ---
        // Agrupa el Flujo Alternativo 1 (Fecha Pasada) y el Test Extra (Teléfono Incorrecto)
        // Se usa un 'int' para los días porque no se puede pasar DateTime en InlineData
        [Theory(DisplayName = "UC2.Validaciones Errores de Negocio (Theory)")]
        [Trait("Category", "UIT")]
        [InlineData(-1, "+34600000000", "fecha de entrega no puede ser en el pasado")] // Fecha incorrecta
        [InlineData(1, "666777888", "debe empezar por +34")]                           // Teléfono incorrecto
        public void UC2_Validaciones_Negocio_Theory(int diasFecha, string telefono, string mensajeErrorEsperado)
        {
            string herramienta = "Taladro"; 
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            
            // Precondición: Añadir algo al carrito
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Rellenar formulario con los datos parametrizados
            _crearPO.RellenarDatosCliente("Test", "User", DateTime.Today.AddDays(diasFecha), telefono, "1"); 
            _crearPO.RellenarDetalleItem(herramienta, 1, "Problema test");

            _crearPO.PulsarCrear();
            _crearPO.ConfirmarModal();

            // Verificar que aparece el mensaje de error específico
            // Utilizamos el método CheckErrorMessage definido en tu PO
            bool hayError = _crearPO.CheckErrorMessage(mensajeErrorEsperado);
            Assert.True(hayError, $"No apareció el mensaje de error esperado: '{mensajeErrorEsperado}'");
        }

        // --- THEORY: CAMPOS OBLIGATORIOS (Validación Cliente/CSS) ---
        // Agrupa el Flujo Alternativo 4 probando que falte Nombre o Apellido
        [Theory(DisplayName = "UC2.AF4 Campos Obligatorios Vacíos (Theory)")]
        [Trait("Category", "UIT")]
        [InlineData("", "ApellidoTest", "Name")]    // Falta Nombre -> Error en campo 'Name'
        [InlineData("NombreTest", "", "Surname")]   // Falta Apellido -> Error en campo 'Surname'
        public void UC2_AF4_DatosObligatorios_Theory(string nombre, string apellido, string idCampoError)
        {
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");
            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            _selectPO.PulsarContinuar();

            // Rellenamos con datos vacíos según el caso
            _crearPO.RellenarDatosCliente(nombre, apellido, DateTime.Today.AddDays(2), "+34600000000", "1");
            _crearPO.RellenarDetalleItem(herramienta, 1, "desc");

            _crearPO.PulsarCrear();
            
            // Verificamos que seguimos en la misma página
            Assert.Contains("CrearReparacion", _driver.Url);
            
            // Verificamos que el input específico tiene la clase invalid
            // Usamos tu método HayMensajesDeValidacionCampo
            Assert.True(_crearPO.HayMensajesDeValidacionCampo(idCampoError), $"El campo {idCampoError} debería estar marcado como inválido");
        }

        // --- FLUJO ALTERNATIVO 2 y 3: CARRITO VACÍO / BORRAR ---
        // Este se mantiene como Fact porque es una secuencia de pasos lógica específica
        [Fact(DisplayName = "UC2.AF2_AF3 Modificar carrito y Carrito Vacio")]
        [Trait("Category", "UIT")]
        public void UC2_AF2_AF3_BorrarItem_Y_CarritoVacio()
        {
            string herramienta = "Martillo";
            _driver.Navigate().GoToUrl(_URI + "Reparacion/SelectItemReparacion");

            // AF3: Carrito vacío inicial
            Assert.False(_selectPO.EsVisibleBotonContinuar(), "El botón continuar no debería verse con carrito vacío");

            _selectPO.BuscarHerramienta(herramienta);
            _selectPO.AgregarHerramienta(herramienta);
            
            Assert.True(_selectPO.EsVisibleBotonContinuar(), "El botón continuar debería verse tras añadir item");

            // AF2: Borrar item
            _selectPO.BorrarDelCarrito(herramienta);

            Assert.False(_selectPO.EsVisibleBotonContinuar(), "El botón continuar debería ocultarse tras vaciar carrito");
        }

        // --- FLUJO ALTERNATIVO 5: CANTIDAD 0 ---
        // Se mantiene como Fact, aunque podría ser Theory si hubiera más casos de lógica de botón deshabilitado
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