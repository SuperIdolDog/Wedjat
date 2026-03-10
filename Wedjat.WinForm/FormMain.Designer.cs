namespace Wedjat.WinForm
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            AntdUI.MenuItem menuItem13 = new AntdUI.MenuItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            AntdUI.MenuItem menuItem14 = new AntdUI.MenuItem();
            AntdUI.MenuItem menuItem15 = new AntdUI.MenuItem();
            AntdUI.MenuItem menuItem16 = new AntdUI.MenuItem();
            AntdUI.MenuItem menuItem17 = new AntdUI.MenuItem();
            AntdUI.MenuItem menuItem18 = new AntdUI.MenuItem();
            this.pageHeader1 = new AntdUI.PageHeader();
            this.label_CPU = new AntdUI.Label();
            this.btn_Accordion = new AntdUI.Button();
            this.btn_Theme = new AntdUI.Button();
            this.btn_global = new AntdUI.Dropdown();
            this.btn_setting = new AntdUI.Button();
            this.panel1 = new AntdUI.Panel();
            this.panelContent = new AntdUI.Panel();
            this.sys_Menu = new AntdUI.Menu();
            this.pageHeader1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pageHeader1
            // 
            this.pageHeader1.Controls.Add(this.label_CPU);
            this.pageHeader1.Controls.Add(this.btn_Accordion);
            this.pageHeader1.Controls.Add(this.btn_Theme);
            this.pageHeader1.Controls.Add(this.btn_global);
            this.pageHeader1.Controls.Add(this.btn_setting);
            this.pageHeader1.DividerShow = true;
            this.pageHeader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pageHeader1.Font = new System.Drawing.Font("Siemens Slab SC", 14F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pageHeader1.Location = new System.Drawing.Point(0, 0);
            this.pageHeader1.MDI = true;
            this.pageHeader1.Name = "pageHeader1";
            this.pageHeader1.ShowButton = true;
            this.pageHeader1.Size = new System.Drawing.Size(1440, 60);
            this.pageHeader1.TabIndex = 16;
            this.pageHeader1.Text = "Wedjat";
            // 
            // label_CPU
            // 
            this.label_CPU.BackColor = System.Drawing.Color.Transparent;
            this.label_CPU.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_CPU.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_CPU.Location = new System.Drawing.Point(879, 0);
            this.label_CPU.Name = "label_CPU";
            this.label_CPU.Size = new System.Drawing.Size(165, 60);
            this.label_CPU.TabIndex = 18;
            this.label_CPU.Text = "";
            // 
            // btn_Accordion
            // 
            this.btn_Accordion.Dock = System.Windows.Forms.DockStyle.Left;
            this.btn_Accordion.Ghost = true;
            this.btn_Accordion.IconSvg = "AppstoreFilled";
            this.btn_Accordion.Location = new System.Drawing.Point(119, 0);
            this.btn_Accordion.Name = "btn_Accordion";
            this.btn_Accordion.Radius = 0;
            this.btn_Accordion.Size = new System.Drawing.Size(76, 60);
            this.btn_Accordion.TabIndex = 17;
            this.btn_Accordion.ToggleIconSvg = "";
            this.btn_Accordion.WaveSize = 0;
            this.btn_Accordion.Click += new System.EventHandler(this.btn_Accordion_Click);
            // 
            // btn_Theme
            // 
            this.btn_Theme.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_Theme.Ghost = true;
            this.btn_Theme.IconSvg = "SunOutlined";
            this.btn_Theme.Location = new System.Drawing.Point(1044, 0);
            this.btn_Theme.Margin = new System.Windows.Forms.Padding(0);
            this.btn_Theme.Name = "btn_Theme";
            this.btn_Theme.Radius = 0;
            this.btn_Theme.Size = new System.Drawing.Size(60, 60);
            this.btn_Theme.TabIndex = 16;
            this.btn_Theme.ToggleIconSvg = "MoonOutlined";
            this.btn_Theme.WaveSize = 0;
            this.btn_Theme.Click += new System.EventHandler(this.btn_Theme_Click);
            // 
            // btn_global
            // 
            this.btn_global.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_global.Ghost = true;
            this.btn_global.IconSvg = "GlobalOutlined";
            this.btn_global.Location = new System.Drawing.Point(1104, 0);
            this.btn_global.Margin = new System.Windows.Forms.Padding(0);
            this.btn_global.Name = "btn_global";
            this.btn_global.Radius = 0;
            this.btn_global.Size = new System.Drawing.Size(60, 60);
            this.btn_global.TabIndex = 15;
            this.btn_global.WaveSize = 0;
            // 
            // btn_setting
            // 
            this.btn_setting.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_setting.Ghost = true;
            this.btn_setting.IconSvg = "SettingOutlined";
            this.btn_setting.Location = new System.Drawing.Point(1164, 0);
            this.btn_setting.Margin = new System.Windows.Forms.Padding(0);
            this.btn_setting.Name = "btn_setting";
            this.btn_setting.Radius = 0;
            this.btn_setting.Size = new System.Drawing.Size(60, 60);
            this.btn_setting.TabIndex = 14;
            this.btn_setting.WaveSize = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panelContent);
            this.panel1.Controls.Add(this.sys_Menu);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1440, 810);
            this.panel1.TabIndex = 17;
            this.panel1.Text = "panelMain";
            // 
            // panelContent
            // 
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 50);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1440, 760);
            this.panelContent.TabIndex = 8;
            this.panelContent.Text = "panel2";
            // 
            // sys_Menu
            // 
            this.sys_Menu.BackActive = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(58)))), ((int)(((byte)(183)))));
            this.sys_Menu.Dock = System.Windows.Forms.DockStyle.Top;
            this.sys_Menu.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.sys_Menu.ForeActive = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.sys_Menu.Gap = 15;
            this.sys_Menu.IconRatio = 1.5F;
            this.sys_Menu.itemMargin = 5;
            menuItem13.IconSvg = resources.GetString("menuItem13.IconSvg");
            menuItem13.Name = "UCConsole";
            menuItem13.Select = true;
            menuItem13.Text = "控制台";
            menuItem14.IconSvg = resources.GetString("menuItem14.IconSvg");
            menuItem14.Name = "UCCamera";
            menuItem14.Text = "视觉工业相机";
            menuItem15.IconSvg = resources.GetString("menuItem15.IconSvg");
            menuItem15.Name = "UCPLC";
            menuItem15.Text = "可编程控制器";
            menuItem16.IconSvg = resources.GetString("menuItem16.IconSvg");
            menuItem16.Name = "UCScanner";
            menuItem16.Text = "串口扫码枪";
            menuItem17.IconSvg = resources.GetString("menuItem17.IconSvg");
            menuItem17.Name = "UCReport";
            menuItem17.Text = "报表数据分析";
            menuItem18.IconSvg = resources.GetString("menuItem18.IconSvg");
            menuItem18.Name = "UCDatabase";
            menuItem18.Text = "数据库系统";
            this.sys_Menu.Items.Add(menuItem13);
            this.sys_Menu.Items.Add(menuItem14);
            this.sys_Menu.Items.Add(menuItem15);
            this.sys_Menu.Items.Add(menuItem16);
            this.sys_Menu.Items.Add(menuItem17);
            this.sys_Menu.Items.Add(menuItem18);
            this.sys_Menu.Location = new System.Drawing.Point(0, 0);
            this.sys_Menu.Mode = AntdUI.TMenuMode.Horizontal;
            this.sys_Menu.Name = "sys_Menu";
            this.sys_Menu.Size = new System.Drawing.Size(1440, 50);
            this.sys_Menu.TabIndex = 5;
            this.sys_Menu.SelectChanged += new AntdUI.SelectEventHandler(this.sys_Menu_SelectChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1440, 870);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pageHeader1);
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.pageHeader1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private AntdUI.PageHeader pageHeader1;
        private AntdUI.Button btn_Accordion;
        private AntdUI.Button btn_Theme;
        private AntdUI.Dropdown btn_global;
        private AntdUI.Button btn_setting;
        private AntdUI.Panel panel1;
        private AntdUI.Menu sys_Menu;
        private AntdUI.Label label_CPU;
        private AntdUI.Panel panelContent;
    }
}