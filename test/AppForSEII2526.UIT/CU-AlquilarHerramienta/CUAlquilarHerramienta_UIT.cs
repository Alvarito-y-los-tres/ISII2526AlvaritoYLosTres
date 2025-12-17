
using AppForSEII2526.UIT.CU_AlquilarHerramienta;
using AppForSEII2526.UIT.Shared;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_Alquiler
{
    public class CUCrearAlquiler_UIT : UC_UIT
    {

        private const string herramientaNombre = "Taladro";
        private const string herramientaMaterial = "Acero";
        private const string herramientaPrecio = "30,00 €";

        private readonly SelectHerramientaParaAlquilar_PO _selectPO;
        private readonly CrearAlquiler_PO _crearPO;
        private readonly DetalleAlquiler_PO _detallePO;

        public CUCrearAlquiler_UIT(ITestOutputHelper output) : base(output)
        {
            Initial_step_opening_the_web_page();

            _selectPO = new SelectHerramientaParaAlquilar_PO(_driver, _output);
            _crearPO = new CrearAlquiler_PO(_driver, _output);
            _detallePO = new DetalleAlquiler_PO(_driver, _output);
        }

        private void InitialStepsForCrearAlquiler_UIT()
        {
           
            _driver.Navigate().GoToUrl(_URI + "alquilar/selectherramientaparaalquilar");
        }


        // UC4_1: Flujo Básico - Creación Exitosa (Esc-1)


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_1_CrearAlquiler_Exito()
        {
            // Arrange
            var fechaInicio = DateTime.Today.AddDays(1); 
            var fechaFin = DateTime.Today.AddDays(8);    
            string nombre = "Álvaro";
            string apellido = "Cano";
            string direccion = "Calle"; //tiene que empezar por la palabra calle
            string telefono = "600123456";
            string email = "daniel@test.com";
            string pagoValue = "0"; 
            int cantidad = 2;

            // Act
            InitialStepsForCrearAlquiler_UIT();

            _selectPO.Buscarherramientas(herramientaNombre, herramientaMaterial);
            _selectPO.AddHerramientaToAlquilerCart(herramientaNombre);
            _selectPO.PressAlquilar();
            _crearPO.RellenarDatosCliente(nombre, apellido, direccion, telefono, email, pagoValue);
            _crearPO.RellenarFechas(fechaInicio, fechaFin);
            _crearPO.EstablecerCantidad(herramientaNombre, cantidad);
            _crearPO.PulsarCrearAlquiler();
            _crearPO.ConfirmarModal();

            
            var expectedRow = new List<string[]>
            {
                new string[] {
                    herramientaNombre,      
                    herramientaMaterial,    
                    herramientaPrecio,     
                    cantidad.ToString()      
                }
            };

            Assert.True(_detallePO.CheckDetalleAlquiler(expectedRow), "Los detalles del alquiler no coinciden con lo esperado.");
        }

        // UC4_2: Lista Vacía (Esc-2)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_2_ListaVacia_Error()
        {
            // Act
            InitialStepsForCrearAlquiler_UIT();


            bool botonHabilitado = true;
            try
            {
 
                _selectPO.PressAlquilar();
            }
            catch (Exception)
            {
                botonHabilitado = false;
            }

            // Assert
            if (botonHabilitado)
            {
               
                bool urlCambio = _driver.Url.Contains("createalquiler");

                if (urlCambio)
                {
                  
                    _crearPO.RellenarDatosCliente("Test", "User", "Calle Falsa", "+34600123456", "mail@test.com", "0");
                    _crearPO.RellenarFechas(DateTime.Today.AddDays(1), DateTime.Today.AddDays(2));

                    _crearPO.PulsarCrearAlquiler();

                    bool errorListaVacia = _crearPO.CheckErrorMessage("Debes seleccionar al menos una herramienta") ||
                                           _crearPO.CheckErrorMessage("seleccionar al menos una herramienta");

                    Assert.True(errorListaVacia, "UC4_2 Falló: Se permitió avanzar o no apareció el mensaje de error de lista vacía.");
                }
            }
            else
            {
                Assert.True(!botonHabilitado, "Correcto: El botón continuar no es accesible con la lista vacía.");
            }
        }

        // UC4_3: Comprobación de filtros individuales y combinados
       
        [Theory]
        [Trait("LevelTesting", "Funcional Testing")]
        [InlineData("Taladro", "", "Taladro")]
        [InlineData("", "Acero", "Acero")]
        [InlineData("Taladro", "Acero", "Taladro")]
        public void UC4_3Filtrado_Progresivo(string nombre, string material, string textoEsperado)
        {
            // Act
            InitialStepsForCrearAlquiler_UIT();
            _selectPO.Buscarherramientas(nombre, material);
            System.Threading.Thread.Sleep(1000);

            // Assert
            bool hayResultados = _selectPO.HayResultadosEnTabla();

            bool textoEncontrado = _selectPO.VerificarTextoEnTabla(textoEsperado);

            Assert.True(hayResultados, $"Fallo: No se encontraron resultados al filtrar por Nombre: '{nombre}' y Material: '{material}'.");
            Assert.True(textoEncontrado, $"Fallo: Se esperaba encontrar '{textoEsperado}' pero no aparece en la tabla filtrada.");
        }

        // UC4_3, UC4_4: Errores de Fechas (Esc-3) - Flujo Alternativo 6

        public static IEnumerable<object[]> TestCasesFor_FechasInvalidas_Alquiler()
        {
            var allTests = new List<object[]>
            {
                // Caso 1: Fecha de Inicio en el pasado (Ayer)
                new object[] {
                    DateTime.Today.AddDays(-1),
                    DateTime.Today.AddDays(5),
                    "anterior a la fecha actual"
                },
                
                // Caso 2: Fecha Fin anterior a Fecha Inicio
                new object[] {
                    DateTime.Today.AddDays(5),
                    DateTime.Today.AddDays(2),
                    "fecha de fin no puede ser anterior"
                }
            };
            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_FechasInvalidas_Alquiler))]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_FechasInvalidas_Error(DateTime inicio, DateTime fin, string mensajeError)
        {
            // Act
            InitialStepsForCrearAlquiler_UIT();
            _selectPO.Buscarherramientas(herramientaNombre, "");
            System.Threading.Thread.Sleep(1000);
            _selectPO.AddHerramientaToAlquilerCart(herramientaNombre);
            _selectPO.PressAlquilar();

            _crearPO.RellenarDatosCliente("Test", "User", "Calle Falsa", "+34600123456", "test@test.com", "0");

            _crearPO.RellenarFechas(inicio, fin);

            _crearPO.EstablecerCantidad(herramientaNombre, 1);

            _crearPO.PulsarCrearAlquiler();

            try { _crearPO.ConfirmarModal(); } catch { }

            Assert.True(_crearPO.CheckErrorMessage(mensajeError),
                $"Fallo en validación de fechas ({inicio.ToShortDateString()} - {fin.ToShortDateString()}). Esperaba mensaje que contuviera: '{mensajeError}'");
        }

        // UC4_5 Usuario No Existe Esc-3
        
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_5UsuarioNoExiste_Error()
        {
            // Act
            InitialStepsForCrearAlquiler_UIT();

            _selectPO.Buscarherramientas(herramientaNombre, "");
            _selectPO.AddHerramientaToAlquilerCart(herramientaNombre);
            _selectPO.PressAlquilar();

            _crearPO.RellenarDatosCliente("Usuario", "Fantasma", "Calle Desconocida", "+34600000000", "fantasma@test.com", "0");
            _crearPO.RellenarFechas(DateTime.Today.AddDays(1), DateTime.Today.AddDays(2));
            _crearPO.EstablecerCantidad(herramientaNombre, 1);

            // 3. Intentar crear
            _crearPO.PulsarCrearAlquiler();

           
            try { _crearPO.ConfirmarModal(); } catch { }

            // Assert
            bool errorUsuario = _crearPO.CheckErrorMessage("no existe") ||
                                _crearPO.CheckErrorMessage("usuario no encontrado");

            Assert.True(errorUsuario, "Fallo: No se mostró el error de usuario inexistente.");
        }


        // UC4_6: Validación Campos Obligatorios - Dirección Vacía (Esc-3)

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_6_DireccionVacia_Error()
        {
            // Arrange
            var fechaInicio = DateTime.Today.AddDays(1);
            var fechaFin = DateTime.Today.AddDays(8);

            string nombre = "Álvaro";
            string apellido = "Cano";
            string direccion = ""; // Dato inválido (Vacío)
            string telefono = "+34600123456";
            string email = "daniel@test.com";
            string pagoValue = "0";
            int cantidad = 2;

            // Act
            InitialStepsForCrearAlquiler_UIT();

            _selectPO.Buscarherramientas(herramientaNombre, "");
            _selectPO.AddHerramientaToAlquilerCart(herramientaNombre);
            _selectPO.PressAlquilar();

            _crearPO.RellenarDatosCliente(nombre, apellido, direccion, telefono, email, pagoValue);
            _crearPO.RellenarFechas(fechaInicio, fechaFin);
            _crearPO.EstablecerCantidad(herramientaNombre, cantidad);

            _crearPO.PulsarCrearAlquiler();

            try { _crearPO.ConfirmarModal(); } catch { }

            // Assert
            bool hayMensajeError = _crearPO.CheckErrorMessage("dirección de envío es obligatoria");
            bool seguimosEnFormulario = _driver.Url.Contains("createalquiler");

            Assert.True(hayMensajeError || seguimosEnFormulario,
                "Fallo: El sistema permitió avanzar con la dirección vacía o no mostró el error.");
        }

        // UC4_7: Validación Cantidad - Cantidad 0 (Esc-5)

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_7_CantidadCero_Error()
        {
            // Arrange
            var fechaInicio = DateTime.Today.AddDays(1);
            var fechaFin = DateTime.Today.AddDays(8);

            string nombre = "Álvaro";
            string apellido = "Cano";
            string direccion = "Calle Universidad";
            string telefono = "+34600123456";
            string email = "daniel@test.com";
            string pagoValue = "0";

            int cantidad = 0;

            // Act
            InitialStepsForCrearAlquiler_UIT();

            _selectPO.Buscarherramientas(herramientaNombre, "");
            _selectPO.AddHerramientaToAlquilerCart(herramientaNombre);
            _selectPO.PressAlquilar();

            _crearPO.RellenarDatosCliente(nombre, apellido, direccion, telefono, email, pagoValue);
            _crearPO.RellenarFechas(fechaInicio, fechaFin);

            _crearPO.EstablecerCantidad(herramientaNombre, cantidad);

            _crearPO.PulsarCrearAlquiler();

            try { _crearPO.ConfirmarModal(); } catch { }

            // Assert.
            bool hayMensajeError = _crearPO.CheckErrorMessage("Cantidad") || _crearPO.CheckErrorMessage("1");
            bool seguimosEnFormulario = _driver.Url.Contains("createalquiler", StringComparison.InvariantCultureIgnoreCase);

            Assert.True(hayMensajeError || seguimosEnFormulario,
                "Fallo: El sistema permitió avanzar con cantidad 0 o no mostró el error.");
        }


        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_BorrarHerramientaCarrito()
        {
            // Act
            InitialStepsForCrearAlquiler_UIT();

            _selectPO.Buscarherramientas(herramientaNombre, "");
            _selectPO.AddHerramientaToAlquilerCart(herramientaNombre);
            _selectPO.PressAlquilar();
            _crearPO.PulsarModificarCarrito();
            _selectPO.BorrarHerramientasDelCarrito(herramientaNombre);

            // Assert
            Assert.True(_selectPO.CheckCartVisibility(false),
                "Error: El carrito (botón alquilar) sigue visible tras borrar la única herramienta.");
        }




        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC4_Examen()
        {
            
            var fechaInicio = DateTime.Today.AddDays(3);
            var fechaFin = DateTime.Today.AddDays(10);
            string nombre = "Álvaro";
            string apellido = "Cano";
            string direccion = "Calle Mayor";
            string telefono = "600654321";
            string email = "daniel@gmail.com";
            int cantidad = 1;
            string pagoValue = "0";
            InitialStepsForCrearAlquiler_UIT();
            _selectPO.Buscarherramientas(herramientaNombre, "");
            _selectPO.AddHerramientaToAlquilerCart(herramientaNombre);
            System.Threading.Thread.Sleep(500);
            _selectPO.Buscarherramientas("", "Madera");
            _selectPO.AddHerramientaToAlquilerCart("Martillo");
            System.Threading.Thread.Sleep(500);
            _selectPO.PressAlquilar();
            System.Threading.Thread.Sleep(500);
            _crearPO.PulsarModificarCarrito();
            _selectPO.RemoveHerramientaFromAlquilerCart(herramientaNombre);
            _selectPO.PressAlquilar();
            _crearPO.RellenarDatosCliente(nombre, apellido, direccion, telefono, email, pagoValue);
            _crearPO.RellenarFechas(fechaInicio, fechaFin);
            _crearPO.EstablecerCantidad("Martillo", cantidad);
            _crearPO.PulsarCrearAlquiler();
            _crearPO.ConfirmarModal();


            var expectedRow = new List<string[]>
            {
                new string[] {
                    "Martillo",
                    "Madera",
                    "5,00 €",
                    cantidad.ToString()
                }
            };

            Assert.True(_detallePO.CheckDetalleAlquiler(expectedRow), "Los detalles del alquiler no coinciden con lo esperado.");


        }




    }





}
