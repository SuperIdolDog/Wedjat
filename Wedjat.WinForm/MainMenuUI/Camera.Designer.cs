namespace Wedjat.WinForm.MainMenuUI
{
    partial class Camera
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
            AntdUI.Tabs.StyleCard styleCard1 = new AntdUI.Tabs.StyleCard();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Camera));
            AntdUI.MenuItem menuItem1 = new AntdUI.MenuItem();
            AntdUI.MenuItem menuItem2 = new AntdUI.MenuItem();
            AntdUI.MenuItem menuItem3 = new AntdUI.MenuItem();
            this.splitter_main = new AntdUI.Splitter();
            this.gridPanel1 = new AntdUI.GridPanel();
            this.tabs_info = new AntdUI.Tabs();
            this.tabPage1 = new AntdUI.TabPage();
            this.panel_DetectionLog = new AntdUI.Panel();
            this.table_Log = new AntdUI.Table();
            this.tabPage2 = new AntdUI.TabPage();
            this.tabPage3 = new AntdUI.TabPage();
            this.menu_camera = new AntdUI.Menu();
            this.hWindow_Final1 = new HalconControl.HWindow_Final();
            this.panel1 = new AntdUI.Panel();
            this.switch_camera = new AntdUI.Switch();
            this.button_searchCamera = new AntdUI.Button();
            this.select_DeviceSerialNumber = new AntdUI.Select();
            this.label_DeviceSerialNumber = new AntdUI.Label();
            this.gridPanel_tool = new AntdUI.GridPanel();
            this.panel_CameraMenu = new AntdUI.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitter_main)).BeginInit();
            this.splitter_main.Panel1.SuspendLayout();
            this.splitter_main.Panel2.SuspendLayout();
            this.splitter_main.SuspendLayout();
            this.gridPanel1.SuspendLayout();
            this.tabs_info.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel_DetectionLog.SuspendLayout();
            this.gridPanel_tool.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter_main
            // 
            this.splitter_main.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitter_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitter_main.Location = new System.Drawing.Point(0, 0);
            this.splitter_main.Name = "splitter_main";
            // 
            // splitter_main.Panel1
            // 
            this.splitter_main.Panel1.Controls.Add(this.gridPanel1);
            // 
            // splitter_main.Panel2
            // 
            this.splitter_main.Panel2.Controls.Add(this.gridPanel_tool);
            this.splitter_main.Size = new System.Drawing.Size(1440, 760);
            this.splitter_main.SplitterDistance = 966;
            this.splitter_main.TabIndex = 0;
            // 
            // gridPanel1
            // 
            this.gridPanel1.Controls.Add(this.tabs_info);
            this.gridPanel1.Controls.Add(this.menu_camera);
            this.gridPanel1.Controls.Add(this.hWindow_Final1);
            this.gridPanel1.Controls.Add(this.panel1);
            this.gridPanel1.Controls.Add(this.switch_camera);
            this.gridPanel1.Controls.Add(this.button_searchCamera);
            this.gridPanel1.Controls.Add(this.select_DeviceSerialNumber);
            this.gridPanel1.Controls.Add(this.label_DeviceSerialNumber);
            this.gridPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel1.Location = new System.Drawing.Point(0, 0);
            this.gridPanel1.Name = "gridPanel1";
            this.gridPanel1.Size = new System.Drawing.Size(966, 760);
            this.gridPanel1.Span = "5%:15% 20% 10% 15% 40%;\r\n65%:80% 20%; \r\n30%:100%";
            this.gridPanel1.TabIndex = 1;
            this.gridPanel1.Text = "gridPanel1";
            // 
            // tabs_info
            // 
            this.tabs_info.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabs_info.BackColor = System.Drawing.Color.Transparent;
            this.tabs_info.Controls.Add(this.tabPage1);
            this.tabs_info.Controls.Add(this.tabPage2);
            this.tabs_info.Controls.Add(this.tabPage3);
            this.tabs_info.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabs_info.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs_info.DragOrder = true;
            this.tabs_info.Fill = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.tabs_info.FillActive = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.tabs_info.FillHover = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(119)))), ((int)(((byte)(255)))));
            this.tabs_info.IconRatio = 1F;
            this.tabs_info.Location = new System.Drawing.Point(3, 535);
            this.tabs_info.Name = "tabs_info";
            this.tabs_info.Pages.Add(this.tabPage1);
            this.tabs_info.Pages.Add(this.tabPage2);
            this.tabs_info.Pages.Add(this.tabPage3);
            this.tabs_info.Size = new System.Drawing.Size(960, 222);
            this.tabs_info.Style = styleCard1;
            this.tabs_info.TabIndex = 13;
            this.tabs_info.Text = "tabs1";
            this.tabs_info.Type = AntdUI.TabType.Card;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel_DetectionLog);
            this.tabPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPage1.IconSvg = resources.GetString("tabPage1.IconSvg");
            this.tabPage1.Location = new System.Drawing.Point(0, 0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(960, 186);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "检测日志";
            // 
            // panel_DetectionLog
            // 
            this.panel_DetectionLog.Controls.Add(this.table_Log);
            this.panel_DetectionLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_DetectionLog.Location = new System.Drawing.Point(0, 0);
            this.panel_DetectionLog.Name = "panel_DetectionLog";
            this.panel_DetectionLog.Size = new System.Drawing.Size(960, 186);
            this.panel_DetectionLog.TabIndex = 8;
            this.panel_DetectionLog.Text = "panel3";
            // 
            // table_Log
            // 
            this.table_Log.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table_Log.Gap = 12;
            this.table_Log.Location = new System.Drawing.Point(0, 0);
            this.table_Log.Name = "table_Log";
            this.table_Log.Size = new System.Drawing.Size(960, 186);
            this.table_Log.TabIndex = 0;
            this.table_Log.Text = "table1";
            // 
            // tabPage2
            // 
            this.tabPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPage2.IconSvg = resources.GetString("tabPage2.IconSvg");
            this.tabPage2.Location = new System.Drawing.Point(0, 0);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(960, 186);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "缺陷统计";
            // 
            // tabPage3
            // 
            this.tabPage3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPage3.IconSvg = resources.GetString("tabPage3.IconSvg");
            this.tabPage3.Location = new System.Drawing.Point(0, 0);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(960, 186);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "检测图片";
            // 
            // menu_camera
            // 
            this.menu_camera.BackActive = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(58)))), ((int)(((byte)(183)))));
            this.menu_camera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menu_camera.ForeActive = System.Drawing.Color.White;
            this.menu_camera.Gap = 20;
            menuItem1.IconSvg = resources.GetString("menuItem1.IconSvg");
            menuItem1.Name = "CollectionControl";
            menuItem1.Select = true;
            menuItem1.Text = "采集控制";
            menuItem2.IconSvg = resources.GetString("menuItem2.IconSvg");
            menuItem2.Name = "AdvancedParameters";
            menuItem2.Text = "高级参数";
            menuItem3.IconSvg = resources.GetString("menuItem3.IconSvg");
            menuItem3.Name = "AlgorithmModule";
            menuItem3.Text = "算法模块";
            this.menu_camera.Items.Add(menuItem1);
            this.menu_camera.Items.Add(menuItem2);
            this.menu_camera.Items.Add(menuItem3);
            this.menu_camera.Location = new System.Drawing.Point(776, 41);
            this.menu_camera.Name = "menu_camera";
            this.menu_camera.ScrollBarBlock = true;
            this.menu_camera.Size = new System.Drawing.Size(187, 488);
            this.menu_camera.TabIndex = 12;
            this.menu_camera.Text = "menu1";
            this.menu_camera.SelectChanged += new AntdUI.SelectEventHandler(this.menu_camera_SelectChanged);
            // 
            // hWindow_Final1
            // 
            this.hWindow_Final1.BackColor = System.Drawing.Color.Transparent;
            this.hWindow_Final1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.hWindow_Final1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindow_Final1.DrawModel = false;
            this.hWindow_Final1.Image = null;
            this.hWindow_Final1.Location = new System.Drawing.Point(4, 42);
            this.hWindow_Final1.Margin = new System.Windows.Forms.Padding(4);
            this.hWindow_Final1.Name = "hWindow_Final1";
            this.hWindow_Final1.Size = new System.Drawing.Size(765, 486);
            this.hWindow_Final1.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Location = new System.Drawing.Point(583, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(380, 32);
            this.panel1.TabIndex = 7;
            this.panel1.Text = "panel1";
            // 
            // switch_camera
            // 
            this.switch_camera.CheckedText = "开";
            this.switch_camera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.switch_camera.Location = new System.Drawing.Point(438, 3);
            this.switch_camera.Name = "switch_camera";
            this.switch_camera.Size = new System.Drawing.Size(139, 32);
            this.switch_camera.TabIndex = 6;
            this.switch_camera.UnCheckedText = "关";
            this.switch_camera.CheckedChanged += new AntdUI.BoolEventHandler(this.switch_camera_CheckedChanged);
            // 
            // button_searchCamera
            // 
            this.button_searchCamera.Dock = System.Windows.Forms.DockStyle.Right;
            this.button_searchCamera.JoinMode = AntdUI.TJoinMode.Right;
            this.button_searchCamera.Location = new System.Drawing.Point(338, 0);
            this.button_searchCamera.Margin = new System.Windows.Forms.Padding(0);
            this.button_searchCamera.Name = "button_searchCamera";
            this.button_searchCamera.Size = new System.Drawing.Size(97, 38);
            this.button_searchCamera.TabIndex = 5;
            this.button_searchCamera.Text = "搜索";
            this.button_searchCamera.Type = AntdUI.TTypeMini.Primary;
            this.button_searchCamera.Click += new System.EventHandler(this.button_searchCamera_Click);
            // 
            // select_DeviceSerialNumber
            // 
            this.select_DeviceSerialNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.select_DeviceSerialNumber.JoinMode = AntdUI.TJoinMode.Left;
            this.select_DeviceSerialNumber.Location = new System.Drawing.Point(145, 0);
            this.select_DeviceSerialNumber.Margin = new System.Windows.Forms.Padding(0);
            this.select_DeviceSerialNumber.Name = "select_DeviceSerialNumber";
            this.select_DeviceSerialNumber.Size = new System.Drawing.Size(193, 38);
            this.select_DeviceSerialNumber.TabIndex = 4;
            // 
            // label_DeviceSerialNumber
            // 
            this.label_DeviceSerialNumber.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_DeviceSerialNumber.Location = new System.Drawing.Point(3, 3);
            this.label_DeviceSerialNumber.Name = "label_DeviceSerialNumber";
            this.label_DeviceSerialNumber.Size = new System.Drawing.Size(139, 32);
            this.label_DeviceSerialNumber.TabIndex = 1;
            this.label_DeviceSerialNumber.Text = "相机序列号:";
            this.label_DeviceSerialNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gridPanel_tool
            // 
            this.gridPanel_tool.Controls.Add(this.panel_CameraMenu);
            this.gridPanel_tool.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel_tool.Location = new System.Drawing.Point(0, 0);
            this.gridPanel_tool.Name = "gridPanel_tool";
            this.gridPanel_tool.Size = new System.Drawing.Size(470, 760);
            this.gridPanel_tool.Span = "\r\n100%";
            this.gridPanel_tool.TabIndex = 0;
            this.gridPanel_tool.Text = "gridPanel1";
            // 
            // panel_CameraMenu
            // 
            this.panel_CameraMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_CameraMenu.Location = new System.Drawing.Point(0, 0);
            this.panel_CameraMenu.Margin = new System.Windows.Forms.Padding(0);
            this.panel_CameraMenu.Name = "panel_CameraMenu";
            this.panel_CameraMenu.Size = new System.Drawing.Size(470, 760);
            this.panel_CameraMenu.TabIndex = 0;
            this.panel_CameraMenu.Text = "panel2";
            // 
            // Camera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.splitter_main);
            this.Name = "Camera";
            this.Size = new System.Drawing.Size(1440, 760);
            this.Load += new System.EventHandler(this.Camera_Load);
            this.splitter_main.Panel1.ResumeLayout(false);
            this.splitter_main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitter_main)).EndInit();
            this.splitter_main.ResumeLayout(false);
            this.gridPanel1.ResumeLayout(false);
            this.tabs_info.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel_DetectionLog.ResumeLayout(false);
            this.gridPanel_tool.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.Splitter splitter_main;
        private AntdUI.GridPanel gridPanel_tool;
        private AntdUI.GridPanel gridPanel1;
        private AntdUI.Label label_DeviceSerialNumber;
        private AntdUI.Select select_DeviceSerialNumber;
        private AntdUI.Button button_searchCamera;
        private AntdUI.Switch switch_camera;
        private AntdUI.Panel panel1;
        private AntdUI.Panel panel_CameraMenu;
        private AntdUI.Tabs tabs_info;
        private AntdUI.TabPage tabPage3;
        private AntdUI.TabPage tabPage1;
        private AntdUI.Panel panel_DetectionLog;
        private AntdUI.TabPage tabPage2;
        private AntdUI.Menu menu_camera;
        private HalconControl.HWindow_Final hWindow_Final1;
        private AntdUI.Table table_Log;
    }
}
