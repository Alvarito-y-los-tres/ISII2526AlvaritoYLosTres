using AppForSEII2526.UIT.Shared;
using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_OfertaHerramientas
{
    public class CUCrearOferta_UIT : UC_UIT
    {
        private const string fabricante1 = "Ferreteria Lopez";
        private const string nombreHerramienta1 = "Martillo";
        private const string materialHerramienta1 = "Madera";
        private const double precioHerramienta1 = 5;

        private const string fabricante2 = "Ferreteria Ruiz";
        private const string nombreHerramienta2 = "Alicates";
        private const string materialHerramienta2 = "Hierro";
        private const double precioHerramienta2 = 4;

        private const int metodoPago = 1; // PayPal
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
        public static IEnumerable<object[]> DatosEscenariosEspecificos()
        {
            // Caso 1: Martillo | Tarjeta de Crédito | Sin Target
            yield return new object[]
            {
                nombreHerramienta1, materialHerramienta1, fabricante1, precioHerramienta1, 0, null                   
            };

            // Caso 2: Alicates | PapPal | Socios
            yield return new object[]
            {
                nombreHerramienta2, materialHerramienta2, fabricante2, precioHerramienta2, 1, 0                     
            };

            // Caso 3: Martillo | Cash | Clientes
            yield return new object[]
            {
                nombreHerramienta1, materialHerramienta1, fabricante1, precioHerramienta1, 2, 1                  
            };
        }
        [Theory]
        [MemberData(nameof(DatosEscenariosEspecificos))]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_1_FlujoBasico_Exito
            (string nombreHerramienta, string materialHerramienta, string fabricanteHerramienta, double precioHerramienta, int _metodoPago, int? _dirigidaA)
        {
            // Arrange
            var fechaInicio = DateTime.Today.AddDays(1);
            var fechaFin = DateTime.Today.AddDays(20);
            var fechaOferta = DateTime.Today;
            int porcentajeDescuento = 80;
            double precioFinal = Math.Round(precioHerramienta * (1 - (porcentajeDescuento / 100.0)), 2);

            // Act
            PasosInicialesParaCrearOferta_UIT();

            // Buscar y Seleccionar la herramienta
            _selectPO.BuscarHerramientas(fabricanteHerramienta, precioHerramienta);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta);
            _selectPO.ContinuarConOferta();

            // Rellenar datos necesarios
            _crearPO.RellenarDatos(fechaInicio, fechaFin, _metodoPago, nombreUsuario , _dirigidaA);
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
                    fabricanteHerramienta,
                    precioHerramienta.ToString("0.00") + " €",
                    porcentajeDescuento.ToString() + " %",
                    precioFinal.ToString("0.00") + " €"
                }
            };

            Assert.True(_detallePO.CheckOfertaCreadaCorrectamente(detalleEsperado),
                "Los detalles de la oferta creada no coinciden con los datos ingresados.");
        }


        // Filtros
        public static IEnumerable<object[]> Filtros()
        {
            yield return new object[]
            {
                fabricante1,
                precioHerramienta1,
                new List<string[]>
                {
                    new string[] { nombreHerramienta1, materialHerramienta1, fabricante1, precioHerramienta1.ToString(), "Añadir" }
                }
            };

            yield return new object[]
            {
                null,
                (double)10,
                new List<string[]>
                {
                    new string[] { nombreHerramienta1, materialHerramienta1, fabricante1, precioHerramienta1.ToString(), "Añadir" },
                    new string[] { nombreHerramienta2, materialHerramienta2, fabricante2, precioHerramienta2.ToString(), "Añadir" }
                }
            };

            yield return new object[]
            {
                fabricante1,
                null,
                new List<string[]>
                {
                    new string[] { nombreHerramienta1, materialHerramienta1, fabricante1, precioHerramienta1.ToString(), "Añadir" }
                }
            };
        }

        [Theory]
        [MemberData(nameof(Filtros))]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_2_FA0_Filtros(string? fabricante, double? precio, List<string[]> expectedHerramientas)
        {
            //Act
            PasosInicialesParaCrearOferta_UIT();
            _selectPO.BuscarHerramientas(fabricante, precio);
            //Assert
            Assert.True(_selectPO.CheckListaHerramientasCargadas(expectedHerramientas),
                "Error: La lista de herramientas filtradas no coincide con la esperada.");
        }


        // Carrito vacío
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_3_FA4_CarritoVacio_Error()
        {
            // Act
            PasosInicialesParaCrearOferta_UIT();

            // Intentar pulsar continuar sin añadir nada
            try
            {
                _selectPO.ContinuarConOferta();
            }
            catch (Exception) { }

            // Assert
            bool seguimosEnSeleccion = _driver.Url.Contains("oferta/selectherramientaparaoferta");
            Assert.True(seguimosEnSeleccion, "El sistema permitió continuar con el carrito vacío.");
        }


        // Fechas inválidas
        public static IEnumerable<object[]> FechasInvalidas()
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
        [MemberData(nameof(FechasInvalidas))]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_4_FA1_Fechas_Error(DateTime fechaInicio, DateTime fechaFin, string error)
        {
            // Act
            PasosInicialesParaCrearOferta_UIT();
            _selectPO.BuscarHerramientas(fabricante1, precioHerramienta1);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta1);
            _selectPO.ContinuarConOferta();

            // Rellenar formulario con fechas inválidas
            _crearPO.RellenarDatos(fechaInicio, fechaFin, metodoPago, nombreUsuario, dirigidaA);

            _crearPO.PulsarCrearOferta();
            try
            { _crearPO.ConfirmarModal(); }
            catch { }

            // Assert
            Assert.True(_crearPO.CheckErroresMensaje(error),
                $"Fallo con la validación de fechas. Error: {error}");
        }


        // Eliminar herramienta del carrito
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_5_FA2_EliminarHerramientaCarrito()
        {
            //Act
            PasosInicialesParaCrearOferta_UIT();
            _selectPO.BuscarHerramientas(fabricante1, precioHerramienta1);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta1);
            _selectPO.ContinuarConOferta();
            _crearPO.ModificarHerramientas();
            _selectPO.EliminarHerramienta(nombreHerramienta1);

            //Assert
            Assert.True(_selectPO.CheckCarritoVacio(),
                "Error: El carrito no está vacío tras borrar la herramienta.");

        }


        // Porcentajes inválidos
        [Theory]
        [Trait("LevelTesting", "Funcional Testing")]
        [InlineData(120)] 
        [InlineData(-20)] 
        public void UC3_6_FA3_Porcentajes_Error(int porcentajeError)
        {
            // Act
            PasosInicialesParaCrearOferta_UIT();
            _selectPO.BuscarHerramientas(fabricante1, precioHerramienta1);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta1);
            _selectPO.ContinuarConOferta();

            // Rellenamos datos válidos generales
            _crearPO.RellenarDatos(DateTime.Today.AddDays(1), DateTime.Today.AddDays(10), metodoPago, nombreUsuario, dirigidaA);

            // Introducimos el porcentaje erróneo
            _crearPO.EstablecerPorcentaje(nombreHerramienta1, porcentajeError);

            // Intentamos guardar
            _crearPO.PulsarCrearOferta();
            try { _crearPO.ConfirmarModal(); } catch { }

            // Assert
            bool seguimosEnCrear = _driver.Url.Contains("/oferta/crearoferta");
            Assert.True(seguimosEnCrear, $"El sistema permitió crear oferta con porcentaje {porcentajeError}%");

        }


        // Datos obligatorios vacíos
        public static IEnumerable<object[]> DatosVacios()
        {
            yield return new object[] { DateTime.MinValue, DateTime.Today.AddDays(20), nombreUsuario };
            yield return new object[] { DateTime.Today.AddDays(1), DateTime.MinValue, nombreUsuario };
            yield return new object[] { DateTime.Today.AddDays(1), DateTime.Today.AddDays(20), "" };
        }

        [Theory]
        [MemberData(nameof(DatosVacios))]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_7_FA5_DatosObligatorios
            (DateTime FechaInicio, DateTime FechaFinal, string nombreUsuario)
        {
            //Act
            PasosInicialesParaCrearOferta_UIT();
            _selectPO.BuscarHerramientas(fabricante1, precioHerramienta1);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta1);
            _selectPO.ContinuarConOferta();

            _crearPO.RellenarDatos(FechaInicio, FechaFinal, metodoPago, nombreUsuario, dirigidaA);
            _crearPO.PulsarCrearOferta();

            //Assert
            Assert.True(_crearPO.EstaActivoBotonCrear(), "Error: El botón no se ha deshabilitado");
        }


        // Usuario inválido
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_8_UsuarioInvalido_Error()
        {
            // Act
            PasosInicialesParaCrearOferta_UIT();
            _selectPO.BuscarHerramientas(fabricante1, precioHerramienta1);
            _selectPO.AñadirHerramientaAlCarrito(nombreHerramienta1);
            _selectPO.ContinuarConOferta();

            _crearPO.RellenarDatos(DateTime.Today.AddDays(1), DateTime.Today.AddDays(20), metodoPago, "Roberto", dirigidaA);

            _crearPO.PulsarCrearOferta();
            try { _crearPO.ConfirmarModal(); } catch {}

            // Assert
            bool hayError = _crearPO.CheckErroresMensaje("no existe") || _crearPO.CheckErroresMensaje("Error");
            Assert.True(hayError, "El sistema no mostró error al usar un usuario inexistente.");
        }        
    }
}