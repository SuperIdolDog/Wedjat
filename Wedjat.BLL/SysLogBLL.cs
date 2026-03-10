using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model.Entity;

namespace Wedjat.BLL
{
    public class SysLogBLL : BaseBLL<SysLog>
    {
        private readonly SysLogDAL _SysLogdal;
        public SysLogBLL() : base() 
        {
            _SysLogdal = new SysLogDAL();
        }
    }
}
