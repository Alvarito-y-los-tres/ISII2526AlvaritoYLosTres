using AppForSEII2526.UIT.Shared;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_OfertaHerramientas
{
    public class CUCrearOferta_UIT : UC_UIT
    {
        private const string fabricante = "Ferreteria Lopez";
        private const string nombreHerramienta = "Martillo";
        private const string materialHerramienta = "Madera";
        private const double precioHerramienta = 5;
        private const int metodoPago = 1; // Tarjeta
        private const int dirigidaA = 0; // Clientes
        private const string nombreUsuario = "Álvaro";
        

        private readonly SelectHerramientasParaOfertar_PO _selectPO;
        private readonly DetalleOferta_PO _detallePO;
        private readonly CrearOferta_PO _crearPO;


        public CUCrearOferta_UIT(ITestOutputHelper output) : base(output)
        {
            Initial_step_opening_the_web_page();

            _selectPO = new SelectHerramientasParaOfertar_PO(_driver, output);
            _detallePO = new DetalleOferta_PO(_driver, output);
            _crearPO = new CrearOferta_PO(_driver, output);
        }

        private void PasosInicialesParaCrearOferta_UIT()
        {
            _driver.Navigate().GoToUrl(_URI + "oferta/selectherramientaparaoferta");
        }


        // Flujo Básico - Éxito
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_1_CrearOferta_FlujoBasico_Exito()
        {
            // Arrange
            var fechaInicio = DateTime.Today.AddDays(1);
            var fechaFin = DateTime.Today.AddDays(20);
            var fechaOferta = DateTime.Today;
            int porcentajeDescuento = 20;

            // Act
            PasosInicialesParaCrearOferta_UIT();

            // Buscar y Seleccionar la herramienta
            _selectPO.BuscarHerramientas(fabricante);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta);
            _selectPO.ContinuarConOferta();

            // Rellenar datos necesarios
            _crearPO.RellenarDatos(fechaInicio, fechaFin, metodoPago, nombreUsuario , dirigidaA);
            _crearPO.EstablecerPorcentaje(nombreHerramienta, porcentajeDescuento);

            // Confirmar
            _crearPO.PulsarCrearOferta();
            _crearPO.ConfirmarModal();

            // Assert
            var detalleEsperado = new List<string[]>
            {
                new string[] {
                    nombreHerramienta,
                    materialHerramienta,
                    fabricante,
                    precioHerramienta.ToString() + ",00 €",
                    porcentajeDescuento.ToString() + " %",
                    precioHerramienta*(1 - porcentajeDescuento / 100.0) + ",00 €"
                }
            };

            Assert.True(_detallePO.CheckOfertaCreadaCorrectamente(detalleEsperado),
                "Los detalles de la oferta creada no coinciden con los datos ingresados.");
        }


        // Porcentajes inválidos
        [Theory]
        [Trait("LevelTesting", "Funcional Testing")]
        [InlineData(120)] 
        [InlineData(-20)] 
        public void UC3_2_CrearOferta_PorcentajeInvalido_Error(int porcentajeError)
        {
            // Act
            PasosInicialesParaCrearOferta_UIT();
            _selectPO.BuscarHerramientas(fabricante);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta);
            _selectPO.ContinuarConOferta();

            // Rellenamos datos válidos generales
            _crearPO.RellenarDatos(DateTime.Today.AddDays(1), DateTime.Today.AddDays(10), metodoPago, nombreUsuario, dirigidaA);

            // Introducimos el porcentaje erróneo
            _crearPO.EstablecerPorcentaje(nombreHerramienta, porcentajeError);

            // Intentamos guardar
            _crearPO.PulsarCrearOferta();
            try { _crearPO.ConfirmarModal(); } catch { }

            // Assert
            bool seguimosEnCrear = _driver.Url.Contains("/oferta/crearoferta");
            Assert.True(seguimosEnCrear, $"El sistema permitió crear oferta con porcentaje {porcentajeError}%");

        }


        // Fechas inválidas
        public static IEnumerable<object[]> CasosPruebaPara_FechasInvalidas()
        {
            var allTests = new List<object[]>
            {
                //Inicio anterior a hoy
                new object[] { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(20), "La fecha de inicio debe ser futura" },
                
                // Fin anterior a inicio
                new object[] { DateTime.Today.AddDays(1), DateTime.Today, "La fecha de fin debe ser posterior a la fecha de inicio" },
                
                // Duración < 7 días
                new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(5), "¡Error!, la oferta debe durar al menos una semana" }
            };
            return allTests;
        }
        [Theory]
        [MemberData(nameof(CasosPruebaPara_FechasInvalidas))]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_FechasInvalidas_Error(DateTime fechaInicio, DateTime fechaFin, string error)
        {
            // Act
            PasosInicialesParaCrearOferta_UIT();
            _selectPO.BuscarHerramientas(fabricante);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta);
            _selectPO.ContinuarConOferta();

            // Rellenar formulario con fechas inválidas
            _crearPO.RellenarDatos(fechaInicio, fechaFin, metodoPago, nombreUsuario, dirigidaA);

            _crearPO.PulsarCrearOferta();
            try
            { _crearPO.ConfirmarModal(); }catch { }

            // Assert
            Assert.True(_crearPO.CheckErroresMensaje(error),
                $"Fallo con la validación de fechas. Error: {error}");
        }

        // Usuario inválido
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_6_UsuarioNoExistente_Error()
        {
            // Act
            PasosInicialesParaCrearOferta_UIT();
            _selectPO.BuscarHerramientas(fabricante);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta);
            _selectPO.ContinuarConOferta();

            _crearPO.RellenarDatos(DateTime.Today.AddDays(1), DateTime.Today.AddDays(20), metodoPago, "Roberto", dirigidaA);

            _crearPO.PulsarCrearOferta();
            try { _crearPO.ConfirmarModal(); } catch {}

            // Assert
            bool hayError = _crearPO.CheckErroresMensaje("no existe") || _crearPO.CheckErroresMensaje("Error");
            Assert.True(hayError, "El sistema no mostró error al usar un usuario inexistente.");
        }

        // Carrito vacío
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_2_CarritoVacio_Error()
        {
            // Act
            PasosInicialesParaCrearOferta_UIT();

            // Intentar pulsar continuar sin añadir nada
            try
            {
                _selectPO.ContinuarConOferta();
            }
            catch (Exception) {}

            // Assert
            bool seguimosEnSeleccion = _driver.Url.Contains("oferta/selectherramientaparaoferta");
            Assert.True(seguimosEnSeleccion, "El sistema permitió continuar con el carrito vacío.");
        }


        // Borrar herramienta del carrito
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_7_BorrarHerramientaCarrito()
        {
            //Act
            PasosInicialesParaCrearOferta_UIT();
            _selectPO.BuscarHerramientas(fabricante);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta);
            _selectPO.ContinuarConOferta();
            _crearPO.ModificarHerramientas();
            _selectPO.EliminarHerramienta(nombreHerramienta);

            //Assert
            Assert.True(_selectPO.CheckCarritoVacio(),
                "Error: El carrito no está vacío tras borrar la herramienta.");

        }
    }
}