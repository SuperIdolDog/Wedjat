using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWindow.SupportROIModel
{
    public class PaintROI
    {
        /// <summary>
        /// 核心
        /// </summary>
        public List<RoIInfo> MatchRoIInfo = new List<RoIInfo>();

        /// <summary>
        /// 构造函数1
        /// </summary>
        public PaintROI()
        {

        }

        /// <summary>
        /// 根据区域类型，查询ROI信息
        /// </summary>
        /// <param name="iModel">区域类型</param>
        /// <param name="iType">ROI类型</param>
        /// <param name="RoIData">数据</param>
        /// <returns></returns>
        public bool QueryRoI(RoIModel iModel, RoIType iType, ref List<double> RoIData)
        {
            int rect1index = MatchRoIInfo.FindIndex(c => c.roIModel == iModel && c.roIType == iType);
            if (rect1index > -1)
            {
                RoIData = MatchRoIInfo[rect1index].RoIdata;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据区域类型，查询ROI信息
        /// </summary>
        /// <param name="iModel">区域类型</param>
        /// <param name="iType">ROI类型</param>
        /// <param name="RoIData">数据</param>
        /// <returns></returns>
        public bool QueryRoI(RoIType iType, ref List<double> RoIData)
        {
            int rect1index = MatchRoIInfo.FindIndex(c => c.roIType == iType);
            if (rect1index > -1)
            {
                RoIData = MatchRoIInfo[rect1index].RoIdata;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据区域类型，查询ROI信息
        /// </summary>
        /// <param name="iModel"></param>
        /// <param name="RoIData"></param>
        /// <returns></returns>
        public bool QueryRoI(RoIModel iModel, ref RoIType iType, ref List<double> RoIData)
        {
            int rect1index = MatchRoIInfo.FindIndex(c => c.roIModel == iModel);
            if (rect1index > -1)
            {
                iType = MatchRoIInfo[rect1index].roIType;
                RoIData = MatchRoIInfo[rect1index].RoIdata;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 修改ROI保存参数
        /// </summary>
        /// <param name="iType">模RoI信息</param>
        /// <param name="iData">数据信息</param>
        public void UpdateROI(RoIType iType, List<double> iData)
        {
            int rect1index = MatchRoIInfo.FindIndex(c => c.roIType == iType);
            if (rect1index > -1)
            {
                MatchRoIInfo[rect1index].roIType = iType;
                MatchRoIInfo[rect1index].RoIdata = iData;
            }
            else
            {
                MatchRoIInfo.Add(new RoIInfo { roIType = iType, RoIdata = iData });
            }
        }

        /// <summary>
        /// 修改ROI保存参数
        /// </summary>
        /// <param name="iModel">区域类型</param>
        /// <param name="iType">模RoI信息</param>
        /// <param name="iData">数据信息</param>
        public void UpdateROI(RoIModel iModel, RoIType iType, List<double> iData)
        {
            int rect1index = MatchRoIInfo.FindIndex(c => c.roIModel == iModel);
            if (rect1index > -1)
            {
                MatchRoIInfo[rect1index].roIType = iType;
                MatchRoIInfo[rect1index].RoIdata = iData;
            }
            else
            {
                MatchRoIInfo.Add(new RoIInfo { roIModel = iModel, roIType = iType, RoIdata = iData });
            }
        }

    }

    [Serializable]
    public class RoIInfo
    {
        public RoIModel roIModel { get; set; }

        public RoIType roIType { get; set; }

        public List<double> RoIdata { get; set; }

    }

    //区域类型
    public enum RoIModel
    {
        None,
        搜索区域,
        模板区域,
    }

    //搜索区域ROI方式
    public enum RoILink
    {
        None,
        手动区域,
        链接区域,
    }

    //模RoI信息
    public enum RoIType
    {
        None,
        方行,
        矩形,
        圆形,
        直线
    }
}
