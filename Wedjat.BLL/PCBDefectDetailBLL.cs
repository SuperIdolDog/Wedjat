using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model.Entity;

namespace Wedjat.BLL
{
    public class PCBDefectDetailBLL:BaseBLL<PCBDefectDetail>
    {
        private readonly PCBDefectDetailDAL _defectDetailDAL;
        public PCBDefectDetailBLL():base() 
        { 
            _defectDetailDAL = new PCBDefectDetailDAL();
        }
    }
}
