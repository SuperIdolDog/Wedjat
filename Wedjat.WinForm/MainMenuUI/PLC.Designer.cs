namespace Wedjat.WinForm.MainMenuUI
{
    partial class PLC
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
            AntdUI.Tabs.StyleLine styleLine1 = new AntdUI.Tabs.StyleLine();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PLC));
            this.tabs1 = new AntdUI.Tabs();
            this.tabPage1 = new AntdUI.TabPage();
            this.gridPanel_modbus = new AntdUI.GridPanel();
            this.splitter_table = new AntdUI.Splitter();
            this.panel_Equipmentlocation = new AntdUI.Panel();
            this.table_Equipmentlocation = new AntdUI.Table();
            this.panel_PLCLog = new AntdUI.Panel();
            this.table_PLCLog = new AntdUI.Table();
            this.gridPanel_ModbusSetting = new AntdUI.GridPanel();
            this.panel_TCP = new AntdUI.Panel();
            this.gridPanel_TCP = new AntdUI.GridPanel();
            this.panel_TCPbutton = new AntdUI.Panel();
            this.btn_SaveTCP = new AntdUI.Button();
            this.btn_switchTCP = new AntdUI.Button();
            this.panel_TCPPort = new AntdUI.Panel();
            this.inputNumber_TCPPort = new AntdUI.InputNumber();
            this.label_TCPPort = new AntdUI.Label();
            this.panel_TCPip = new AntdUI.Panel();
            this.input_ip = new AntdUI.Input();
            this.label_TCPip = new AntdUI.Label();
            this.panel_RTU = new AntdUI.Panel();
            this.gridPanel_ModbusRTU = new AntdUI.GridPanel();
            this.panel_RTUbutton = new AntdUI.Panel();
            this.btn_SaveRTU = new AntdUI.Button();
            this.btn_switchRTU = new AntdUI.Button();
            this.panel_Parity = new AntdUI.Panel();
            this.select_Parity = new AntdUI.Select();
            this.label_Parity = new AntdUI.Label();
            this.panel_StopBits = new AntdUI.Panel();
            this.select_StopBits = new AntdUI.Select();
            this.label_StopBits = new AntdUI.Label();
            this.panel_DataBits = new AntdUI.Panel();
            this.select_DataBits = new AntdUI.Select();
            this.label_DataBits = new AntdUI.Label();
            this.panel_BaudRate = new AntdUI.Panel();
            this.select_BaudRate = new AntdUI.Select();
            this.label_BaudRate = new AntdUI.Label();
            this.panel_RTUPort = new AntdUI.Panel();
            this.select_Serialport = new AntdUI.Select();
            this.panel_Serialport = new AntdUI.Label();
            this.tabPage2 = new AntdUI.TabPage();
            this.tabs1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gridPanel_modbus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitter_table)).BeginInit();
            this.splitter_table.Panel1.SuspendLayout();
            this.splitter_table.Panel2.SuspendLayout();
            this.splitter_table.SuspendLayout();
            this.panel_Equipmentlocation.SuspendLayout();
            this.panel_PLCLog.SuspendLayout();
            this.gridPanel_ModbusSetting.SuspendLayout();
            this.panel_TCP.SuspendLayout();
            this.gridPanel_TCP.SuspendLayout();
            this.panel_TCPbutton.SuspendLayout();
            this.panel_TCPPort.SuspendLayout();
            this.panel_TCPip.SuspendLayout();
            this.panel_RTU.SuspendLayout();
            this.gridPanel_ModbusRTU.SuspendLayout();
            this.panel_RTUbutton.SuspendLayout();
            this.panel_Parity.SuspendLayout();
            this.panel_StopBits.SuspendLayout();
            this.panel_DataBits.SuspendLayout();
            this.panel_BaudRate.SuspendLayout();
            this.panel_RTUPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs1
            // 
            this.tabs1.Controls.Add(this.tabPage1);
            this.tabs1.Controls.Add(this.tabPage2);
            this.tabs1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tabs1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabs1.Gap = 30;
            this.tabs1.Location = new System.Drawing.Point(0, 0);
            this.tabs1.Name = "tabs1";
            this.tabs1.Pages.Add(this.tabPage1);
            this.tabs1.Pages.Add(this.tabPage2);
            this.tabs1.Size = new System.Drawing.Size(1250, 810);
            this.tabs1.Style = styleLine1;
            this.tabs1.TabIndex = 0;
            this.tabs1.Text = "tabs1";
            this.tabs1.TextCenter = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.gridPanel_modbus);
            this.tabPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPage1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPage1.Location = new System.Drawing.Point(0, 72);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1250, 738);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Modbus";
            // 
            // gridPanel_modbus
            // 
            this.gridPanel_modbus.BorderWidth = 1F;
            this.gridPanel_modbus.Controls.Add(this.splitter_table);
            this.gridPanel_modbus.Controls.Add(this.gridPanel_ModbusSetting);
            this.gridPanel_modbus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel_modbus.Location = new System.Drawing.Point(0, 0);
            this.gridPanel_modbus.Name = "gridPanel_modbus";
            this.gridPanel_modbus.Size = new System.Drawing.Size(1250, 738);
            this.gridPanel_modbus.Span = "100%:20% 80% \r\n";
            this.gridPanel_modbus.TabIndex = 0;
            this.gridPanel_modbus.Text = "gridPanel1";
            // 
            // splitter_table
            // 
            this.splitter_table.Cursor = System.Windows.Forms.Cursors.Default;
            this.splitter_table.Location = new System.Drawing.Point(253, 3);
            this.splitter_table.Name = "splitter_table";
            // 
            // splitter_table.Panel1
            // 
            this.splitter_table.Panel1.Controls.Add(this.panel_Equipmentlocation);
            // 
            // splitter_table.Panel2
            // 
            this.splitter_table.Panel2.Controls.Add(this.panel_PLCLog);
            this.splitter_table.Size = new System.Drawing.Size(994, 732);
            this.splitter_table.SplitterDistance = 600;
            this.splitter_table.TabIndex = 17;
            // 
            // panel_Equipmentlocation
            // 
            this.panel_Equipmentlocation.BorderWidth = 1F;
            this.panel_Equipmentlocation.Controls.Add(this.table_Equipmentlocation);
            this.panel_Equipmentlocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_Equipmentlocation.Location = new System.Drawing.Point(0, 0);
            this.panel_Equipmentlocation.Name = "panel_Equipmentlocation";
            this.panel_Equipmentlocation.Size = new System.Drawing.Size(600, 732);
            this.panel_Equipmentlocation.TabIndex = 17;
            this.panel_Equipmentlocation.Text = "panel1";
            // 
            // table_Equipmentlocation
            // 
            this.table_Equipmentlocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table_Equipmentlocation.EmptyHeader = true;
            this.table_Equipmentlocation.Gap = 12;
            this.table_Equipmentlocation.Location = new System.Drawing.Point(2, 2);
            this.table_Equipmentlocation.Name = "table_Equipmentlocation";
            this.table_Equipmentlocation.Size = new System.Drawing.Size(596, 728);
            this.table_Equipmentlocation.TabIndex = 3;
            this.table_Equipmentlocation.Text = "table1";
            this.table_Equipmentlocation.CellClick += new AntdUI.Table.ClickEventHandler(this.table_Equipmentlocation_CellClick);
            // 
            // panel_PLCLog
            // 
            this.panel_PLCLog.BorderWidth = 1F;
            this.panel_PLCLog.Controls.Add(this.table_PLCLog);
            this.panel_PLCLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_PLCLog.Location = new System.Drawing.Point(0, 0);
            this.panel_PLCLog.Name = "panel_PLCLog";
            this.panel_PLCLog.Size = new System.Drawing.Size(390, 732);
            this.panel_PLCLog.TabIndex = 20;
            this.panel_PLCLog.Text = "panel1";
            // 
            // table_PLCLog
            // 
            this.table_PLCLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table_PLCLog.EmptyHeader = true;
            this.table_PLCLog.Gap = 12;
            this.table_PLCLog.Location = new System.Drawing.Point(2, 2);
            this.table_PLCLog.Name = "table_PLCLog";
            this.table_PLCLog.Size = new System.Drawing.Size(386, 728);
            this.table_PLCLog.TabIndex = 15;
            this.table_PLCLog.Text = "table1";
            // 
            // gridPanel_ModbusSetting
            // 
            this.gridPanel_ModbusSetting.Controls.Add(this.panel_TCP);
            this.gridPanel_ModbusSetting.Controls.Add(this.panel_RTU);
            this.gridPanel_ModbusSetting.Location = new System.Drawing.Point(3, 3);
            this.gridPanel_ModbusSetting.Name = "gridPanel_ModbusSetting";
            this.gridPanel_ModbusSetting.Size = new System.Drawing.Size(244, 732);
            this.gridPanel_ModbusSetting.Span = "65%:100%;\r\n35%:100%";
            this.gridPanel_ModbusSetting.TabIndex = 14;
            this.gridPanel_ModbusSetting.Text = "gridPanel1";
            // 
            // panel_TCP
            // 
            this.panel_TCP.Controls.Add(this.gridPanel_TCP);
            this.panel_TCP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_TCP.Location = new System.Drawing.Point(3, 479);
            this.panel_TCP.Name = "panel_TCP";
            this.panel_TCP.Size = new System.Drawing.Size(238, 250);
            this.panel_TCP.TabIndex = 14;
            this.panel_TCP.Text = "panel2";
            // 
            // gridPanel_TCP
            // 
            this.gridPanel_TCP.BackColor = System.Drawing.Color.Transparent;
            this.gridPanel_TCP.Controls.Add(this.panel_TCPbutton);
            this.gridPanel_TCP.Controls.Add(this.panel_TCPPort);
            this.gridPanel_TCP.Controls.Add(this.panel_TCPip);
            this.gridPanel_TCP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel_TCP.Location = new System.Drawing.Point(0, 0);
            this.gridPanel_TCP.Name = "gridPanel_TCP";
            this.gridPanel_TCP.Size = new System.Drawing.Size(238, 250);
            this.gridPanel_TCP.Span = "33%:100%;\r\n33%:100%;\r\n34%:100%";
            this.gridPanel_TCP.TabIndex = 11;
            this.gridPanel_TCP.Text = "gridPanel1";
            // 
            // panel_TCPbutton
            // 
            this.panel_TCPbutton.Controls.Add(this.btn_SaveTCP);
            this.panel_TCPbutton.Controls.Add(this.btn_switchTCP);
            this.panel_TCPbutton.Location = new System.Drawing.Point(3, 168);
            this.panel_TCPbutton.Name = "panel_TCPbutton";
            this.panel_TCPbutton.Size = new System.Drawing.Size(232, 79);
            this.panel_TCPbutton.TabIndex = 38;
            this.panel_TCPbutton.Text = "panel2";
            // 
            // btn_SaveTCP
            // 
            this.btn_SaveTCP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_SaveTCP.IconRatio = 1F;
            this.btn_SaveTCP.IconSvg = resources.GetString("btn_SaveTCP.IconSvg");
            this.btn_SaveTCP.Location = new System.Drawing.Point(0, 0);
            this.btn_SaveTCP.Name = "btn_SaveTCP";
            this.btn_SaveTCP.Size = new System.Drawing.Size(129, 79);
            this.btn_SaveTCP.TabIndex = 11;
            this.btn_SaveTCP.Text = "保存配置";
            this.btn_SaveTCP.Type = AntdUI.TTypeMini.Primary;
            // 
            // btn_switchTCP
            // 
            this.btn_switchTCP.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_switchTCP.IconRatio = 1.5F;
            this.btn_switchTCP.IconSvg = resources.GetString("btn_switchTCP.IconSvg");
            this.btn_switchTCP.Location = new System.Drawing.Point(129, 0);
            this.btn_switchTCP.Margin = new System.Windows.Forms.Padding(0);
            this.btn_switchTCP.Name = "btn_switchTCP";
            this.btn_switchTCP.Shape = AntdUI.TShape.Circle;
            this.btn_switchTCP.Size = new System.Drawing.Size(103, 79);
            this.btn_switchTCP.TabIndex = 10;
            this.btn_switchTCP.Click += new System.EventHandler(this.btn_switchTCP_Click);
            // 
            // panel_TCPPort
            // 
            this.panel_TCPPort.BackColor = System.Drawing.Color.Transparent;
            this.panel_TCPPort.BorderWidth = 1F;
            this.panel_TCPPort.Controls.Add(this.inputNumber_TCPPort);
            this.panel_TCPPort.Controls.Add(this.label_TCPPort);
            this.panel_TCPPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_TCPPort.Location = new System.Drawing.Point(3, 85);
            this.panel_TCPPort.Name = "panel_TCPPort";
            this.panel_TCPPort.Size = new System.Drawing.Size(232, 76);
            this.panel_TCPPort.TabIndex = 24;
            // 
            // inputNumber_TCPPort
            // 
            this.inputNumber_TCPPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputNumber_TCPPort.IconGap = 1F;
            this.inputNumber_TCPPort.IconRatio = 1F;
            this.inputNumber_TCPPort.Location = new System.Drawing.Point(2, 25);
            this.inputNumber_TCPPort.Name = "inputNumber_TCPPort";
            this.inputNumber_TCPPort.PrefixSvg = resources.GetString("inputNumber_TCPPort.PrefixSvg");
            this.inputNumber_TCPPort.Size = new System.Drawing.Size(228, 49);
            this.inputNumber_TCPPort.TabIndex = 3;
            this.inputNumber_TCPPort.Text = "502";
            this.inputNumber_TCPPort.Value = new decimal(new int[] {
            502,
            0,
            0,
            0});
            // 
            // label_TCPPort
            // 
            this.label_TCPPort.BackColor = System.Drawing.Color.Transparent;
            this.label_TCPPort.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_TCPPort.Location = new System.Drawing.Point(2, 2);
            this.label_TCPPort.Name = "label_TCPPort";
            this.label_TCPPort.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label_TCPPort.Size = new System.Drawing.Size(228, 23);
            this.label_TCPPort.TabIndex = 0;
            this.label_TCPPort.Text = "端口号";
            // 
            // panel_TCPip
            // 
            this.panel_TCPip.BackColor = System.Drawing.Color.Transparent;
            this.panel_TCPip.BorderWidth = 1F;
            this.panel_TCPip.Controls.Add(this.input_ip);
            this.panel_TCPip.Controls.Add(this.label_TCPip);
            this.panel_TCPip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_TCPip.Location = new System.Drawing.Point(3, 3);
            this.panel_TCPip.Name = "panel_TCPip";
            this.panel_TCPip.Size = new System.Drawing.Size(232, 76);
            this.panel_TCPip.TabIndex = 23;
            this.panel_TCPip.Text = "panel1";
            // 
            // input_ip
            // 
            this.input_ip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.input_ip.IconGap = 1F;
            this.input_ip.IconRatio = 1F;
            this.input_ip.Location = new System.Drawing.Point(2, 25);
            this.input_ip.Name = "input_ip";
            this.input_ip.PrefixSvg = resources.GetString("input_ip.PrefixSvg");
            this.input_ip.Size = new System.Drawing.Size(228, 49);
            this.input_ip.TabIndex = 1;
            this.input_ip.Text = "127.0.0.1";
            // 
            // label_TCPip
            // 
            this.label_TCPip.BackColor = System.Drawing.Color.Transparent;
            this.label_TCPip.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_TCPip.Location = new System.Drawing.Point(2, 2);
            this.label_TCPip.Name = "label_TCPip";
            this.label_TCPip.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label_TCPip.Size = new System.Drawing.Size(228, 23);
            this.label_TCPip.TabIndex = 0;
            this.label_TCPip.Text = "IP地址";
            // 
            // panel_RTU
            // 
            this.panel_RTU.BorderWidth = 1F;
            this.panel_RTU.Controls.Add(this.gridPanel_ModbusRTU);
            this.panel_RTU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_RTU.Location = new System.Drawing.Point(3, 3);
            this.panel_RTU.Name = "panel_RTU";
            this.panel_RTU.Size = new System.Drawing.Size(238, 470);
            this.panel_RTU.TabIndex = 9;
            this.panel_RTU.Text = "panel1";
            // 
            // gridPanel_ModbusRTU
            // 
            this.gridPanel_ModbusRTU.BackColor = System.Drawing.Color.Transparent;
            this.gridPanel_ModbusRTU.Controls.Add(this.panel_RTUbutton);
            this.gridPanel_ModbusRTU.Controls.Add(this.panel_Parity);
            this.gridPanel_ModbusRTU.Controls.Add(this.panel_StopBits);
            this.gridPanel_ModbusRTU.Controls.Add(this.panel_DataBits);
            this.gridPanel_ModbusRTU.Controls.Add(this.panel_BaudRate);
            this.gridPanel_ModbusRTU.Controls.Add(this.panel_RTUPort);
            this.gridPanel_ModbusRTU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel_ModbusRTU.Location = new System.Drawing.Point(2, 2);
            this.gridPanel_ModbusRTU.Name = "gridPanel_ModbusRTU";
            this.gridPanel_ModbusRTU.Size = new System.Drawing.Size(234, 466);
            this.gridPanel_ModbusRTU.Span = "16%:100%;\r\n16%:100%;\r\n16%:100%;\r\n16%:100%;\r\n16%:100%;\r\n20%:100%";
            this.gridPanel_ModbusRTU.TabIndex = 7;
            this.gridPanel_ModbusRTU.Text = "gridPanel1";
            // 
            // panel_RTUbutton
            // 
            this.panel_RTUbutton.Controls.Add(this.btn_SaveRTU);
            this.panel_RTUbutton.Controls.Add(this.btn_switchRTU);
            this.panel_RTUbutton.Location = new System.Drawing.Point(3, 376);
            this.panel_RTUbutton.Name = "panel_RTUbutton";
            this.panel_RTUbutton.Size = new System.Drawing.Size(228, 87);
            this.panel_RTUbutton.TabIndex = 37;
            this.panel_RTUbutton.Text = "panel2";
            // 
            // btn_SaveRTU
            // 
            this.btn_SaveRTU.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_SaveRTU.IconRatio = 1F;
            this.btn_SaveRTU.IconSvg = resources.GetString("btn_SaveRTU.IconSvg");
            this.btn_SaveRTU.Location = new System.Drawing.Point(0, 0);
            this.btn_SaveRTU.Name = "btn_SaveRTU";
            this.btn_SaveRTU.Size = new System.Drawing.Size(125, 87);
            this.btn_SaveRTU.TabIndex = 9;
            this.btn_SaveRTU.Text = "保存配置";
            this.btn_SaveRTU.Type = AntdUI.TTypeMini.Primary;
            // 
            // btn_switchRTU
            // 
            this.btn_switchRTU.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_switchRTU.IconRatio = 1.5F;
            this.btn_switchRTU.IconSvg = resources.GetString("btn_switchRTU.IconSvg");
            this.btn_switchRTU.Location = new System.Drawing.Point(125, 0);
            this.btn_switchRTU.Margin = new System.Windows.Forms.Padding(0);
            this.btn_switchRTU.Name = "btn_switchRTU";
            this.btn_switchRTU.Shape = AntdUI.TShape.Circle;
            this.btn_switchRTU.Size = new System.Drawing.Size(103, 87);
            this.btn_switchRTU.TabIndex = 8;
            this.btn_switchRTU.Click += new System.EventHandler(this.btn_switchRTU_ClickAsync);
            // 
            // panel_Parity
            // 
            this.panel_Parity.BackColor = System.Drawing.Color.Transparent;
            this.panel_Parity.BorderWidth = 1F;
            this.panel_Parity.Controls.Add(this.select_Parity);
            this.panel_Parity.Controls.Add(this.label_Parity);
            this.panel_Parity.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Parity.Location = new System.Drawing.Point(3, 301);
            this.panel_Parity.Name = "panel_Parity";
            this.panel_Parity.Size = new System.Drawing.Size(228, 69);
            this.panel_Parity.TabIndex = 36;
            this.panel_Parity.Text = "panel9";
            // 
            // select_Parity
            // 
            this.select_Parity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.select_Parity.IconGap = 1F;
            this.select_Parity.IconRatio = 1F;
            this.select_Parity.Location = new System.Drawing.Point(2, 25);
            this.select_Parity.Name = "select_Parity";
            this.select_Parity.PrefixSvg = resources.GetString("select_Parity.PrefixSvg");
            this.select_Parity.Size = new System.Drawing.Size(224, 42);
            this.select_Parity.TabIndex = 3;
            // 
            // label_Parity
            // 
            this.label_Parity.BackColor = System.Drawing.Color.Transparent;
            this.label_Parity.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_Parity.Location = new System.Drawing.Point(2, 2);
            this.label_Parity.Name = "label_Parity";
            this.label_Parity.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label_Parity.Size = new System.Drawing.Size(224, 23);
            this.label_Parity.TabIndex = 2;
            this.label_Parity.Text = "校验方式";
            // 
            // panel_StopBits
            // 
            this.panel_StopBits.BackColor = System.Drawing.Color.Transparent;
            this.panel_StopBits.BorderWidth = 1F;
            this.panel_StopBits.Controls.Add(this.select_StopBits);
            this.panel_StopBits.Controls.Add(this.label_StopBits);
            this.panel_StopBits.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_StopBits.Location = new System.Drawing.Point(3, 227);
            this.panel_StopBits.Name = "panel_StopBits";
            this.panel_StopBits.Size = new System.Drawing.Size(228, 69);
            this.panel_StopBits.TabIndex = 35;
            this.panel_StopBits.Text = "panel8";
            // 
            // select_StopBits
            // 
            this.select_StopBits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.select_StopBits.IconGap = 1F;
            this.select_StopBits.IconRatio = 1F;
            this.select_StopBits.Location = new System.Drawing.Point(2, 25);
            this.select_StopBits.Name = "select_StopBits";
            this.select_StopBits.PrefixSvg = resources.GetString("select_StopBits.PrefixSvg");
            this.select_StopBits.Size = new System.Drawing.Size(224, 42);
            this.select_StopBits.TabIndex = 3;
            // 
            // label_StopBits
            // 
            this.label_StopBits.BackColor = System.Drawing.Color.Transparent;
            this.label_StopBits.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_StopBits.Location = new System.Drawing.Point(2, 2);
            this.label_StopBits.Name = "label_StopBits";
            this.label_StopBits.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label_StopBits.Size = new System.Drawing.Size(224, 23);
            this.label_StopBits.TabIndex = 2;
            this.label_StopBits.Text = "停止位";
            // 
            // panel_DataBits
            // 
            this.panel_DataBits.BorderWidth = 1F;
            this.panel_DataBits.Controls.Add(this.select_DataBits);
            this.panel_DataBits.Controls.Add(this.label_DataBits);
            this.panel_DataBits.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_DataBits.Location = new System.Drawing.Point(3, 152);
            this.panel_DataBits.Name = "panel_DataBits";
            this.panel_DataBits.Size = new System.Drawing.Size(228, 69);
            this.panel_DataBits.TabIndex = 34;
            this.panel_DataBits.Text = "panel7";
            // 
            // select_DataBits
            // 
            this.select_DataBits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.select_DataBits.IconGap = 1F;
            this.select_DataBits.IconRatio = 1F;
            this.select_DataBits.Location = new System.Drawing.Point(2, 25);
            this.select_DataBits.Name = "select_DataBits";
            this.select_DataBits.PrefixSvg = resources.GetString("select_DataBits.PrefixSvg");
            this.select_DataBits.Size = new System.Drawing.Size(224, 42);
            this.select_DataBits.TabIndex = 3;
            // 
            // label_DataBits
            // 
            this.label_DataBits.BackColor = System.Drawing.Color.Transparent;
            this.label_DataBits.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_DataBits.Location = new System.Drawing.Point(2, 2);
            this.label_DataBits.Name = "label_DataBits";
            this.label_DataBits.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label_DataBits.Size = new System.Drawing.Size(224, 23);
            this.label_DataBits.TabIndex = 2;
            this.label_DataBits.Text = "数据位";
            // 
            // panel_BaudRate
            // 
            this.panel_BaudRate.BackColor = System.Drawing.Color.Transparent;
            this.panel_BaudRate.BorderWidth = 1F;
            this.panel_BaudRate.Controls.Add(this.select_BaudRate);
            this.panel_BaudRate.Controls.Add(this.label_BaudRate);
            this.panel_BaudRate.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_BaudRate.Location = new System.Drawing.Point(3, 78);
            this.panel_BaudRate.Name = "panel_BaudRate";
            this.panel_BaudRate.Size = new System.Drawing.Size(228, 69);
            this.panel_BaudRate.TabIndex = 33;
            this.panel_BaudRate.Text = "panel6";
            // 
            // select_BaudRate
            // 
            this.select_BaudRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.select_BaudRate.IconGap = 1F;
            this.select_BaudRate.IconRatio = 1F;
            this.select_BaudRate.Location = new System.Drawing.Point(2, 25);
            this.select_BaudRate.Name = "select_BaudRate";
            this.select_BaudRate.PrefixSvg = resources.GetString("select_BaudRate.PrefixSvg");
            this.select_BaudRate.Size = new System.Drawing.Size(224, 42);
            this.select_BaudRate.TabIndex = 3;
            // 
            // label_BaudRate
            // 
            this.label_BaudRate.BackColor = System.Drawing.Color.Transparent;
            this.label_BaudRate.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_BaudRate.Location = new System.Drawing.Point(2, 2);
            this.label_BaudRate.Name = "label_BaudRate";
            this.label_BaudRate.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.label_BaudRate.Size = new System.Drawing.Size(224, 23);
            this.label_BaudRate.TabIndex = 2;
            this.label_BaudRate.Text = "波特率";
            // 
            // panel_RTUPort
            // 
            this.panel_RTUPort.BackColor = System.Drawing.Color.Transparent;
            this.panel_RTUPort.BorderWidth = 1F;
            this.panel_RTUPort.Controls.Add(this.select_Serialport);
            this.panel_RTUPort.Controls.Add(this.panel_Serialport);
            this.panel_RTUPort.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_RTUPort.Location = new System.Drawing.Point(3, 3);
            this.panel_RTUPort.Name = "panel_RTUPort";
            this.panel_RTUPort.Size = new System.Drawing.Size(228, 69);
            this.panel_RTUPort.TabIndex = 32;
            this.panel_RTUPort.Text = "panel_Serialport";
            // 
            // select_Serialport
            // 
            this.select_Serialport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.select_Serialport.IconGap = 1F;
            this.select_Serialport.IconRatio = 1F;
            this.select_Serialport.Location = new System.Drawing.Point(2, 25);
            this.select_Serialport.Name = "select_Serialport";
            this.select_Serialport.PrefixSvg = resources.GetString("select_Serialport.PrefixSvg");
            this.select_Serialport.Size = new System.Drawing.Size(224, 42);
            this.select_Serialport.TabIndex = 1;
            // 
            // panel_Serialport
            // 
            this.panel_Serialport.BackColor = System.Drawing.Color.Transparent;
            this.panel_Serialport.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_Serialport.Location = new System.Drawing.Point(2, 2);
            this.panel_Serialport.Name = "panel_Serialport";
            this.panel_Serialport.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.panel_Serialport.Size = new System.Drawing.Size(224, 23);
            this.panel_Serialport.TabIndex = 0;
            this.panel_Serialport.Text = "串口号";
            // 
            // tabPage2
            // 
            this.tabPage2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPage2.Location = new System.Drawing.Point(0, 72);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1250, 738);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "西门子S7";
            // 
            // PLC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabs1);
            this.Name = "PLC";
            this.Size = new System.Drawing.Size(1250, 810);
            this.Load += new System.EventHandler(this.PLC_LoadAsync);
            this.tabs1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.gridPanel_modbus.ResumeLayout(false);
            this.splitter_table.Panel1.ResumeLayout(false);
            this.splitter_table.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitter_table)).EndInit();
            this.splitter_table.ResumeLayout(false);
            this.panel_Equipmentlocation.ResumeLayout(false);
            this.panel_PLCLog.ResumeLayout(false);
            this.gridPanel_ModbusSetting.ResumeLayout(false);
            this.panel_TCP.ResumeLayout(false);
            this.gridPanel_TCP.ResumeLayout(false);
            this.panel_TCPbutton.ResumeLayout(false);
            this.panel_TCPPort.ResumeLayout(false);
            this.panel_TCPip.ResumeLayout(false);
            this.panel_RTU.ResumeLayout(false);
            this.gridPanel_ModbusRTU.ResumeLayout(false);
            this.panel_RTUbutton.ResumeLayout(false);
            this.panel_Parity.ResumeLayout(false);
            this.panel_StopBits.ResumeLayout(false);
            this.panel_DataBits.ResumeLayout(false);
            this.panel_BaudRate.ResumeLayout(false);
            this.panel_RTUPort.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.Tabs tabs1;
        private AntdUI.TabPage tabPage1;
        private AntdUI.TabPage tabPage2;
        private AntdUI.GridPanel gridPanel_modbus;
        private AntdUI.GridPanel gridPanel_ModbusSetting;
        private AntdUI.Panel panel_TCP;
        private AntdUI.GridPanel gridPanel_TCP;
        private AntdUI.Panel panel_TCPbutton;
        private AntdUI.Button btn_SaveTCP;
        private AntdUI.Button btn_switchTCP;
        private AntdUI.Panel panel_TCPPort;
        private AntdUI.InputNumber inputNumber_TCPPort;
        private AntdUI.Label label_TCPPort;
        private AntdUI.Panel panel_TCPip;
        private AntdUI.Input input_ip;
        private AntdUI.Label label_TCPip;
        private AntdUI.Panel panel_RTU;
        private AntdUI.GridPanel gridPanel_ModbusRTU;
        private AntdUI.Panel panel_RTUbutton;
        private AntdUI.Button btn_SaveRTU;
        private AntdUI.Button btn_switchRTU;
        private AntdUI.Panel panel_Parity;
        private AntdUI.Select select_Parity;
        private AntdUI.Label label_Parity;
        private AntdUI.Panel panel_StopBits;
        private AntdUI.Select select_StopBits;
        private AntdUI.Label label_StopBits;
        private AntdUI.Panel panel_DataBits;
        private AntdUI.Select select_DataBits;
        private AntdUI.Label label_DataBits;
        private AntdUI.Panel panel_BaudRate;
        private AntdUI.Select select_BaudRate;
        private AntdUI.Label label_BaudRate;
        private AntdUI.Panel panel_RTUPort;
        private AntdUI.Select select_Serialport;
        private AntdUI.Label panel_Serialport;
        private AntdUI.Splitter splitter_table;
        private AntdUI.Panel panel_Equipmentlocation;
        private AntdUI.Table table_Equipmentlocation;
        private AntdUI.Panel panel_PLCLog;
        private AntdUI.Table table_PLCLog;
    }
}
