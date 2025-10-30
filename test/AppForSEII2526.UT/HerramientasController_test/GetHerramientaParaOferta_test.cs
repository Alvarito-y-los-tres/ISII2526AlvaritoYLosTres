using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.HerramientasController_test
{
    public class GetHerramientaParaOferta_test: AppForSEII25264SqliteUT
    {
        public GetHerramientaParaOferta_test()
        {
            var fabricante = new List<Fabricante>
            {
                new Fabricante ("Ferreteía López"),
                new Fabricante ("Ferreteía García"),
                new Fabricante ("Ferreteía Ruiz")
            };

            var herramientas = new List<Herramienta>
            {
                new Herramienta ("Martillo", "Madera", 35, 3, fabricante[0]),
                new Herramienta ("Alicates", "Acero", 12, 1, fabricante[1]),
                new Herramienta ("Tijeras", "Plastico", 3, 1, fabricante[2])
            };

            ApplicationUser user = new ApplicationUser("Álvaro", "Cano Andrés", "600123456", "alvaro@gmail.com");

            
        }
    }
}
