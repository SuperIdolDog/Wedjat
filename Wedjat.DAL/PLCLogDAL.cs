using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;

namespace Wedjat.DAL
{
    public class PLCLogDAL : BaseDAL<PLCLog>
    {
        public PLCLogDAL() : base(AppDbContext.Sqlite)
        {
               
    }
        #region  插入PLC日志记录
        public async Task<PLCLog> AddPLCLog(PLCLogDTO dto)
        {
            var plcLog = new PLCLog
            {
                SlaveAddress = dto.SlaveAddress,          
                CommunicationType = dto.CommunicationType, 
                AddressInfo = dto.AddressInfo,             
                Data = dto.Data,                          
                IsSuccess = dto.IsSuccess,                
                ErrorMessage = dto.ErrorMessage,
                ConnectionType = dto.ConnectionType,
                ConnectionParams = dto.ConnectionParams
            };
            var result = await InsertModel(plcLog);
            
           
            return plcLog; 
        }
        #endregion
    }
}
