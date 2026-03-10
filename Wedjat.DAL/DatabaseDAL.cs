using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.DAL
{
    public class DatabaseDAL
    {
       private readonly IFreeSql _fSql;

        public  DatabaseDAL(IFreeSql fSql)
        {
            _fSql = fSql;
        }
        #region 获取数据表表名
        public async Task<List<string>> QueryAllTableNamesAsync()
        {
            try
            {

                var tableNames = await _fSql.Ado.QueryAsync<string>(@"
                    SELECT name 
                    FROM sqlite_master 
                    WHERE type = 'table' 
                    AND name NOT LIKE 'sqlite_%'  
                    ORDER BY name ASC
                ").ConfigureAwait(false);


                return tableNames
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct()
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("查询数据库表名失败：" + ex.Message);
            }
        }
        #endregion
       
    }
}
