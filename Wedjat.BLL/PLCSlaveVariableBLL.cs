using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wedjat.DAL;
using Wedjat.Driver;
using Wedjat.Model.DTO;
using Wedjat.Model.Entity;

namespace Wedjat.BLL
{
    public class PLCSlaveVariableBLL : BaseBLL<PLCSlaveVariable>
    {
        private readonly PLCSlaveVariableDAL _pLCSlaveVariableDAL;
        private readonly NModbusDriver _nModbusDriver;
        private List<PLCSlaveVariable> _lastReadVariables = new List<PLCSlaveVariable>();
        public PLCSlaveVariableBLL(NModbusDriver modbusDriver):base()
        {
            _pLCSlaveVariableDAL = new PLCSlaveVariableDAL();
            _nModbusDriver = modbusDriver;
        }

        public async Task<List<PLCSlaveVariable>> ShowSlaveVarToTable()
        {
            try
            {
                return await _pLCSlaveVariableDAL.ShowSlaveVarToTable();
            }
            catch (Exception ex)
            {

                throw new Exception("显示数据失败" + ex.Message);
            }
           
        }

        //public async Task ReadSlaveFromPLCAsync()
        //{
        //    try
        //    {
        //        if (_nModbusDriver==null)
        //        {
        //            throw new Exception("Modbus 未连接，请先建立连接");
        //        }
        //        var allVariables = await _pLCSlaveVariableDAL.ShowSlaveVarToTable();

        //        if (allVariables == null || !allVariables.Any())
        //        {
        //            throw new Exception("数据库中未查询到任何PLC变量");
        //        }

        //        foreach (var variable in allVariables)
        //        {


        //            try
        //            {
        //                switch (variable.ModbusType)
        //                {
        //                    case "保持寄存器":
        //                        byte[] regData = _nModbusDriver.ReadHoldingRegisters(
        //                            (byte)variable.SlaveAddress,
        //                            (ushort)variable.ModbusAddress,
        //                            1);

        //                        variable.CurrentValue = BitConverter.ToUInt16(regData, 0);
        //                        break;

        //                    case "线圈":
        //                        bool[] coilData = _nModbusDriver.ReadOutputCoils(
        //                            (byte)variable.SlaveAddress,
        //                            (ushort)variable.ModbusAddress,
        //                            1);
        //                        variable.CurrentValue = coilData[0] ? 1 : 0;
        //                        break;

        //                    case "离散输入":
        //                        bool[] inputCoilData = _nModbusDriver.ReadInputCoils(
        //                            (byte)variable.SlaveAddress,
        //                            (ushort)variable.ModbusAddress,
        //                            1);
        //                        variable.CurrentValue = inputCoilData[0] ? 1 : 0;
        //                        break;

        //                    case "输入寄存器":
        //                        byte[] inputRegData = _nModbusDriver.ReadInputRegister(
        //                            (byte)variable.SlaveAddress,
        //                            (ushort)variable.ModbusAddress,
        //                            2);

        //                        variable.CurrentValue = BitConverter.ToUInt16(inputRegData, 0);
        //                        break;

        //                    default:
        //                        Console.WriteLine($"跳过不支持的变量类型：{variable.ModbusType}（ID：{variable.Id}）");
        //                        continue;
        //                }

        //                variable.UpdateTime = DateTime.Now;
        //                await _pLCSlaveVariableDAL.UpdateModels(
        //                    variable,
        //                    p => new { p.CurrentValue, p.UpdateTime }).ConfigureAwait(false);

        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"读取变量失败（ID：{variable.Id}，名称：{variable.VariableName}）：{ex.Message}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception("读取变量失败:" + ex.Message);
        //    }

        //}
        public async Task<List<PLCSlaveVariable>> ReadChangedValuesOnlyAsync()
        {
            try
            {
                // 1. 获取当前变量列表
                var currentVariables = await _pLCSlaveVariableDAL.ShowSlaveVarToTable();
                if (currentVariables == null || !currentVariables.Any())
                    return new List<PLCSlaveVariable>();

                // 2. 如果上次读取为空，则初始化并返回（第一次读取）
                if (!_lastReadVariables.Any())
                {
                    _lastReadVariables = currentVariables.Select(v =>
                    {
                        var copy = new PLCSlaveVariable
                        {
                            Id = v.Id,
                            CurrentValue = v.CurrentValue,
                            UpdateTime = v.UpdateTime,
                            VariableName=v.VariableName
                        };
                        return copy;
                    }).ToList();
                    return currentVariables;
                }

                // 3. 只读取可能发生变化的变量（根据业务逻辑）
                var changedVariables = new List<PLCSlaveVariable>();
                var variablesToRead = new List<PLCSlaveVariable>();

                // 确定需要读取的变量（例如：只读取可写或重要的变量）
                foreach (var variable in currentVariables)
                {
                    // 根据业务需求筛选，例如：
                    // - 只读取可写变量
                    // - 只读取最近有变化的变量
                    // - 根据优先级读取
                    if (variable.IsWritable || ShouldReadVariable(variable))
                    {
                        variablesToRead.Add(variable);
                    }
                }

                // 4. 并行读取多个变量（如果有连接）
                if (_nModbusDriver != null )
                {
                    // 使用Parallel.ForEach并行读取（注意线程安全）
                    var parallelOptions = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount
                    };

                    // 用于收集结果的列表
                    var readResults = new ConcurrentBag<(long Id, double Value)>();

                    await Task.Run(() =>
                    {
                        Parallel.ForEach(variablesToRead, parallelOptions, variable =>
                        {
                            try
                            {
                                double newValue = ReadSingleValue(variable);

                                // 检查是否真正变化
                                var lastValue = _lastReadVariables
                                    .FirstOrDefault(v => v.Id == variable.Id)?.CurrentValue ?? 0;

                                if (Math.Abs(lastValue - newValue) > 0.001)
                                {
                                    variable.CurrentValue = newValue;
                                    variable.UpdateTime = DateTime.Now;

                                    // 添加到变更列表
                                    lock (changedVariables)
                                    {
                                        changedVariables.Add(variable);
                                    }

                                    // 记录读取结果
                                    readResults.Add((variable.Id, newValue));
                                }
                            }
                            catch (Exception ex)
                            {
                                // 静默处理，不影响其他变量读取
                                System.Diagnostics.Debug.WriteLine($"读取变量{variable.Id}失败: {ex.Message}");
                            }
                        });
                    });

                    // 5. 批量更新有变化的变量到数据库
                    if (changedVariables.Any())
                    {
                        await BatchUpdateVariables(changedVariables);

                        // 更新上次读取记录
                        foreach (var result in readResults)
                        {
                            var lastVar = _lastReadVariables.FirstOrDefault(v => v.Id == result.Id);
                            if (lastVar != null)
                            {
                                lastVar.CurrentValue = result.Value;
                                lastVar.UpdateTime = DateTime.Now;
                            }
                        }
                    }
                }

                return currentVariables;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"读取变量失败: {ex.Message}");
                return new List<PLCSlaveVariable>();
            }
        }
        // 判断是否需要读取该变量（根据业务逻辑）
        private bool ShouldReadVariable(PLCSlaveVariable variable)
        {
            // 示例：只读取特定类型的变量
            string[] highPriorityTypes = { "保持寄存器", "线圈" ,"离散输入"};
            return highPriorityTypes.Contains(variable.ModbusType);
        }

        // 读取单个变量值（同步版本）
        private double ReadSingleValue(PLCSlaveVariable variable)
        {
            try
            {
                switch (variable.ModbusType)
                {
                    case "保持寄存器":
                        byte[] regData = _nModbusDriver.ReadHoldingRegisters(
                            (byte)variable.SlaveAddress,
                            (ushort)variable.ModbusAddress,
                            1);
                        return BitConverter.ToUInt16(regData, 0);

                    case "线圈":
                        bool[] coilData = _nModbusDriver.ReadOutputCoils(
                            (byte)variable.SlaveAddress,
                            (ushort)variable.ModbusAddress,
                            1);
                        return coilData[0] ? 1 : 0;

                    case "离散输入":
                        bool[] inputCoilData = _nModbusDriver.ReadInputCoils(
                            (byte)variable.SlaveAddress,
                            (ushort)variable.ModbusAddress,
                            1);
                        return inputCoilData[0] ? 1 : 0;

                    case "输入寄存器":
                        byte[] inputRegData = _nModbusDriver.ReadInputRegisters(
                            (byte)variable.SlaveAddress,
                            (ushort)variable.ModbusAddress,
                            1);
                        return BitConverter.ToUInt16(inputRegData, 0);

                    default:
                        return variable.CurrentValue;
                }
            }
            catch
            {
                return variable.CurrentValue; // 读取失败时返回原值
            }
        }

        // 批量更新变量到数据库
        private async Task BatchUpdateVariables(List<PLCSlaveVariable> variables)
        {
            if (!variables.Any()) return;

            try
            {
                // 使用FreeSql的批量更新
                var updateTasks = variables.Select(variable =>
                    _pLCSlaveVariableDAL.UpdateModels(
                        variable,
                        p => new { p.CurrentValue, p.UpdateTime }
                    )
                );

                // 并行执行更新（限制并发数）
                var results = await Task.WhenAll(updateTasks);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"批量更新失败: {ex.Message}");
            }
        }
        public async Task ReadSlaveFromPLCAsync()
        {
            try
            {
                if (_nModbusDriver == null)
                {
                    throw new Exception("Modbus 未连接，请先建立连接");
                }

                var allVariables = await _pLCSlaveVariableDAL.ShowSlaveVarToTable();
                if (allVariables == null || !allVariables.Any())
                {
                    throw new Exception("数据库中未查询到任何PLC变量");
                }

                // 分批处理，避免一次性处理太多
                int batchSize = 10; // 每次处理10个变量
                for (int i = 0; i < allVariables.Count; i += batchSize)
                {
                    var batch = allVariables.Skip(i).Take(batchSize).ToList();

                    // 处理当前批次
                    foreach (var variable in batch)
                    {
                        try
                        {
                            double oldValue = variable.CurrentValue;
                            double newValue = ReadSingleValue(variable);

                            // 只有变化时才更新
                            if (Math.Abs(oldValue - newValue) > 0.001)
                            {
                                variable.CurrentValue = newValue;
                                variable.UpdateTime = DateTime.Now;

                                // 立即更新数据库（异步）
                                _ = _pLCSlaveVariableDAL.UpdateModels(
                                    variable,
                                    p => new { p.CurrentValue, p.UpdateTime }
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"读取变量失败: {ex.Message}");
                        }
                    }

                    // 小延迟，避免过快
                    await Task.Delay(10);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取变量失败:" + ex.Message);
            }
        }
        public async Task<PLCSlaveVariable> UpdateSlaveFromScadaAsync(PLCValueUpdateDTO dto)
        {
            if (_nModbusDriver == null)
                throw new InvalidOperationException("TCP/RTU连接已断开，无法执行写入操作");
            if (dto.Value == null)
                throw new ArgumentException("输入值不能为空");

            string inputValue = dto.Value.ToString().Trim();
            var data = new PLCSlaveVariable
            {
                Id = dto.Id,
                UpdateTime = DateTime.Now
            };

            try
            {
                switch (dto.ModbusType)
                {
                    case "保持寄存器":
                        if (!int.TryParse(inputValue, out int regInt) || regInt < 0 || regInt > 65535)
                            throw new ArgumentException("保持寄存器值必须是0~65535之间的整数");

                        // 同步Modbus操作包装到Task.Run，脱离UI线程
                        await Task.Run(() =>
                        {
                            _nModbusDriver.WriteSingleRegister((byte)dto.SlaveId, (ushort)dto.Address, (ushort)regInt);
                        }).ConfigureAwait(false);

                        data.CurrentValue = regInt;
                        // 数据库更新必须用异步方法（需DAL层配合）
                        await _pLCSlaveVariableDAL.UpdateModels(data, p => new { p.CurrentValue, p.UpdateTime }).ConfigureAwait(false);
                        break;

                    case "线圈":
                        bool isOn = inputValue.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                                   inputValue.Equals("true", StringComparison.OrdinalIgnoreCase);
                        if (!isOn && !inputValue.Equals("0", StringComparison.OrdinalIgnoreCase) &&
                            !inputValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                            throw new ArgumentException("线圈值必须是0（false）或1（true）");

                        // 同步Modbus操作异步化
                        await Task.Run(() =>
                        {
                            _nModbusDriver.WriteSingleCoil((byte)dto.SlaveId, (ushort)dto.Address, isOn);
                        }).ConfigureAwait(false);

                        data.CurrentValue = isOn ? 1 : 0;
                        await _pLCSlaveVariableDAL.UpdateModels(data, p => new { p.CurrentValue, p.UpdateTime }).ConfigureAwait(false);
                        break;

                    default:
                        throw new NotSupportedException($"不支持的变量类型：{dto.ModbusType}");
                }
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("更新数据失败：" + ex.Message);
            }
        }

        public async Task<int> GetCurrentValueByVariableNameAsync(PLCValueUpdateDTO dto)
        {
           

            try
            {
                if (string.IsNullOrWhiteSpace(dto.VariableName))
                    throw new ArgumentException("变量名不能为空");
                double dbValue = await _pLCSlaveVariableDAL.SelectCurrentValueByVariableNameAsync(dto.VariableName);
                return (int)Math.Round(dbValue);
            }
            catch (Exception ex)
            {
                throw new Exception($"查询变量[{dto.VariableName}]值失败：{ex.Message}");
            }
        }

        public async Task<bool> UpdateCurrentValueByVariableNameAsync(PLCValueUpdateDTO dto)
        {
            
           


            try
            {
                if (string.IsNullOrWhiteSpace(dto.VariableName))
                    throw new ArgumentException("变量名不能为空", nameof(dto.VariableName));
                // 1. 查询变量完整信息
                var targetVar = await _pLCSlaveVariableDAL.GetModel(v =>
                    v.VariableName.Equals(dto.VariableName, StringComparison.OrdinalIgnoreCase)
                );
                if (targetVar == null)
                    throw new Exception($"未找到变量：{dto.VariableName}");
                if (!targetVar.IsWritable)
                    throw new Exception($"变量[{dto.VariableName}]不可写");

                // 2. 安全转换dto.Value，避免强转失败（核心修改）
                double safeValue;
                if (!double.TryParse(dto.Value?.ToString(), out safeValue))
                    throw new Exception($"变量[{dto.VariableName}]值转换失败：{dto.Value}不是有效的数字");

                // 3. 验证新值合法性（基于安全转换后的值，修正强转）
                switch (targetVar.ModbusType)
                {
                    case "保持寄存器":
                    case "输入寄存器":
                        // 校验是否为整数+范围（0-65535）
                        if (safeValue < 0 || safeValue > 65535 || safeValue != Math.Floor(safeValue))
                            throw new ArgumentException(nameof(dto.Value), $"{targetVar.ModbusType}值必须是0~65535之间的整数，当前值：{safeValue}");
                        break;
                    case "线圈":
                    case "离散输入":
                        // 线圈只能是0或1，转换为int后校验
                        int coilValue = (int)Math.Round(safeValue);
                        if (coilValue != 0 && coilValue != 1)
                            throw new ArgumentException(nameof(dto.Value), $"{targetVar.ModbusType}值必须是0或1，当前值：{safeValue}");
                        break;
                    default:
                        throw new Exception($"不支持的Modbus类型：{targetVar.ModbusType}");
                }

                // 4. 更新数据库（使用安全转换后的值）
                bool dbUpdated = await _pLCSlaveVariableDAL.UpdateCurrentValueByVariableNameAsync(
                    dto.VariableName,
                    safeValue // 无需强转，直接用安全转换后的double
                );
                if (!dbUpdated)
                    throw new Exception($"数据库更新失败：{dto.VariableName}");

                // 5. 同步到Modbus（按Modbus类型安全转换写入类型）
                await Task.Run(() =>
                {
                    
                        switch (targetVar.ModbusType)
                        {
                            case "保持寄存器":
                                // 安全转换为ushort（已校验范围0-65535，不会溢出）
                                ushort regValue = (ushort)safeValue;
                                try
                                {
                                    _nModbusDriver.WriteSingleRegister(
                                       (byte)targetVar.SlaveAddress,
                                       (ushort)targetVar.ModbusAddress,
                                       regValue
                                   );
                                }
                                catch (Exception ex)
                                {

                                    throw new Exception(ex.Message);
                                }
                               
                                break;
                            case "线圈":
                                // 转换为bool（0=false，1=true）
                                bool coilValue = (int)Math.Round(safeValue) == 1;
                                try
                                {
                                    _nModbusDriver.WriteSingleCoil(
                                       (byte)targetVar.SlaveAddress,
                                       (ushort)targetVar.ModbusAddress,
                                       coilValue
                                   );
                                }
                                catch (Exception ex)
                                {

                                    throw new Exception(ex.Message);
                                }

                            break;
                            case "输入寄存器":
                            case "离散输入":
                                throw new Exception($"{targetVar.ModbusType}不可写");
                            default:
                                throw new Exception($"不支持的Modbus类型：{targetVar.ModbusType}");
                        }
                });

                Console.WriteLine($"变量[{dto.VariableName}]更新成功：新值={safeValue}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新变量[{dto.VariableName}]值失败：{ex.Message}", ex);
                return false;
            }
        }
    }
}
