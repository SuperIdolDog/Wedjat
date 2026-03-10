// 版权所有(C) ChoiceTech Corporation。保留所有权利。
// 此代码的发布遵从
// ChoiceTech 公共许可(HY-PL，http://choicetech.cn/hy-pl.html)的条款。
//
//版权所有(C) ChoiceTech Corporation。保留所有权利。

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HalconDotNet;
using System.Diagnostics;

namespace HalconControl
{

    /// <summary>
    /// halcon鼠标缩放控件
    /// 
    /// 描述:
    ///      1, 必须首先通过this.HobjectToHimage(HObject hobject)传入图片,此图片称为"背景图"
    ///      2, 有了背景图,就可以通过本控件自定义的 this.DispObj(HObject hObj)显示HObject,类似原方法
    ///      3,默认显示红色,DispObj(HObject hObj,string color)可显示其他颜色
    /// </summary>

    public delegate void DelegateRePaint();

    public partial class HWindow_Final : UserControl
    {

        public event DelegateRePaint RePaint;

        private HWindow hv_window;//halcon窗体控件的句柄 this.mCtrl_HWindow.HalconWindow;
        private ContextMenuStrip hv_MenuStrip;//右键菜单控件

        // 窗体控件右键菜单内容
        ToolStripMenuItem fit_strip;//适应窗体
        ToolStripMenuItem barVisible_strip;//显示状态
        private ToolStripMenuItem blnCross_strip;//显示/隐藏十字
        ToolStripMenuItem saveImg_strip;
        ToolStripMenuItem saveWindow_strip;
        //ToolStripMenuItem histogram_strip;

        private HImage  /**/                 hv_image;                                        //缩放时操作的图片  此处千万不要使用hv_image = new HImage(),不然在生成控件dll的时候,会导致无法序列化,去你妈隔壁,还好老子有版本控制,不然都找不到这种恶心问题
        private int /**/                     hv_imageWidth, hv_imageHeight;                   //图片宽,高
        private string /**/                  str_imgSize;                                     //图片尺寸大小 5120X3840
        private bool    /**/                 drawModel = false;                                //绘制模式下,不允许缩放和鼠标右键菜单

        public ViewWindow.ViewWindow viewWindow;    /**/                                      //ViewWindow
        public HWindowControl hWindowControl;   /**/                                           // 当前halcon窗口

        private bool drawCrcess = false;//是否绘制十字线

        public static bool IsDesignMode()
        {
            bool returnFlag = false;
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                returnFlag = true;
            }
            else if (Process.GetCurrentProcess().ProcessName == "devenv")
            {
                returnFlag = true;
            }
            return returnFlag;
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        public HWindow_Final()
        {
            InitializeComponent();

            viewWindow = new ViewWindow.ViewWindow(mCtrl_HWindow);
            hWindowControl = this.mCtrl_HWindow;

            if (!IsDesignMode())
            {
                hv_window = this.mCtrl_HWindow.HalconWindow;
            }

            //              设定鼠标按下时图标的形状
            //              'arrow'  'default' 'crosshair' 'text I-beam' 'Slashed circle' 'Size All'
            //              'Size NESW' 'Size S' 'Size NWSE' 'Size WE' 'Vertical Arrow' 'Hourglass'
            //
            // hv_window.SetMshape("Hourglass");

            fit_strip = new ToolStripMenuItem("适应窗口");
            fit_strip.Click += new EventHandler((s, e) => DispImageFit(mCtrl_HWindow));

            blnCross_strip = new ToolStripMenuItem("显示/隐藏十字");
            blnCross_strip.Click += new EventHandler((s, e) => DispCross());

            barVisible_strip = new ToolStripMenuItem("显示StatusBar");
            barVisible_strip.CheckOnClick = true;
            barVisible_strip.CheckedChanged += new EventHandler(barVisible_strip_CheckedChanged);
            m_CtrlHStatusLabelCtrl.Visible = false;
            mCtrl_HWindow.Height = this.Height;

            saveImg_strip = new ToolStripMenuItem("保存原始图像");
            saveImg_strip.Click += new EventHandler((s, e) => SaveImage());

            saveWindow_strip = new ToolStripMenuItem("保存窗口缩略图");
            saveWindow_strip.Click += new EventHandler((s, e) => SaveWindowDump());

            //histogram_strip = new ToolStripMenuItem("显示直方图(H)");
            //histogram_strip.CheckOnClick = true;
            //histogram_strip.Checked = false;

            hv_MenuStrip = new ContextMenuStrip();
            hv_MenuStrip.Items.Add(fit_strip);
            hv_MenuStrip.Items.Add(blnCross_strip);
            hv_MenuStrip.Items.Add(barVisible_strip);
            hv_MenuStrip.Items.Add(new ToolStripSeparator());
            hv_MenuStrip.Items.Add(saveImg_strip);
            hv_MenuStrip.Items.Add(saveWindow_strip);

            fit_strip.Enabled = false;
            barVisible_strip.Enabled = true;
            blnCross_strip.Enabled = false;
            saveImg_strip.Enabled = false;
            saveWindow_strip.Enabled = false;

            mCtrl_HWindow.ContextMenuStrip = hv_MenuStrip;
            mCtrl_HWindow.SizeChanged += new EventHandler((s, e) => DispImageFit(mCtrl_HWindow));

        }

        /// <summary>
        /// 绘制模式下,不允许缩放和鼠标右键菜单
        /// </summary>
        public bool DrawModel
        {
            get { return drawModel; }
            set
            {
                //缩放控制
                viewWindow.setDrawModel(value);
                //绘制模式 不现实右键
                if (value == true)
                {
                    mCtrl_HWindow.ContextMenuStrip = null;
                }
                else
                {
                    //恢复
                    mCtrl_HWindow.ContextMenuStrip = hv_MenuStrip;
                }
                drawModel = value;
            }
        }

        /// <summary>
        /// 设置image,初始化控件参数
        /// </summary>
        public HImage Image
        {
            get
            {
                return this.hv_image;
            }
            set
            {
                if (value != null)
                {
                    if (this.hv_image != null)
                    {
                        this.hv_image.Dispose();
                    }
                    this.hv_image = value;
                    hv_image.GetImageSize(out hv_imageWidth, out hv_imageHeight);
                    str_imgSize = String.Format("W: {0} H: {1}", hv_imageWidth, hv_imageHeight);
                    try
                    {
                        fit_strip.Enabled = true;
                        barVisible_strip.Enabled = true;
                        blnCross_strip.Enabled = true;
                        saveImg_strip.Enabled = true;
                        saveWindow_strip.Enabled = true;
                    }
                    catch (Exception)
                    {
                    }
                    viewWindow.displayImage(hv_image);
                }
            }
        }

        /// <summary>
        /// 获得halcon窗体控件的句柄
        /// </summary>
        public IntPtr HWindowHalconID
        {
            get { return this.mCtrl_HWindow.HalconWindow; }
        }

        /// <summary>
        /// 获得控件
        /// </summary>
        /// <returns></returns>
        public HWindowControl getHWindowControl()
        {
            return this.mCtrl_HWindow;
        }

        /// <summary>
        /// 状态条 显示/隐藏 CheckedChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void barVisible_strip_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem strip = sender as ToolStripMenuItem;
            this.SuspendLayout();
            if (strip.Checked)
            {
                m_CtrlHStatusLabelCtrl.Visible = true;
                //mCtrl_HWindow.Height = this.Height - m_CtrlHStatusLabelCtrl.Height - m_CtrlHStatusLabelCtrl.Margin.Top - m_CtrlHStatusLabelCtrl.Margin.Bottom;
                mCtrl_HWindow.HMouseMove += HWindowControl_HMouseMove;
            }
            else
            {
                m_CtrlHStatusLabelCtrl.Visible = false;
                //mCtrl_HWindow.Height = this.Height;
                mCtrl_HWindow.HMouseMove -= HWindowControl_HMouseMove;
            }
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// 保存窗体截图到本地
        /// </summary>
        private void SaveWindowDump()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG图像|*.png|所有文件|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (String.IsNullOrEmpty(sfd.FileName))
                    return;
                HOperatorSet.DumpWindow(this.mCtrl_HWindow.HalconWindow, "png best", sfd.FileName);
            }
        }

        /// <summary>
        /// 保存窗体截图到本地
        /// </summary>
        public void SaveWindowDump2(string FileName)
        {
            if (String.IsNullOrEmpty(FileName))
                return;
            HOperatorSet.DumpWindow(this.mCtrl_HWindow.HalconWindow, "png best",FileName);
        }

        /// <summary>
        /// 保存原始图片到本地
        /// </summary>
        private void SaveImage()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "BMP图像|*.bmp|所有文件|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (String.IsNullOrEmpty(sfd.FileName))
                {
                    return;
                }
                HOperatorSet.WriteImage(hv_image, "bmp", 0, sfd.FileName);
            }
        }

        /// <summary>
        /// 图片适应大小显示在窗体
        /// </summary>
        /// <param name="hw_Ctrl">halcon窗体控件</param>
        private void DispImageFit(HWindowControl hw_Ctrl)
        {
            try
            {
                this.viewWindow.resetWindowImage();
                if (RePaint != null)
                {
                    RePaint();
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 鼠标在空间窗体里滑动,显示鼠标所在位置的灰度值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWindowControl_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (hv_image != null && hv_image.IsInitialized())
            {
                try
                {
                    int button_state;
                    double positionX, positionY;
                    string str_value;
                    string str_position;
                    bool _isXOut = true, _isYOut = true;
                    HTuple channel_count;

                    HOperatorSet.CountChannels(hv_image, out channel_count);

                    hv_window.GetMpositionSubPix(out positionY, out positionX, out button_state);
                    str_position = String.Format("X: {0:0000.0}, Y: {1:0000.0}", positionX, positionY);

                    _isXOut = (positionX < 0 || positionX >= hv_imageWidth);
                    _isYOut = (positionY < 0 || positionY >= hv_imageHeight);

                    if (!_isXOut && !_isYOut)
                    {
                        if ((int)channel_count == 1)
                        {
                            double grayVal;
                            grayVal = hv_image.GetGrayval((int)positionY, (int)positionX);
                            str_value = String.Format("Gray: {0:000.0}", grayVal);
                        }
                        else if ((int)channel_count == 3)
                        {
                            double grayValRed, grayValGreen, grayValBlue;

                            HImage _RedChannel, _GreenChannel, _BlueChannel;

                            _RedChannel = hv_image.AccessChannel(1);
                            _GreenChannel = hv_image.AccessChannel(2);
                            _BlueChannel = hv_image.AccessChannel(3);

                            grayValRed = _RedChannel.GetGrayval((int)positionY, (int)positionX);
                            grayValGreen = _GreenChannel.GetGrayval((int)positionY, (int)positionX);
                            grayValBlue = _BlueChannel.GetGrayval((int)positionY, (int)positionX);

                            _RedChannel.Dispose();
                            _GreenChannel.Dispose();
                            _BlueChannel.Dispose();

                            str_value = String.Format("Gray: ({0:000.0}, {1:000.0}, {2:000.0})", grayValRed, grayValGreen, grayValBlue);
                        }
                        else
                        {
                            str_value = "";
                        }
                        //m_CtrlHStatusLabelCtrl.Location = new Point(0, 0);
                        //m_CtrlHStatusLabelCtrl.BackColor = Color.White;
                        m_CtrlHStatusLabelCtrl.Text = str_imgSize + " | " + str_position + " | " + str_value;
                    }
                }
                catch (Exception)
                {
                    //throw ex;
                    //不处理
                }
            }

        }

        public void ClearWindow()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    //this.hv_image = null;
                    m_CtrlHStatusLabelCtrl.Visible = false;
                    barVisible_strip.Enabled = false;
                    fit_strip.Enabled = false;
                    //histogram_strip.Enabled = false;
                    saveImg_strip.Enabled = false;
                    saveWindow_strip.Enabled = false;

                    mCtrl_HWindow.HalconWindow.ClearWindow();
                    viewWindow.ClearWindow();

                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Hobject转换为的临时Himage,显示背景图
        /// </summary>
        /// <param name="hobject">传递Hobject,必须为图像</param>
        public void HobjectToHimage(HObject hobject)
        {
            if (hobject == null || !hobject.IsInitialized())
            {
                ClearWindow();
                return;
            }
            this.Image = new HImage(hobject);
        }

        #region DispObj

        /// <summary>
        /// 默认红颜色显示
        /// </summary>
        /// <param name="hObj">传入的region.xld,image</param>
        public void DispObj(HObject hObj)
        {
            lock (this)
            {
                viewWindow.displayHobject(hObj, null);
            }
        }

        /// <summary>
        /// 重新开辟内存保存 防止被传入的HObject在其他地方dispose后,不能重现
        /// </summary>
        /// <param name="hObj">传入的region.xld,image</param>
        /// <param name="color">颜色</param>
        public void DispObj(HObject hObj, string color)
        {
            lock (this)
            {
                viewWindow.displayHobject(hObj, color);
            }
        }

        #endregion

        #region DispText

        /// <summary>
        /// 窗体显示的内容
        /// </summary>
        /// <param name="meg"></param>
        public void DispTxt(string Meg, string color, int fontSize)
        {
            lock (this)
            {
                viewWindow.displayTxt(Meg, color, fontSize);
            }
        }

        public void DispTxt(string Meg, int fontSize)
        {
            lock (this)
            {
                viewWindow.displayTxt(Meg, fontSize);
            }
        }
        public void DispTxt(string Meg)
        {
            lock (this)
            {
                viewWindow.displayTxt(Meg, null);
            }
        }

        public void DispTxt(string Meg, string color)
        {
            lock (this)
            {
                viewWindow.displayTxt(Meg, color);
            }
        }

        #endregion

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCtrl_HWindow_MouseLeave(object sender, EventArgs e)
        {
            //避免鼠标离开窗口,再返回的时候,图表随着鼠标移动
            viewWindow.mouseleave();
        }

        /// <summary>
        /// 显示或者隐藏十字
        /// </summary>
        public void DispCross()
        {
            try
            {
                drawCrcess = !drawCrcess;
                viewWindow.dispCross(drawCrcess);
                this.viewWindow.resetWindowImage();
            }
            catch (Exception) { }
        }

    }
}
