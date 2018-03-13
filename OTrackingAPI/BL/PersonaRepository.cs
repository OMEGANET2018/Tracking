using DAL;
using System;
using System.Linq;

namespace BL
{
   
    public class PersonaRepository
    {
        private DatabaseContext ctx = new DatabaseContext();

        public bool Prueba ()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
