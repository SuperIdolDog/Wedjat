using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model.Entity;

namespace Wedjat.BLL
{
    public class PCBBLL : BaseBLL<PCB>
    {
        private PCBDAL _pcbDAL;
        public PCBBLL():base() 
        { 
            _pcbDAL = new PCBDAL();
        }
    }
}
