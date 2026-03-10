using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;

namespace Wedjat.BLL
{
    public class PLCLogBLL:BaseBLL<PLCLog>
    {
        private readonly PLCLogDAL _PLCLogdal;
        public PLCLogBLL():base()
        {
            _PLCLogdal = new PLCLogDAL();
        }
        public async Task<PLCLog> AddPLCLog(PLCLogDTO dto)
        {
            var result = await _PLCLogdal.AddPLCLog(dto);
            return result;
        }
    }
}
