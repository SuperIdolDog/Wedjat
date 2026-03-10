using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.Config
{
    public static class TestNumberInfo
    {
        public static int TotalCount = 0;

        public static int NGCount = 0;

        public static int OKCount = 0;
        public static int[] SplitTotalCount(int count)
        {
            int limited = count % 10000; // 限制为4位数字
            if (limited < 0) limited += 10000;

            int thousands = limited / 1000;       // 千位
            int hundreds = (limited % 1000) / 100; // 百位
            int tens = (limited % 100) / 10;       // 十位
            int units = limited % 10;              // 个位

            return new[] { thousands, hundreds, tens, units };
        }
        public static void ResetCounts()
        {
            TotalCount = 0;
            NGCount = 0;
            OKCount = 0;
        }
    }
}
