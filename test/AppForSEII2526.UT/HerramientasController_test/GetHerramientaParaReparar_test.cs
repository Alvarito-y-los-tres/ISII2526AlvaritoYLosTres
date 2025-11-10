using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;

namespace AppForSEII2526.UT.HerramientasController_test;

public class GetHerramientaParaReparar_test : AppForSEII25264SqliteUT
{
	public GetHerramientaParaReparar_test()
	{
		var fabricante = new List<Fabricante>
			{
				new Fabricante ("Ferretería López"),
				new Fabricante ("Ferretería García"),
				new Fabricante ("Ferretería Ruiz")
			};

		var herramientas = new List<Herramienta>
			{
				new Herramienta ("Martillo", "Madera", 5, 3, fabricante[0]),
				new Herramienta ("Taladro", "Acero", 30, 14, fabricante[1]),
				new Herramienta ("Alicates", "Hierro", 4, 5, fabricante[2])
			};

		_context.AddRange(fabricante);
		_context.AddRange(herramientas);
		_context.SaveChanges();

	}


	public static IEnumerable<object[]> TestCases_GetHerramientasParaReparar()
	{
		var herramientasDTOs = new List<HerramientaRepararDTO>
		{
			new HerramientaRepararDTO("Martillo", "Madera", "Ferretería López", 5, 3),
			new HerramientaRepararDTO("Taladro", "Acero", "Ferretería García", 30, 14),
			new HerramientaRepararDTO("Alicates", "Hierro", "Ferretería Ruiz", 4, 5)
		};

		var herramientasDTOs_TC1 = new List<HerramientaRepararDTO> { herramientasDTOs[0], herramientasDTOs[1], herramientasDTOs[2] }; // Sin filtros
		var herramientasDTOs_TC2 = new List<HerramientaRepararDTO> { herramientasDTOs[1] }; // Filtrar por nombre = "Taladro"
		var herramientasDTOs_TC3 = new List<HerramientaRepararDTO> { herramientasDTOs[0], herramientasDTOs[2] }; // Filtrar por tiempo <= 5

		var allTestCases = new List<object?[]>
		{
			new object[] { null, null, herramientasDTOs_TC1 },
			new object[] { "Taladro", null, herramientasDTOs_TC2 },
			new object[] { null, 5f, herramientasDTOs_TC3 }
		};
		return allTestCases;
	}
	
	[Theory]
	[MemberData(nameof(TestCases_GetHerramientasParaReparar))]
	public async Task GetHerramientaParaReparar_OK_test(string? nombre, float? tiempoReparacion, IList<HerramientaRepararDTO> herramientasDTOEsperado)
	{
		// Arrange
		var controller = new HerramientaController(_context, Mock.Of<ILogger<HerramientaController>>());

		// Act
		var result = await controller.GetHerramientasParaReparar(nombre, tiempoReparacion);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		var herramientasDTOActual = Assert.IsType<List<HerramientaRepararDTO>>(okResult.Value);

		Assert.Equal(herramientasDTOEsperado, herramientasDTOActual);

	}
}