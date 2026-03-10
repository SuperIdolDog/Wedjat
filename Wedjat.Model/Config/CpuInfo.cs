using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wedjat.Model.Config
{
   public class CpuInfo
    {
        private readonly PerformanceCounter _cpuCounter;
        public float UsagePercentage {  get; set; }
        public int ProcessorCount { get; set; }

        public CpuInfo() 
        {
            ProcessorCount = Environment.ProcessorCount;
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _cpuCounter.NextValue();
        }
        public override string ToString()
        {
            return $"CPU使用率:{UsagePercentage:F2}%"+ "\r\n"+$"处理器数量:{ProcessorCount}";
        }

        public void GetCpuInfo(out bool result, out string errorMsg)
        {
            result = false;
            errorMsg = string.Empty;
            try
            {
                Thread.Sleep(50);
                UsagePercentage = _cpuCounter.NextValue();
                result = true;
            }
            catch (Exception ex)
            {
                errorMsg = $"CPU信息获取失败：{ex.Message}\n堆栈：{ex.StackTrace}";
            }
        }
    }
}
