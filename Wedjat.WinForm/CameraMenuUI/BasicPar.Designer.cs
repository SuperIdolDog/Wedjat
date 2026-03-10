namespace Wedjat.WinForm.CameraMenuUI
{
    partial class BasicPar
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
            AntdUI.SliderMarkItem sliderMarkItem7 = new AntdUI.SliderMarkItem();
            AntdUI.SliderMarkItem sliderMarkItem8 = new AntdUI.SliderMarkItem();
            AntdUI.SliderMarkItem sliderMarkItem9 = new AntdUI.SliderMarkItem();
            AntdUI.SliderMarkItem sliderMarkItem10 = new AntdUI.SliderMarkItem();
            AntdUI.SliderMarkItem sliderMarkItem11 = new AntdUI.SliderMarkItem();
            AntdUI.SliderMarkItem sliderMarkItem12 = new AntdUI.SliderMarkItem();
            this.gridPanel_normalSetting = new AntdUI.GridPanel();
            this.panel_FrameRate = new AntdUI.Panel();
            this.slider_FrameRate = new AntdUI.Slider();
            this.label_FrameRate = new AntdUI.Label();
            this.panel_Exposure = new AntdUI.Panel();
            this.slider_Exposure = new AntdUI.Slider();
            this.label_Exposure = new AntdUI.Label();
            this.panel_Gain = new AntdUI.Panel();
            this.slider_Gain = new AntdUI.Slider();
            this.label_Gain = new AntdUI.Label();
            this.panel_Resolution = new AntdUI.Panel();
            this.select_Resolution = new AntdUI.Select();
            this.label_Resolution = new AntdUI.Label();
            this.gridPanel_normalSetting.SuspendLayout();
            this.panel_FrameRate.SuspendLayout();
            this.panel_Exposure.SuspendLayout();
            this.panel_Gain.SuspendLayout();
            this.panel_Resolution.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridPanel_normalSetting
            // 
            this.gridPanel_normalSetting.BackColor = System.Drawing.Color.Transparent;
            this.gridPanel_normalSetting.Controls.Add(this.panel_FrameRate);
            this.gridPanel_normalSetting.Controls.Add(this.panel_Exposure);
            this.gridPanel_normalSetting.Controls.Add(this.panel_Gain);
            this.gridPanel_normalSetting.Controls.Add(this.panel_Resolution);
            this.gridPanel_normalSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel_normalSetting.Location = new System.Drawing.Point(0, 0);
            this.gridPanel_normalSetting.Name = "gridPanel_normalSetting";
            this.gridPanel_normalSetting.Size = new System.Drawing.Size(470, 760);
            this.gridPanel_normalSetting.Span = "\r\n10%:50% 50%;\r\n10%:50% 50%\r\n";
            this.gridPanel_normalSetting.TabIndex = 1;
            this.gridPanel_normalSetting.Text = "gridPanel1";
            // 
            // panel_FrameRate
            // 
            this.panel_FrameRate.Controls.Add(this.slider_FrameRate);
            this.panel_FrameRate.Controls.Add(this.label_FrameRate);
            this.panel_FrameRate.Location = new System.Drawing.Point(238, 79);
            this.panel_FrameRate.Name = "panel_FrameRate";
            this.panel_FrameRate.Size = new System.Drawing.Size(229, 70);
            this.panel_FrameRate.TabIndex = 3;
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
            sliderMarkItem7.Fore = System.Drawing.Color.Black;
            sliderMarkItem7.Text = "16μs";
            sliderMarkItem7.Value = 16;
            sliderMarkItem8.Fore = System.Drawing.Color.Black;
            sliderMarkItem8.Text = "1sec";
            sliderMarkItem8.Value = 1000000;
            this.slider_FrameRate.Marks.Add(sliderMarkItem7);
            this.slider_FrameRate.Marks.Add(sliderMarkItem8);
            this.slider_FrameRate.MaxValue = 1000000;
            this.slider_FrameRate.MinValue = 16;
            this.slider_FrameRate.Name = "slider_FrameRate";
            this.slider_FrameRate.ShowValue = true;
            this.slider_FrameRate.Size = new System.Drawing.Size(154, 70);
            this.slider_FrameRate.TabIndex = 6;
            this.slider_FrameRate.Text = "slider1";
            // 
            // label_FrameRate
            // 
            this.label_FrameRate.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_FrameRate.Location = new System.Drawing.Point(0, 0);
            this.label_FrameRate.Margin = new System.Windows.Forms.Padding(0);
            this.label_FrameRate.Name = "label_FrameRate";
            this.label_FrameRate.Size = new System.Drawing.Size(75, 70);
            this.label_FrameRate.TabIndex = 5;
            this.label_FrameRate.Text = "帧率:";
            this.label_FrameRate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel_Exposure
            // 
            this.panel_Exposure.Controls.Add(this.slider_Exposure);
            this.panel_Exposure.Controls.Add(this.label_Exposure);
            this.panel_Exposure.Location = new System.Drawing.Point(3, 79);
            this.panel_Exposure.Name = "panel_Exposure";
            this.panel_Exposure.Size = new System.Drawing.Size(229, 70);
            this.panel_Exposure.TabIndex = 2;
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
            sliderMarkItem9.Fore = System.Drawing.Color.Black;
            sliderMarkItem9.Text = "16μs";
            sliderMarkItem9.Value = 16;
            sliderMarkItem10.Fore = System.Drawing.Color.Black;
            sliderMarkItem10.Text = "1sec";
            sliderMarkItem10.Value = 1000000;
            this.slider_Exposure.Marks.Add(sliderMarkItem9);
            this.slider_Exposure.Marks.Add(sliderMarkItem10);
            this.slider_Exposure.MaxValue = 1000000;
            this.slider_Exposure.MinValue = 16;
            this.slider_Exposure.Name = "slider_Exposure";
            this.slider_Exposure.ShowValue = true;
            this.slider_Exposure.Size = new System.Drawing.Size(141, 70);
            this.slider_Exposure.TabIndex = 4;
            this.slider_Exposure.Text = "slider1";
            // 
            // label_Exposure
            // 
            this.label_Exposure.BackColor = System.Drawing.Color.Transparent;
            this.label_Exposure.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_Exposure.Location = new System.Drawing.Point(0, 0);
            this.label_Exposure.Margin = new System.Windows.Forms.Padding(0);
            this.label_Exposure.Name = "label_Exposure";
            this.label_Exposure.Size = new System.Drawing.Size(88, 70);
            this.label_Exposure.TabIndex = 3;
            this.label_Exposure.Text = "曝光时间:";
            this.label_Exposure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel_Gain
            // 
            this.panel_Gain.Controls.Add(this.slider_Gain);
            this.panel_Gain.Controls.Add(this.label_Gain);
            this.panel_Gain.Location = new System.Drawing.Point(238, 3);
            this.panel_Gain.Name = "panel_Gain";
            this.panel_Gain.Size = new System.Drawing.Size(229, 70);
            this.panel_Gain.TabIndex = 1;
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
            sliderMarkItem11.Fore = System.Drawing.Color.Black;
            sliderMarkItem11.Text = "0";
            sliderMarkItem12.Fore = System.Drawing.Color.Black;
            sliderMarkItem12.Text = "15dB";
            sliderMarkItem12.Value = 15;
            this.slider_Gain.Marks.Add(sliderMarkItem11);
            this.slider_Gain.Marks.Add(sliderMarkItem12);
            this.slider_Gain.MaxValue = 15;
            this.slider_Gain.Name = "slider_Gain";
            this.slider_Gain.ShowValue = true;
            this.slider_Gain.Size = new System.Drawing.Size(154, 70);
            this.slider_Gain.TabIndex = 2;
            this.slider_Gain.Text = "slider1";
            // 
            // label_Gain
            // 
            this.label_Gain.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_Gain.Location = new System.Drawing.Point(0, 0);
            this.label_Gain.Margin = new System.Windows.Forms.Padding(0);
            this.label_Gain.Name = "label_Gain";
            this.label_Gain.Size = new System.Drawing.Size(75, 70);
            this.label_Gain.TabIndex = 1;
            this.label_Gain.Text = "增益:";
            this.label_Gain.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel_Resolution
            // 
            this.panel_Resolution.BackColor = System.Drawing.Color.Transparent;
            this.panel_Resolution.Controls.Add(this.select_Resolution);
            this.panel_Resolution.Controls.Add(this.label_Resolution);
            this.panel_Resolution.Location = new System.Drawing.Point(3, 3);
            this.panel_Resolution.Name = "panel_Resolution";
            this.panel_Resolution.Size = new System.Drawing.Size(229, 70);
            this.panel_Resolution.TabIndex = 0;
            this.panel_Resolution.Text = "panel1";
            // 
            // select_Resolution
            // 
            this.select_Resolution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.select_Resolution.Items.AddRange(new object[] {
            "RGB 8",
            "Mono 8"});
            this.select_Resolution.Location = new System.Drawing.Point(88, 0);
            this.select_Resolution.Margin = new System.Windows.Forms.Padding(0);
            this.select_Resolution.Name = "select_Resolution";
            this.select_Resolution.SelectedIndex = 0;
            this.select_Resolution.SelectedValue = "RGB 8";
            this.select_Resolution.Size = new System.Drawing.Size(141, 70);
            this.select_Resolution.TabIndex = 1;
            this.select_Resolution.Text = "RGB 8";
            // 
            // label_Resolution
            // 
            this.label_Resolution.BackColor = System.Drawing.Color.Transparent;
            this.label_Resolution.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_Resolution.Location = new System.Drawing.Point(0, 0);
            this.label_Resolution.Margin = new System.Windows.Forms.Padding(0);
            this.label_Resolution.Name = "label_Resolution";
            this.label_Resolution.Size = new System.Drawing.Size(88, 70);
            this.label_Resolution.TabIndex = 0;
            this.label_Resolution.Text = "像素格式:";
            this.label_Resolution.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BasicPar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridPanel_normalSetting);
            this.Name = "BasicPar";
            this.Size = new System.Drawing.Size(470, 760);
            this.gridPanel_normalSetting.ResumeLayout(false);
            this.panel_FrameRate.ResumeLayout(false);
            this.panel_Exposure.ResumeLayout(false);
            this.panel_Gain.ResumeLayout(false);
            this.panel_Resolution.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.GridPanel gridPanel_normalSetting;
        private AntdUI.Panel panel_FrameRate;
        private AntdUI.Slider slider_FrameRate;
        private AntdUI.Label label_FrameRate;
        private AntdUI.Panel panel_Exposure;
        private AntdUI.Slider slider_Exposure;
        private AntdUI.Label label_Exposure;
        private AntdUI.Panel panel_Gain;
        private AntdUI.Slider slider_Gain;
        private AntdUI.Label label_Gain;
        private AntdUI.Panel panel_Resolution;
        private AntdUI.Select select_Resolution;
        private AntdUI.Label label_Resolution;
    }
}
