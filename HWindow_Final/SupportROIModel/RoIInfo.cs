using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWindow.SupportROIModel
{
    /// <summary>
    /// 直线信息
    /// </summary>
    [Serializable]
    public struct Line_INFO
    {
        public double StartY;//起点行坐标
        public double StartX;//起点列坐标
        public double EndX;//终点行坐标
        public double EndY;//终点列坐标
        public double Ny;//行向量
        public double Nx;//列向量
        public double Dist;//距离
        public double Phi;//方向
        public double MidY;//中间点行坐标
        public double MidX;//中间点列坐标

        public Line_INFO(double m_start_Row, double m_start_Col, double m_end_Row, double m_end_Col)
        {
            this.StartY = m_start_Row;
            this.StartX = m_start_Col;
            this.EndY = m_end_Row;
            this.EndX = m_end_Col;
            this.Ny = m_start_Col - m_end_Col;
            this.Nx = m_end_Row - m_start_Row;
            this.Dist = m_start_Col * m_end_Row - m_end_Col * m_start_Row;
            Phi = HMisc.AngleLx(StartY, StartX, EndY, EndX);
            MidY = (StartY + EndY) / 2;
            MidX = (StartX + EndX) / 2;
        }
        public HXLDCont genXLD()
        {
            HXLDCont xld = new HXLDCont();
            xld.GenContourPolygonXld(new HTuple(StartX, EndX), new HTuple(StartY, EndY));
            return xld;
        }
    }

    /// <summary>
    /// 面信息
    /// </summary>
    [Serializable]
    public struct Plane_INFO
    {
        public double x, y, z;
        public double ax, by, cz, d;
        public double Angle;
        public double xAn, yAn, zAn;
        public double Flat, MinFlat, MaxFlat;
        public double MinZ, MaxZ;
    }

    /// <summary>
    /// 圆信息
    /// </summary>
    [Serializable]
    public class Circle_INFO : ROI, ICloneable
    {
        public double CenterY, CenterX, Radius;
        public double StartPhi = 0.0, EndPhi = Math.PI * 2;
        public string PonitOrder = "positive";

        public Circle_INFO()
        {

        }

        public Circle_INFO(double m_Row_center, double m_Colum_center, double m_Radius)
        {
            this.CenterY = m_Row_center;
            this.CenterX = m_Colum_center;
            this.Radius = m_Radius;
        }

        public Circle_INFO(double m_Row_center, double m_Colum_center, double m_Radius, double m_StartPhi, double m_EndPhi, string m_PointOrder)
        {
            this.CenterY = m_Row_center;
            this.CenterX = m_Colum_center;
            this.Radius = m_Radius;
            this.StartPhi = m_StartPhi;
            this.EndPhi = m_EndPhi;
        }

        public override HRegion GenRegion()
        {
            HRegion h = new HRegion();
            h.GenCircle(CenterY, CenterX, Radius);
            return h;
        }

        public override HXLDCont GenXld()
        {
            HXLDCont xld = new HXLDCont();
            xld.GenCircleContourXld(CenterY, CenterX, Radius, StartPhi, EndPhi, PonitOrder, 1.0);
            return xld;
        }

        public override HTuple GetTuple()
        {
            double[] circle = new double[] { CenterY, CenterX, Radius };
            return new HTuple(circle);
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    /// <summary>
    /// 椭圆信息
    /// </summary>
    [Serializable]
    public class Ellipse_INFO : ROI, ICloneable
    {
        public double CenterY, CenterX, Phi, Radius1, Radius2;
        double StartPhi = 0.0, EndPhi = Math.PI * 2;
        public string PointOrder = "positive";

        public Ellipse_INFO()
        {

        }

        public Ellipse_INFO(double m_Row_center, double m_Colum_center, double m_Phi, double m_Radius1, double m_Radius2)
        {
            this.CenterY = m_Row_center;
            this.CenterX = m_Colum_center;
            this.Phi = m_Phi;
            this.Radius1 = m_Radius1;
            this.Radius2 = m_Radius2;
        }

        public override HRegion GenRegion()
        {
            HRegion h = new HRegion();
            h.GenEllipse(CenterY, CenterX, Phi, Radius1, Radius2);
            return h;
        }

        public override HXLDCont GenXld()
        {
            HXLDCont xld = new HXLDCont();
            xld.GenEllipseContourXld(CenterY, CenterX, Phi, Radius1, Radius2, StartPhi, EndPhi, PointOrder, 1.0);
            return xld;
        }

        public override HTuple GetTuple()
        {
            double[] ellipse = new double[] { CenterY, CenterX, Phi, Radius1, Radius2 };
            return ellipse;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }



    /// <summary>
    /// 添加自定义形状
    /// </summary>
    [Serializable]
    public class UserDefinedShape_INFO : ROI
    {
        HRegion mHRegion;
        public UserDefinedShape_INFO()
        {

        }
        public UserDefinedShape_INFO(HRegion hRegion)
        {
            mHRegion = hRegion;
        }
        public override HRegion GenRegion()
        {
            return mHRegion;
        }

        public override HXLDCont GenXld()
        {
            if (mHRegion != null && mHRegion.IsInitialized())
            {
                return mHRegion.GenContourRegionXld("border_holes");
            }
            else
            {
                return new HXLDCont();
            }
        }

        public override HTuple GetTuple()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 矩形信息
    /// </summary>
    [Serializable]
    public class Rectangle_INFO : ROI
    {
        public double StartY, StartX, EndY, EndX;

        public Rectangle_INFO()
        {

        }

        public Rectangle_INFO(double m_Row_Start, double m_Column_Start, double m_Row_End, double m_Column_End)
        {
            this.StartY = m_Row_Start;
            this.StartX = m_Column_Start;
            this.EndY = m_Row_End;
            this.EndX = m_Column_End;
        }

        public override HRegion GenRegion()
        {
            HRegion h = new HRegion();

            h.GenRectangle1(StartY, StartX, EndY, EndX);
            //h.GenRectangle1(StartX, StartY, EndX, EndY);

            return h;
        }

        public override HXLDCont GenXld()
        {
            HXLDCont xld = new HXLDCont();
            HTuple row = new HTuple(StartY, EndY, EndY, StartY, StartY);
            HTuple col = new HTuple(StartX, StartX, EndX, EndX, StartX);
            xld.GenContourPolygonXld(row, col);
            return xld;
        }

        public override HTuple GetTuple()
        {
            double[] rect1 = new double[] { StartY, StartX, EndY, EndX };
            return new HTuple(rect1);
        }
    }

    /// <summary>
    /// 旋转矩形信息
    /// </summary>
    [Serializable]
    public class Rectangle2_INFO : ROI, ICloneable
    {

        public double CenterY, CenterX, Phi, Length1, Length2;

        public Rectangle2_INFO()
        {
        }

        public Rectangle2_INFO(double m_Row_center, double m_Column_center, double m_Phi, double m_Length1, double m_Length2)
        {
            this.CenterY = m_Row_center;
            this.CenterX = m_Column_center;
            this.Phi = m_Phi;
            this.Length1 = m_Length1;
            this.Length2 = m_Length2;
        }

        public override HRegion GenRegion()
        {
            HRegion h = new HRegion();
            h.GenRectangle2(CenterY, CenterX, Phi, Length1, Length2);
            return h;
        }

        public override HXLDCont GenXld()
        {
            HXLDCont xld = new HXLDCont();
            xld.GenRectangle2ContourXld(CenterY, CenterX, Phi, Length1, Length2);
            return xld;
        }

        public override HTuple GetTuple()
        {
            double[] rect2 = new double[] { CenterY, CenterX, Phi, Length1, Length2 };
            return new HTuple(rect2);
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }


    public class Cross_INFO : ROI, ICloneable
    {
        public double X, Y, Phi, Size;



        public Cross_INFO()
        {

        }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Phi"></param>
        /// <param name="Size"></param>
        public Cross_INFO(double X, double Y, double Size, double Phi)
        {
            this.X = X;
            this.Y = Y;
            this.Phi = Phi;
            this.Size = Size;
        }
        public override HRegion GenRegion()
        {
            throw new NotImplementedException();
        }

        public HObject GenXld2()
        {

            HObject ho_Cross;
            HOperatorSet.GenCrossContourXld(out ho_Cross, Y, X, Size, Phi);
            return ho_Cross;
        }

        public override HXLDCont GenXld()
        {
            throw new NotImplementedException();

        }

        public override HTuple GetTuple()
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

    }


    /// <summary>
    /// 矩形阵列返回的信息
    /// </summary>
    [Serializable]
    public struct RectInfo
    {

    }

    /// <summary>
    /// 十字坐标信息
    /// </summary>
    [Serializable]
    public struct Coordinate_INFO
    {
        public double Y, X, Phi;
        public Coordinate_INFO(double _row, double _col, double _phi)
        {
            this.Y = _row;
            this.X = _col;
            this.Phi = _phi;//坐标系X轴与图像X轴正方向的夹角
        }


    }

    /// <summary>
    /// 测量信息
    /// </summary>
    [Serializable]
    public struct Metrology_INFO
    {

        public double Length1 ;
        public double Length2;
        public double Threshold;
        public double MeasureDis;

        public HTuple ParamName;
        public HTuple ParamValue;

        public int PointsOrder;

        public Metrology_INFO(double _length1, double _length2, double _threshold, double _measureDis, HTuple _paraName, HTuple _paraValue, int _pointsOrder)
        {
            this.Length1 = _length1;                        // 长/2
            this.Length2 = _length2;                        // 宽/2
            this.Threshold = _threshold;                    // 阈值
            this.MeasureDis = _measureDis;                  //间隔
            this.ParamName = _paraName;                     //参数名
            this.ParamValue = _paraValue;                   //参数值
            this.PointsOrder = _pointsOrder;                //点顺序 0位默认，1 顺时针，2 逆时针
        }
    }

}