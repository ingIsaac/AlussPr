using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cristales_pva
{
    class p_registros
    {
        public int folio { get; set; }
        public int dia { get; set; }
        public int mes { get; set; }
        public int año { get; set; }

        public p_registros(int folio, string fecha)
        {
            this.folio = folio;
            splitDate(fecha);
        }

        private void splitDate(string fecha)
        {           
            string[] p = fecha.Split('/');
            if (p.Length == 3)
            {
                dia = constants.stringToInt(p[0]);
                mes = constants.stringToInt(p[1]);
                año = constants.stringToInt(p[2]);
            }        
        }
    }
}
