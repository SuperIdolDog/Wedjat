using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;

namespace Wedjat.BLL
{
    public class ScannerDataBLL:BaseBLL<ScannerData>
    {
        private readonly ScannerDataDAL _scannerDal;

        public ScannerDataBLL() :base()
        {
            _scannerDal=new ScannerDataDAL();
        }
        public async Task<(List<ScannerData> Data, long Total)> ShowScanCodeRecord(
            int pageIndex = 1,
            int pageSize = 20,
            Expression<Func<ScannerData, bool>> whereExpression = null,
            Expression<Func<ScannerData, object>> orderByExpression = null,
            bool isAsc = false)
        {
           
            var list= await _dal.GetPageListAsync(pageIndex, pageSize, whereExpression, orderByExpression, isAsc);
            return list;
        }
        public async Task<ScannerData> AddScanRecord(ScannerDataDTO dto)
        {
       
            return await _scannerDal.ScanCoderecord(dto);
        }
    }
}
