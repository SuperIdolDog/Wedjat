using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model.Entity;

namespace Wedjat.BLL
{
    public class PCBInspectionBLL : BaseBLL<PCBInspection>
    {
        private readonly PCBInspectionDAL _pcbInspectionDAL;
        public PCBInspectionBLL():base()
        { 
            _pcbInspectionDAL = new PCBInspectionDAL();
        }
    }
}
