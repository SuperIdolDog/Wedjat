using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Model;

namespace Wedjat.BLL
{
    /// <summary>
    /// 基础业务逻辑层
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class BaseBLL<T> where T : BaseModel, new()
    {
        protected readonly BaseDAL<T> _dal;

        public BaseBLL()
        {
            _dal = new BaseDAL<T> (AppDbContext.Sqlite);
        }

        #region 查询

        /// <summary>
        /// 根据主键获取一个实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<T> GetModelAsync(object id)
        {
            return await _dal.GetModel(id).ConfigureAwait(false);
        }

        /// <summary>
        /// 根据条件获取一个实体
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        public virtual async Task<T> GetModelAsync(Expression<Func<T, bool>> expression)
        {
            return await _dal.GetModel(expression).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync()
        {
            return await _dal.GetListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression)
        {
            return await _dal.GetListAsync(expression).ConfigureAwait(false);
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
        public virtual async Task<(List<T> Data, long Total)> GetPageListAsync(int pageIndex, int pageSize,
            Expression<Func<T, bool>> expression = null,
            Expression<Func<T, object>> orderBy = null,
            bool isAsc = true)
        {
            return await _dal.GetPageListAsync(pageIndex, pageSize, expression, orderBy, isAsc).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        public virtual async Task<long> CountAsync(Expression<Func<T, bool>> expression = null)
        {
            return await _dal.CountAsync(expression).ConfigureAwait(false);
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="expression">查询条件</param>
        /// <returns></returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _dal.CountAsync(expression).ConfigureAwait(false) > 0;
        }

        #endregion

        #region 添加

        /// <summary>
        /// 添加实体数据，并返回主键值
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public virtual async Task<long> InsertAsync(T entity)
        {
            // 这里可以添加业务逻辑，比如数据验证、设置默认值等
            ValidateEntity(entity);
            SetDefaultValues(entity);

            return await _dal.InsertModel(entity).ConfigureAwait(false);
        }

        /// <summary>
        /// 添加实体数据，并指定插入字段集合
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="insertColumns">插入字段</param>
        /// <returns></returns>
        public virtual async Task<long> InsertAsync(T entity, Expression<Func<T, object>> insertColumns)
        {
            ValidateEntity(entity);
            SetDefaultValues(entity);

            return await _dal.InsertModel(entity, insertColumns).ConfigureAwait(false);
        }

        /// <summary>
        /// 批量添加实体数据
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns></returns>
        public virtual async Task<bool> InsertBatchAsync(List<T> entities)
        {
            if (entities == null || entities.Count == 0)
                return false;

            foreach (var entity in entities)
            {
                ValidateEntity(entity);
                SetDefaultValues(entity);
                await _dal.InsertModel(entity).ConfigureAwait(false);
            }

            return true;
        }

        #endregion

        #region 更新

        /// <summary>
        /// 根据主键更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity)
        {
            ValidateEntity(entity);
            return await _dal.UpdateModels(entity).ConfigureAwait(false) > 0;
        }

        /// <summary>
        /// 根据主键更新实体（指定更新列）
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="columns">更新字段</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(T entity, Expression<Func<T, object>> columns)
        {
            ValidateEntity(entity);
            return await _dal.UpdateModels(entity, columns).ConfigureAwait(false) > 0;
        }

        /// <summary>
        /// 根据主键更新实体（忽略指定字段）
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="ignoreColumns">忽略字段</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateIgnoreAsync(T entity, Expression<Func<T, object>> ignoreColumns)
        {
            ValidateEntity(entity);
            return await _dal.UpdateModels_Ignore(entity, ignoreColumns).ConfigureAwait(false) > 0;
        }

        /// <summary>
        /// 根据条件更新
        /// </summary>
        /// <param name="columns">更新字段</param>
        /// <param name="expressionWhere">条件表达式</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(Expression<Func<T, T>> columns, Expression<Func<T, bool>> expressionWhere)
        {
            return await _dal.Update(columns, expressionWhere).ConfigureAwait(false) > 0;
        }

        #endregion

        #region 删除

        /// <summary>
        /// 根据主键删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(object id)
        {
            return await _dal.DeleteByIds(id.ToString()).ConfigureAwait(false);
        }

        /// <summary>
        /// 根据主键批量删除
        /// </summary>
        /// <param name="ids">主键集合</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteBatchAsync(string ids)
        {
            return await _dal.DeleteByIds(ids).ConfigureAwait(false);
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="expression">删除条件</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> expression)
        {
            return await _dal.DeleteModels(expression).ConfigureAwait(false) > 0;
        }

        #endregion

        #region 业务方法

        /// <summary>
        /// 实体验证（子类可重写此方法实现具体业务验证）
        /// </summary>
        /// <param name="entity">实体对象</param>
        protected virtual void ValidateEntity(T entity)
        {
            // 基础验证逻辑，子类可以重写添加具体验证
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "实体对象不能为空");
        }

        /// <summary>
        /// 设置默认值（子类可重写此方法设置实体默认值）
        /// </summary>
        /// <param name="entity">实体对象</param>
        protected virtual void SetDefaultValues(T entity)
        {
            // 设置创建时间、更新时间等默认值
            // 具体实现根据您的 BaseModel 定义
        }

        /// <summary>
        /// 软删除（如果实体支持软删除）
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task<bool> SoftDeleteAsync(object id)
        {
            // 这里假设实体有一个 IsDeleted 字段用于软删除
            // 具体实现需要根据您的实体结构调整
            var entity = await GetModelAsync(id).ConfigureAwait(false);
            if (entity == null) return false;

            // 使用反射设置 IsDeleted 为 true
            var property = typeof(T).GetProperty("IsDeleted");
            if (property != null && property.PropertyType == typeof(bool))
            {
                property.SetValue(entity, true);
                return await UpdateAsync(entity).ConfigureAwait(false);
            }

            return false;
        }

        #endregion
    }
}