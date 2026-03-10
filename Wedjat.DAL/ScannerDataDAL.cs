using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;


namespace Wedjat.DAL
{
    public class ScannerDataDAL : BaseDAL<ScannerData>
    {
       
        public ScannerDataDAL() : base(AppDbContext.Sqlite)
        {

        }

       

        #region 展示扫码记录
        public async Task<(List<ScannerData> Data, long Total)> ShowScanCodeRecord(
            int pageIndex = 1,
            int pageSize = 20,
            Expression<Func<ScannerData, bool>> whereExpression = null,
            Expression<Func<ScannerData, object>> orderByExpression = null,
            bool isAsc = false)
        {
            if (orderByExpression == null)
            {
                orderByExpression = x => x.ScanTime;
                isAsc = false; 
            }
            return await GetPageListAsync(pageIndex, pageSize, whereExpression, orderByExpression, isAsc);
        }
        #endregion

        #region 插入扫码枪扫描记录
        public async Task<ScannerData> ScanCoderecord(ScannerDataDTO dto)
        {
            var scanRecord = new ScannerData
            {
                CodeContent = dto.ScanContent,          
                OperatorWorkId = dto.OperatorWorkId,    
                ScanTime = dto.ScanTime,               
                WorkOrderNo = dto.Result,                  
                Statu = dto.ScanStatus                                               
            };
            long insertResult = await InsertModel(scanRecord);
            if (insertResult < 0)
            {
                throw new Exception("扫码记录插入数据库失败");
            }
            return scanRecord;
        }
        #endregion
       
    }
}
