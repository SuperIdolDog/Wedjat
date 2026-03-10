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
    public class PLCSlaveVariableDAL : BaseDAL<PLCSlaveVariable>
    {
        public PLCSlaveVariableDAL() : base(AppDbContext.Sqlite)
        {

        }

        #region 根据主键获取当前数据块的信息
        public async Task<PLCSlaveVariable> SelectOneSlave(int id)
        {
            return await GetModel(id).ConfigureAwait(false); 
        }
        #endregion

        //#region 获取所有数据块的信息

        //public async Task<List<PLCSlaveVariable>> SelectAllSlave()
        //{
        //    return await GetListAsync();
        //}
        //#endregion

        #region 显示PLC的数据块参数
        public async Task<List<PLCSlaveVariable>> ShowSlaveVarToTable()
        {
            return await GetListAsync().ConfigureAwait(false); ;
        }
        #endregion
        public async Task<double> SelectCurrentValueByVariableNameAsync(string variableName)
        {
            if (string.IsNullOrWhiteSpace(variableName))
                return 0.0;

            
            var slaveVar = await GetModel(v => v.VariableName.Equals(variableName, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false); ;
            return slaveVar?.CurrentValue ?? 0.0;
        }

        public async Task<bool> UpdateCurrentValueByVariableNameAsync(string variableName, double newCurrentValue)
        {
            if (string.IsNullOrWhiteSpace(variableName))
                return false;

            long affectedRows = await Update(
                columns: v => new PLCSlaveVariable
                {
                    CurrentValue = newCurrentValue,
                    updateTime = DateTime.Now
                },
                expressionWhere: v => v.VariableName.Equals(variableName, StringComparison.OrdinalIgnoreCase)
            ).ConfigureAwait(false); ;

            return affectedRows > 0;
        }

        public async Task<bool> UpdateCurrentValueByIdAsync(int id, double newValue)
        {
            try
            {
                var data = new PLCSlaveVariable
                {
                    Id = id,
                    CurrentValue = newValue,
                    UpdateTime = DateTime.Now
                };

                return await UpdateModels(data, p => new { p.CurrentValue, p.UpdateTime }).ConfigureAwait(false) > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DAL层更新CurrentValue失败: {ex.Message}");
                return false;
            }
        }
    }
}
