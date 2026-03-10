using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model.Entity;

namespace Wedjat.BLL
{
   public class PCBDefectTypeBLL : BaseBLL<PCBDefectType>
    {
        private readonly PCBDefectTypeDAL _defectTypeDAL;
        public PCBDefectTypeBLL():base() 
        { 
            _defectTypeDAL = new PCBDefectTypeDAL();    
        }
    }
}
