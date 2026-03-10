using FreeSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Wedjat.Model;


namespace Wedjat.DAL
{
    /// <summary>
    /// 基础数据访问层
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public  class BaseDAL<T> where T : BaseModel, new()
    {
       public  readonly IFreeSql _fSql;

       
        public  BaseDAL(IFreeSql fSql)
        {
            _fSql = fSql;
        }


      
        #region 查询
        /// <summary>
        ///根据主键获取一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public async  Task<T> GetModel(object id) 
        {
            try
            {
                return await _fSql.Select<T>().WhereDynamic(id).FirstAsync().ConfigureAwait(false); 
            }
            catch (Exception ex)
            {

                throw new Exception("根据主键获取一条数据失败:"+ex.Message);
            }
           
        }
        /// <summary>
        /// 根据条件获取一个实体
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public  async Task<T> GetModel(Expression<Func<T,bool>> expression)
        {
            try
            {
                return await _fSql.Select<T>().Where(expression).FirstAsync().ConfigureAwait(false); 
            }
            catch (Exception ex)
            {

                throw new Exception("根据条件获取一条实体失败:"+ex.Message);
            }
           
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public  async Task<List<T>> GetListAsync()
        {
            try
            {
                return await _fSql.Select<T>().ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw new Exception("获取所有数据失败:"+ex.Message);
            }
            
        }

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        public  async Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression)
        {
            try
            {
                return await _fSql.Select<T>().Where(expression).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw new Exception("根据条件获取多条数据失败:"+ex.Message);
            }
           
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="expression">查询条件</param>
        /// <param name="orderBy">排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        public  async Task<(List<T> Data, long Total)> GetPageListAsync(int pageIndex, int pageSize,
            Expression<Func<T, bool>> expression = null,
            Expression<Func<T, object>> orderBy = null,
            bool isAsc = true)
        {

            try
            {
                var query = _fSql.Select<T>();

                if (expression != null)
                    query = query.Where(expression);

                if (orderBy != null)
                    query = isAsc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

                var total = await query.CountAsync().ConfigureAwait(false);
                var data = await query.Page(pageIndex, pageSize).ToListAsync().ConfigureAwait(false);

                return (data, total);
            }
            catch (Exception ex)
            {

                throw new Exception("分页获取数据失败:"+ex.Message);
            }
           
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        public  async Task<long> CountAsync(Expression<Func<T, bool>> expression = null)
        {
            try
            {
                var query = _fSql.Select<T>();
                if (expression != null)
                    query = query.Where(expression);
                return await query.CountAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw new Exception("根据条件获取数据数量失败:"+ex.Message);
            }
           
        }

        #endregion


        #region 添加 
        //全部列 < 指定列(InsertColumns) < 忽略列(IgnoreColumns)

        /// <summary>
        /// 添加实体数据，并返回主键值(主键为long类型雪花id,实体需要设置主键特性),null值也会插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public  async Task<long> InsertModel(T t) 
        {
            try
            {
                long tempNum = await _fSql.Insert<T>(t).ExecuteIdentityAsync().ConfigureAwait(false);
                return tempNum == 0 ? 0 : t.Id;//0为插入失败
            }
            catch (Exception ex)
            {

                throw new Exception("添加数据失败:"+ex.Message);
            }
            
        }

        /// <summary>
        /// 添加实体数据,并指定插入字段集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="insertColumns"></param>
        /// <returns></returns>
        public  async  Task<long> InsertModel(T t, Expression<Func<T, object>> insertColumns)
        {
            try
            {
                long tempNum = await _fSql.Insert<T>(t).InsertColumns(insertColumns).ExecuteIdentityAsync().ConfigureAwait(false);
                return tempNum == 0 ? 0 : t.Id;//0为插入失败
            }
            catch (Exception ex)
            {

                throw new Exception("自定义插入数据失败:"+ex.Message);
            }
            
        }

        /// <summary>
        /// 插入一条，并忽略指定字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="ignoreColumns">要忽略的字段</param>
        /// <returns></returns>
        public  async Task<long> InsertModel_Ignore(T t, Expression<Func<T, object>> ignoreColumns) 
        {
            try
            {
                long tempNum = await _fSql.Insert<T>(t).IgnoreColumns(ignoreColumns).ExecuteIdentityAsync().ConfigureAwait(false);
                return tempNum == 0 ? 0 : t.Id;//0为插入失败
            }
            catch (Exception ex)
            {

                throw new Exception("插入一条忽略某字段的数据失败:"+ex.Message);
            }
           
        }

        #endregion

        #region 更新
        //> 全部列 < 指定列(Set/SetRaw) < 忽略列(IgnoreColumns) 

        /// <summary>
        /// 根据主键更新实体，返回影响条数(实体字段要有主键特性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public   async Task<long> UpdateModels(T t) 
        {
            try
            {
                return await _fSql.Update<T>().SetSource(t).ExecuteAffrowsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw new Exception("更新数据失败:"+ex.Message);
            }
           
        }

        /// <summary>
        /// 根据主键更新实体(要指定更新的列)，返回影响条数(实体字段要有主键特性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public  async Task<long> UpdateModels(T t, Expression<Func<T, object>> columns)
        {
            try
            {
                return await _fSql.Update<T>().SetSource(t).UpdateColumns(columns).ExecuteAffrowsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw new Exception("根据主键更新数据失败:"+ex.Message);
            }
           
            /*
             mo=new entity(){FontIcon=1,Icon=2,ID=1};
             FreeSqlHelper.UpdateModels(mo,p=>new {p.FontIcon,p.Icon })
             */
        }

        /// <summary>
        /// 根据主键更新实体(并忽略指定字段)，返回影响条数(实体字段要有主键特性)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="ignoreColumns">要忽略的字段</param>
        /// <returns></returns>
        public  async Task<long> UpdateModels_Ignore(T t, Expression<Func<T, object>> ignoreColumns)
        {
            try
            {
                return await _fSql.Update<T>().SetSource(t).IgnoreColumns(ignoreColumns).ExecuteAffrowsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw new Exception("根据主键更新(含有忽略字段)的数据失败:"+ex.Message);
            }
            
        }


        /// <summary>
        /// 动态更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns">要更新的字段</param>
        /// <param name="expressionWhere">条件表达式</param>
        /// <returns></returns>
        public   async Task<long> Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> expressionWhere) 
        {
            //这个方法也可以实现：_fSql.Update<T>(expressionWhere).Set(columns).ExecuteAffrows();
            try
            {
                return await _fSql.Update<T>().Set(columns).Where(expressionWhere).ExecuteAffrowsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw new Exception("根据条件更新数据失败:"+ex.Message);
            }
          
            // FreeSqlHelper.Update<SysModule>(p => new SysModule() {Icon = src}, p => p.ID.ToString() == id);
        }

        /*  update子查询。 注意freesql的Update方法只能单表操作。
                UPDATE sysModule
                    SET	
                         isend=0
                WHERE id=(SELECT parentID FROM sysModule AS sm WHERE sm.id=0)
            
            //多表update操作或update子查询，应该如下写法。
            int tempNum = db.Select<Entities.SysModule>()
                .Where(p => db.Select<Entities.SysModule>().Where(m => m.ID == id && p.ID == m.ParentID).Any())
                .ToUpdate()
                .Set(p => new Entities.SysModule() {IsLeaf = false})
                .ExecuteAffrows();
         */

        #endregion

        #region 删除方法 

        /// <summary>
        /// 删除ids集合条件的字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids">ids为字符串 "1,2,3"或"1" 形式</param>
        /// <returns></returns>
        public  async Task<bool> DeleteByIds(string ids) 
        {
            try
            {
                var idList = GetLongListByString(ids);
                return await _fSql.Delete<T>(idList).ExecuteAffrowsAsync().ConfigureAwait(false) > 0;
            }
            catch (Exception ex)
            {

                throw new Exception("删除1至多条数据失败:"+ex.Message);
            }
           
        }

        /// <summary>
        /// 根据条件表达式删除，返回影响条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public  async Task<long> DeleteModels(Expression<Func<T, bool>> expression)
        {
            //此方法也可以实现 ：_fSql.Delete<T>(expression).ExecuteAffrows();
            try
            {
                return await _fSql.Delete<T>().Where(expression).ExecuteAffrowsAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {

                throw new Exception("根据条件删除数据失败:"+ex.Message);
            }
            
        }

        #endregion

        #region ids转集合

        /// <summary>
        /// 将字符串转成int数组(, 号分割)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public   int[] GetIntListByString(string ids)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids)) return null;
                return Array.ConvertAll<string, int>(ids.Split(','), Int32.Parse);
            }
            catch (Exception ex)
            {

                throw new Exception("字符串转int数组失败:"+ex.Message);
            }
          
        }

        /// <summary>
        /// 将字符串转成long数组(, 号分割)
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public   long[] GetLongListByString(string ids)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ids)) return null;
                return Array.ConvertAll<string, long>(ids.Split(','), Convert.ToInt64);
            }
            catch (Exception ex)
            {

                throw new Exception("将字符串转成long数组失败:"+ex.Message);
            }
           
        }

        #endregion

    }
}
