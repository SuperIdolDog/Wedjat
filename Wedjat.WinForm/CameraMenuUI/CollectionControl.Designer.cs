namespace Wedjat.WinForm.CameraMenuUI
{
    partial class CollectionControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            AntdUI.SliderMarkItem sliderMarkItem1 = new AntdUI.SliderMarkItem();
            AntdUI.SliderMarkItem sliderMarkItem2 = new AntdUI.SliderMarkItem();
            AntdUI.SliderMarkItem sliderMarkItem3 = new AntdUI.SliderMarkItem();
            AntdUI.SliderMarkItem sliderMarkItem4 = new AntdUI.SliderMarkItem();
            AntdUI.SliderMarkItem sliderMarkItem5 = new AntdUI.SliderMarkItem();
            AntdUI.SliderMarkItem sliderMarkItem6 = new AntdUI.SliderMarkItem();
            this.gridPanel_main = new AntdUI.GridPanel();
            this.panel_FrameRate = new AntdUI.Panel();
            this.slider_FrameRate = new AntdUI.Slider();
            this.label_FrameRate = new AntdUI.Label();
            this.panel_Gain = new AntdUI.Panel();
            this.slider_Gain = new AntdUI.Slider();
            this.label_Gain = new AntdUI.Label();
            this.panel_Exposure = new AntdUI.Panel();
            this.slider_Exposure = new AntdUI.Slider();
            this.label_Exposure = new AntdUI.Label();
            this.panel_Resolution = new AntdUI.Panel();
            this.select_Resolution = new AntdUI.Select();
            this.label_Resolution = new AntdUI.Label();
            this.btn_StopGrab = new AntdUI.Button();
            this.btn_StartGrab = new AntdUI.Button();
            this.btn_Importimage = new AntdUI.Button();
            this.btn_ImportModel = new AntdUI.Button();
            this.select_Model = new AntdUI.Select();
            this.button_TriggerExec = new AntdUI.Button();
            this.checkbox_hardTri = new AntdUI.Checkbox();
            this.checkbox_softTri = new AntdUI.Checkbox();
            this.label2 = new AntdUI.Label();
            this.radio_TriggerMode = new AntdUI.Radio();
            this.radio_ContinuesMode = new AntdUI.Radio();
            this.label1 = new AntdUI.Label();
            this.gridPanel_main.SuspendLayout();
            this.panel_FrameRate.SuspendLayout();
            this.panel_Gain.SuspendLayout();
            this.panel_Exposure.SuspendLayout();
            this.panel_Resolution.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridPanel_main
            // 
            this.gridPanel_main.BackColor = System.Drawing.Color.Transparent;
            this.gridPanel_main.Controls.Add(this.panel_FrameRate);
            this.gridPanel_main.Controls.Add(this.panel_Gain);
            this.gridPanel_main.Controls.Add(this.panel_Exposure);
            this.gridPanel_main.Controls.Add(this.panel_Resolution);
            this.gridPanel_main.Controls.Add(this.btn_StopGrab);
            this.gridPanel_main.Controls.Add(this.btn_StartGrab);
            this.gridPanel_main.Controls.Add(this.btn_Importimage);
            this.gridPanel_main.Controls.Add(this.btn_ImportModel);
            this.gridPanel_main.Controls.Add(this.select_Model);
            this.gridPanel_main.Controls.Add(this.button_TriggerExec);
            this.gridPanel_main.Controls.Add(this.checkbox_hardTri);
            this.gridPanel_main.Controls.Add(this.checkbox_softTri);
            this.gridPanel_main.Controls.Add(this.label2);
            this.gridPanel_main.Controls.Add(this.radio_TriggerMode);
            this.gridPanel_main.Controls.Add(this.radio_ContinuesMode);
            this.gridPanel_main.Controls.Add(this.label1);
            this.gridPanel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel_main.Gap = 2;
            this.gridPanel_main.Location = new System.Drawing.Point(0, 0);
            this.gridPanel_main.Name = "gridPanel_main";
            this.gridPanel_main.Size = new System.Drawing.Size(470, 760);
            this.gridPanel_main.Span = "\r\n5%:100%;\r\n10%:50% 50%;\r\n5%:100%;\r\n10%:33% 33% 33%;\r\n10%:50% 25% 25%;\r\n15%:50% 5" +
    "0%;\r\n10%:50% 50%;\r\n10%:50% 50%\r\n\r\n";
            this.gridPanel_main.TabIndex = 8;
            this.gridPanel_main.Text = "gridPanel1";
            // 
            // panel_FrameRate
            // 
            this.panel_FrameRate.Controls.Add(this.slider_FrameRate);
            this.panel_FrameRate.Controls.Add(this.label_FrameRate);
            this.panel_FrameRate.Location = new System.Drawing.Point(241, 500);
            this.panel_FrameRate.Name = "panel_FrameRate";
            this.panel_FrameRate.Size = new System.Drawing.Size(223, 64);
            this.panel_FrameRate.TabIndex = 46;
            this.panel_FrameRate.Text = "panel";
            // 
            // slider_FrameRate
            // 
            this.slider_FrameRate.BackColor = System.Drawing.Color.Transparent;
            this.slider_FrameRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slider_FrameRate.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(103)))), ((int)(((byte)(255)))));
            this.slider_FrameRate.FillActive = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(103)))), ((int)(((byte)(255)))));
            this.slider_FrameRate.FillHover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(103)))), ((int)(((byte)(255)))));
            this.slider_FrameRate.ForeColor = System.Drawing.Color.Black;
            this.slider_FrameRate.Location = new System.Drawing.Point(75, 0);
            sliderMarkItem1.Fore = System.Drawing.Color.Black;
            sliderMarkItem1.Text = "0fps";
            sliderMarkItem2.Fore = System.Drawing.Color.Black;
            sliderMarkItem2.Text = "60fps";
            sliderMarkItem2.Value = 60;
            this.slider_FrameRate.Marks.Add(sliderMarkItem1);
            this.slider_FrameRate.Marks.Add(sliderMarkItem2);
            this.slider_FrameRate.MaxValue = 60;
            this.slider_FrameRate.Name = "slider_FrameRate";
            this.slider_FrameRate.ShowValue = true;
            this.slider_FrameRate.Size = new System.Drawing.Size(148, 64);
            this.slider_FrameRate.TabIndex = 6;
            this.slider_FrameRate.Text = "slider1";
            this.slider_FrameRate.ValueChanged += new AntdUI.IntEventHandler(this.slider_FrameRate_ValueChanged);
            this.slider_FrameRate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.slider_FrameRate_MouseUp);
            // 
            // label_FrameRate
            // 
            this.label_FrameRate.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_FrameRate.Location = new System.Drawing.Point(0, 0);
            this.label_FrameRate.Margin = new System.Windows.Forms.Padding(0);
            this.label_FrameRate.Name = "label_FrameRate";
            this.label_FrameRate.Size = new System.Drawing.Size(75, 64);
            this.label_FrameRate.TabIndex = 5;
            this.label_FrameRate.Text = "帧率:";
            this.label_FrameRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel_Gain
            // 
            this.panel_Gain.Controls.Add(this.slider_Gain);
            this.panel_Gain.Controls.Add(this.label_Gain);
            this.panel_Gain.Location = new System.Drawing.Point(6, 500);
            this.panel_Gain.Name = "panel_Gain";
            this.panel_Gain.Size = new System.Drawing.Size(223, 64);
            this.panel_Gain.TabIndex = 45;
            this.panel_Gain.Text = "panel2";
            // 
            // slider_Gain
            // 
            this.slider_Gain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slider_Gain.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(103)))), ((int)(((byte)(255)))));
            this.slider_Gain.FillActive = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(103)))), ((int)(((byte)(255)))));
            this.slider_Gain.FillHover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(103)))), ((int)(((byte)(255)))));
            this.slider_Gain.ForeColor = System.Drawing.Color.Black;
            this.slider_Gain.Location = new System.Drawing.Point(75, 0);
            sliderMarkItem3.Fore = System.Drawing.Color.Black;
            sliderMarkItem3.Text = "0";
            sliderMarkItem4.Fore = System.Drawing.Color.Black;
            sliderMarkItem4.Text = "15dB";
            sliderMarkItem4.Value = 15;
            this.slider_Gain.Marks.Add(sliderMarkItem3);
            this.slider_Gain.Marks.Add(sliderMarkItem4);
            this.slider_Gain.MaxValue = 15;
            this.slider_Gain.Name = "slider_Gain";
            this.slider_Gain.ShowValue = true;
            this.slider_Gain.Size = new System.Drawing.Size(148, 64);
            this.slider_Gain.TabIndex = 2;
            this.slider_Gain.Text = "slider1";
            this.slider_Gain.ValueChanged += new AntdUI.IntEventHandler(this.slider_Gain_ValueChanged);
            this.slider_Gain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.slider_Gain_MouseUp);
            // 
            // label_Gain
            // 
            this.label_Gain.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_Gain.Location = new System.Drawing.Point(0, 0);
            this.label_Gain.Margin = new System.Windows.Forms.Padding(0);
            this.label_Gain.Name = "label_Gain";
            this.label_Gain.Size = new System.Drawing.Size(75, 64);
            this.label_Gain.TabIndex = 1;
            this.label_Gain.Text = "增益:";
            this.label_Gain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel_Exposure
            // 
            this.panel_Exposure.Controls.Add(this.slider_Exposure);
            this.panel_Exposure.Controls.Add(this.label_Exposure);
            this.panel_Exposure.Location = new System.Drawing.Point(241, 424);
            this.panel_Exposure.Name = "panel_Exposure";
            this.panel_Exposure.Size = new System.Drawing.Size(223, 64);
            this.panel_Exposure.TabIndex = 44;
            this.panel_Exposure.Text = "panel3";
            // 
            // slider_Exposure
            // 
            this.slider_Exposure.BackColor = System.Drawing.Color.Transparent;
            this.slider_Exposure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slider_Exposure.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(103)))), ((int)(((byte)(255)))));
            this.slider_Exposure.FillActive = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(103)))), ((int)(((byte)(255)))));
            this.slider_Exposure.FillHover = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(103)))), ((int)(((byte)(255)))));
            this.slider_Exposure.ForeColor = System.Drawing.Color.Black;
            this.slider_Exposure.Location = new System.Drawing.Point(88, 0);
            sliderMarkItem5.Fore = System.Drawing.Color.Black;
            sliderMarkItem5.Text = "20μs";
            sliderMarkItem5.Value = 20;
            sliderMarkItem6.Fore = System.Drawing.Color.Black;
            sliderMarkItem6.Text = "1s";
            sliderMarkItem6.Value = 100000;
            this.slider_Exposure.Marks.Add(sliderMarkItem5);
            this.slider_Exposure.Marks.Add(sliderMarkItem6);
            this.slider_Exposure.MaxValue = 100000;
            this.slider_Exposure.MinValue = 20;
            this.slider_Exposure.Name = "slider_Exposure";
            this.slider_Exposure.ShowValue = true;
            this.slider_Exposure.Size = new System.Drawing.Size(135, 64);
            this.slider_Exposure.TabIndex = 4;
            this.slider_Exposure.Text = "slider1";
            this.slider_Exposure.Value = 20;
            this.slider_Exposure.ValueChanged += new AntdUI.IntEventHandler(this.slider_Exposure_ValueChanged);
            this.slider_Exposure.MouseUp += new System.Windows.Forms.MouseEventHandler(this.slider_Exposure_MouseUp);
            // 
            // label_Exposure
            // 
            this.label_Exposure.BackColor = System.Drawing.Color.Transparent;
            this.label_Exposure.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_Exposure.Location = new System.Drawing.Point(0, 0);
            this.label_Exposure.Margin = new System.Windows.Forms.Padding(0);
            this.label_Exposure.Name = "label_Exposure";
            this.label_Exposure.Size = new System.Drawing.Size(88, 64);
            this.label_Exposure.TabIndex = 3;
            this.label_Exposure.Text = "曝光:";
            this.label_Exposure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel_Resolution
            // 
            this.panel_Resolution.BackColor = System.Drawing.Color.Transparent;
            this.panel_Resolution.Controls.Add(this.select_Resolution);
            this.panel_Resolution.Controls.Add(this.label_Resolution);
            this.panel_Resolution.Location = new System.Drawing.Point(6, 424);
            this.panel_Resolution.Name = "panel_Resolution";
            this.panel_Resolution.Size = new System.Drawing.Size(223, 64);
            this.panel_Resolution.TabIndex = 43;
            this.panel_Resolution.Text = "panel1";
            // 
            // select_Resolution
            // 
            this.select_Resolution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.select_Resolution.Location = new System.Drawing.Point(88, 0);
            this.select_Resolution.Margin = new System.Windows.Forms.Padding(0);
            this.select_Resolution.Name = "select_Resolution";
            this.select_Resolution.Size = new System.Drawing.Size(135, 64);
            this.select_Resolution.TabIndex = 1;
            this.select_Resolution.SelectedValueChanged += new AntdUI.ObjectNEventHandler(this.select_Resolution_SelectedValueChanged);
            // 
            // label_Resolution
            // 
            this.label_Resolution.BackColor = System.Drawing.Color.Transparent;
            this.label_Resolution.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_Resolution.Location = new System.Drawing.Point(0, 0);
            this.label_Resolution.Margin = new System.Windows.Forms.Padding(0);
            this.label_Resolution.Name = "label_Resolution";
            this.label_Resolution.Size = new System.Drawing.Size(88, 64);
            this.label_Resolution.TabIndex = 0;
            this.label_Resolution.Text = "像素格式:";
            this.label_Resolution.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_StopGrab
            // 
            this.btn_StopGrab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_StopGrab.Enabled = false;
            this.btn_StopGrab.Location = new System.Drawing.Point(241, 310);
            this.btn_StopGrab.Name = "btn_StopGrab";
            this.btn_StopGrab.Size = new System.Drawing.Size(223, 102);
            this.btn_StopGrab.TabIndex = 42;
            this.btn_StopGrab.Text = "停止采集";
            this.btn_StopGrab.Type = AntdUI.TTypeMini.Error;
            this.btn_StopGrab.Click += new System.EventHandler(this.btn_StopGrab_Click);
            // 
            // btn_StartGrab
            // 
            this.btn_StartGrab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_StartGrab.Enabled = false;
            this.btn_StartGrab.Location = new System.Drawing.Point(6, 310);
            this.btn_StartGrab.Name = "btn_StartGrab";
            this.btn_StartGrab.Size = new System.Drawing.Size(223, 102);
            this.btn_StartGrab.TabIndex = 41;
            this.btn_StartGrab.Text = "开始采集";
            this.btn_StartGrab.Type = AntdUI.TTypeMini.Success;
            this.btn_StartGrab.Click += new System.EventHandler(this.btn_StartGrab_Click);
            // 
            // btn_Importimage
            // 
            this.btn_Importimage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_Importimage.Location = new System.Drawing.Point(358, 234);
            this.btn_Importimage.Name = "btn_Importimage";
            this.btn_Importimage.Size = new System.Drawing.Size(106, 64);
            this.btn_Importimage.TabIndex = 39;
            this.btn_Importimage.Text = "导入图片";
            this.btn_Importimage.Type = AntdUI.TTypeMini.Info;
            this.btn_Importimage.Click += new System.EventHandler(this.btn_Importimage_Click);
            // 
            // btn_ImportModel
            // 
            this.btn_ImportModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_ImportModel.Location = new System.Drawing.Point(241, 234);
            this.btn_ImportModel.Name = "btn_ImportModel";
            this.btn_ImportModel.Size = new System.Drawing.Size(106, 64);
            this.btn_ImportModel.TabIndex = 36;
            this.btn_ImportModel.Text = "导入模板";
            this.btn_ImportModel.Type = AntdUI.TTypeMini.Warn;
            this.btn_ImportModel.Click += new System.EventHandler(this.btn_ImportModel_Click);
            // 
            // select_Model
            // 
            this.select_Model.Dock = System.Windows.Forms.DockStyle.Fill;
            this.select_Model.Location = new System.Drawing.Point(6, 234);
            this.select_Model.Name = "select_Model";
            this.select_Model.PrefixText = "模板:";
            this.select_Model.Size = new System.Drawing.Size(223, 64);
            this.select_Model.TabIndex = 33;
            this.select_Model.SelectedIndexChanged += new AntdUI.IntEventHandler(this.select_Model_SelectedIndexChanged);
            // 
            // button_TriggerExec
            // 
            this.button_TriggerExec.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button_TriggerExec.Enabled = false;
            this.button_TriggerExec.Location = new System.Drawing.Point(316, 158);
            this.button_TriggerExec.Name = "button_TriggerExec";
            this.button_TriggerExec.Size = new System.Drawing.Size(143, 64);
            this.button_TriggerExec.TabIndex = 30;
            this.button_TriggerExec.Text = "单次触发";
            this.button_TriggerExec.Type = AntdUI.TTypeMini.Primary;
            this.button_TriggerExec.Click += new System.EventHandler(this.button_TriggerExec_Click);
            // 
            // checkbox_hardTri
            // 
            this.checkbox_hardTri.BackColor = System.Drawing.Color.Transparent;
            this.checkbox_hardTri.Checked = true;
            this.checkbox_hardTri.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkbox_hardTri.Location = new System.Drawing.Point(158, 155);
            this.checkbox_hardTri.Margin = new System.Windows.Forms.Padding(0);
            this.checkbox_hardTri.Name = "checkbox_hardTri";
            this.checkbox_hardTri.Size = new System.Drawing.Size(149, 70);
            this.checkbox_hardTri.TabIndex = 28;
            this.checkbox_hardTri.Text = "硬件触发";
            this.checkbox_hardTri.CheckedChanged += new AntdUI.BoolEventHandler(this.checkbox_TriggerType_CheckedChanged);
            // 
            // checkbox_softTri
            // 
            this.checkbox_softTri.BackColor = System.Drawing.Color.Transparent;
            this.checkbox_softTri.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkbox_softTri.Location = new System.Drawing.Point(3, 155);
            this.checkbox_softTri.Margin = new System.Windows.Forms.Padding(0);
            this.checkbox_softTri.Name = "checkbox_softTri";
            this.checkbox_softTri.Size = new System.Drawing.Size(149, 70);
            this.checkbox_softTri.TabIndex = 2;
            this.checkbox_softTri.Text = "软件触发";
            this.checkbox_softTri.CheckedChanged += new AntdUI.BoolEventHandler(this.checkbox_TriggerType_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(6, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(458, 26);
            this.label2.TabIndex = 18;
            this.label2.Text = "触发方式";
            // 
            // radio_TriggerMode
            // 
            this.radio_TriggerMode.Checked = true;
            this.radio_TriggerMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radio_TriggerMode.Location = new System.Drawing.Point(241, 44);
            this.radio_TriggerMode.Name = "radio_TriggerMode";
            this.radio_TriggerMode.Size = new System.Drawing.Size(223, 64);
            this.radio_TriggerMode.TabIndex = 12;
            this.radio_TriggerMode.Text = "触发采集";
            this.radio_TriggerMode.CheckedChanged += new AntdUI.BoolEventHandler(this.radio_Mode_CheckedChanged);
            // 
            // radio_ContinuesMode
            // 
            this.radio_ContinuesMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radio_ContinuesMode.Location = new System.Drawing.Point(6, 44);
            this.radio_ContinuesMode.Name = "radio_ContinuesMode";
            this.radio_ContinuesMode.Size = new System.Drawing.Size(223, 64);
            this.radio_ContinuesMode.TabIndex = 11;
            this.radio_ContinuesMode.Text = "自动采集";
            this.radio_ContinuesMode.CheckedChanged += new AntdUI.BoolEventHandler(this.radio_Mode_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(458, 26);
            this.label1.TabIndex = 9;
            this.label1.Text = "采集模式";
            // 
            // CollectionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridPanel_main);
            this.Name = "CollectionControl";
            this.Size = new System.Drawing.Size(470, 760);
            this.Load += new System.EventHandler(this.CollectionControl_Load);
            this.gridPanel_main.ResumeLayout(false);
            this.panel_FrameRate.ResumeLayout(false);
            this.panel_Gain.ResumeLayout(false);
            this.panel_Exposure.ResumeLayout(false);
            this.panel_Resolution.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.GridPanel gridPanel_main;
        private AntdUI.Button btn_StopGrab;
        private AntdUI.Button btn_StartGrab;
        private AntdUI.Button btn_Importimage;
        private AntdUI.Button btn_ImportModel;
        private AntdUI.Select select_Model;
        private AntdUI.Button button_TriggerExec;
        private AntdUI.Checkbox checkbox_hardTri;
        private AntdUI.Checkbox checkbox_softTri;
        private AntdUI.Label label2;
        private AntdUI.Radio radio_TriggerMode;
        private AntdUI.Radio radio_ContinuesMode;
        private AntdUI.Label label1;
        private AntdUI.Panel panel_Resolution;
        private AntdUI.Select select_Resolution;
        private AntdUI.Label label_Resolution;
        private AntdUI.Panel panel_Exposure;
        private AntdUI.Slider slider_Exposure;
        private AntdUI.Label label_Exposure;
        private AntdUI.Panel panel_Gain;
        private AntdUI.Slider slider_Gain;
        private AntdUI.Label label_Gain;
        private AntdUI.Panel panel_FrameRate;
        private AntdUI.Slider slider_FrameRate;
        private AntdUI.Label label_FrameRate;
    }
}
