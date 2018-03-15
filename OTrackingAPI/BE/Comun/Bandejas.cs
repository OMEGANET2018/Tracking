using BE.Administracion;
using System.Collections.Generic;

namespace BE.Comun
{
    public class BandejaComponente : Componente
    {
        public int RecordStatus { get; set; }
        public List<BandejaComponente> Lista { get; set; }
    }
}
