using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model;

namespace Wedjat.BLL
{
    public class DatabaseBLL
    {
        private DatabaseDAL _dbDAL;
        public DatabaseBLL()
        {
            _dbDAL = new DatabaseDAL(AppDbContext.Sqlite);
        }
        public async Task<List<string>> QueryAllTableNamesAsync()
        {
            

            try
            {
             return await  _dbDAL.QueryAllTableNamesAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("查询数据库表名失败（DAL层）：" + ex.Message, ex);
            }
        }
    }
}
