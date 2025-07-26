using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio
{
    public class Pokemon
    {
        public int ID { get; set; }
        public int NUMERO { get; set; }
        public string NOMBRE { get; set; }
        public string DESCRIPCION{ get; set; }
        public string URLIMAGEN { get; set; }
        public Elemento TIPO { get; set; }
        public Elemento DEBILIDAD { get; set; }
    }
}
