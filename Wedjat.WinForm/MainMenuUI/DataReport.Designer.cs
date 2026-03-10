namespace Wedjat.WinForm.MainMenuUI
{
    partial class DataReport
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
            this.panel_main = new AntdUI.Panel();
            this.gridPanel_main = new AntdUI.GridPanel();
            this.chart_Defect = new AntdUI.Chart();
            this.chart_InspectionEfficiency = new AntdUI.Chart();
            this.gridPanel_Card = new AntdUI.GridPanel();
            this.panel4 = new AntdUI.Panel();
            this.label_passRate = new AntdUI.Label();
            this.label8 = new AntdUI.Label();
            this.panel3 = new AntdUI.Panel();
            this.label_NGCount = new AntdUI.Label();
            this.label6 = new AntdUI.Label();
            this.panel2 = new AntdUI.Panel();
            this.label_InspectionCount = new AntdUI.Label();
            this.label4 = new AntdUI.Label();
            this.panel1 = new AntdUI.Panel();
            this.label_workOrderCount = new AntdUI.Label();
            this.label1 = new AntdUI.Label();
            this.chart4 = new AntdUI.Chart();
            this.stackPanel1 = new AntdUI.StackPanel();
            this.button_Reset = new AntdUI.Button();
            this.button_Select = new AntdUI.Button();
            this.datePickerRange = new AntdUI.DatePickerRange();
            this.label_date = new AntdUI.Label();
            this.input_WorkOrder = new AntdUI.Input();
            this.label_workOrder = new AntdUI.Label();
            this.panel_main.SuspendLayout();
            this.gridPanel_main.SuspendLayout();
            this.gridPanel_Card.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.stackPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_main
            // 
            this.panel_main.Controls.Add(this.gridPanel_main);
            this.panel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_main.Location = new System.Drawing.Point(0, 0);
            this.panel_main.Name = "panel_main";
            this.panel_main.Size = new System.Drawing.Size(1440, 760);
            this.panel_main.TabIndex = 0;
            this.panel_main.Text = "panel1";
            // 
            // gridPanel_main
            // 
            this.gridPanel_main.BackColor = System.Drawing.Color.Transparent;
            this.gridPanel_main.Controls.Add(this.chart_Defect);
            this.gridPanel_main.Controls.Add(this.chart_InspectionEfficiency);
            this.gridPanel_main.Controls.Add(this.gridPanel_Card);
            this.gridPanel_main.Controls.Add(this.chart4);
            this.gridPanel_main.Controls.Add(this.stackPanel1);
            this.gridPanel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel_main.Gap = 5;
            this.gridPanel_main.Location = new System.Drawing.Point(0, 0);
            this.gridPanel_main.Name = "gridPanel_main";
            this.gridPanel_main.Size = new System.Drawing.Size(1440, 760);
            this.gridPanel_main.Span = "10%:100%; \r\n85% 15%;\r\n60% 40% ";
            this.gridPanel_main.TabIndex = 0;
            this.gridPanel_main.Text = "gridPanel1";
            // 
            // chart_Defect
            // 
            this.chart_Defect.AxisColor = null;
            this.chart_Defect.ChartType = AntdUI.TChartType.Doughnut;
            this.chart_Defect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart_Defect.GridColor = null;
            this.chart_Defect.LegendBackColor = null;
            this.chart_Defect.LegendBorderColor = null;
            this.chart_Defect.Location = new System.Drawing.Point(875, 429);
            this.chart_Defect.Name = "chart_Defect";
            this.chart_Defect.PieColors = null;
            this.chart_Defect.ShowAxes = false;
            this.chart_Defect.ShowGrid = false;
            this.chart_Defect.Size = new System.Drawing.Size(554, 320);
            this.chart_Defect.TabIndex = 10;
            this.chart_Defect.Text = "chart3";
            this.chart_Defect.Title = "缺陷类型分布";
            this.chart_Defect.TitleColor = null;
            this.chart_Defect.TitleFont = null;
            // 
            // chart_InspectionEfficiency
            // 
            this.chart_InspectionEfficiency.AxisColor = null;
            this.chart_InspectionEfficiency.ChartType = AntdUI.TChartType.StackedHorizontalBar;
            this.chart_InspectionEfficiency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart_InspectionEfficiency.GridColor = null;
            this.chart_InspectionEfficiency.LegendBackColor = null;
            this.chart_InspectionEfficiency.LegendBorderColor = null;
            this.chart_InspectionEfficiency.Location = new System.Drawing.Point(11, 429);
            this.chart_InspectionEfficiency.Name = "chart_InspectionEfficiency";
            this.chart_InspectionEfficiency.PieColors = null;
            this.chart_InspectionEfficiency.ShowGrid = false;
            this.chart_InspectionEfficiency.Size = new System.Drawing.Size(842, 320);
            this.chart_InspectionEfficiency.TabIndex = 9;
            this.chart_InspectionEfficiency.Text = "chart2";
            this.chart_InspectionEfficiency.Title = "产线检测效率";
            this.chart_InspectionEfficiency.TitleColor = null;
            this.chart_InspectionEfficiency.TitleFont = null;
            // 
            // gridPanel_Card
            // 
            this.gridPanel_Card.BackExtend = "";
            this.gridPanel_Card.Controls.Add(this.panel4);
            this.gridPanel_Card.Controls.Add(this.panel3);
            this.gridPanel_Card.Controls.Add(this.panel2);
            this.gridPanel_Card.Controls.Add(this.panel1);
            this.gridPanel_Card.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel_Card.Location = new System.Drawing.Point(1235, 87);
            this.gridPanel_Card.Name = "gridPanel_Card";
            this.gridPanel_Card.Size = new System.Drawing.Size(194, 320);
            this.gridPanel_Card.Span = "25%:100%;\r\n25%: 100%;\r\n25%:100%;\r\n25%: 100%\r\n\r\n";
            this.gridPanel_Card.TabIndex = 8;
            this.gridPanel_Card.Text = "gridPanel2";
            // 
            // panel4
            // 
            this.panel4.BorderWidth = 1F;
            this.panel4.Controls.Add(this.label_passRate);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Location = new System.Drawing.Point(3, 243);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(188, 74);
            this.panel4.TabIndex = 3;
            this.panel4.Text = "panel4";
            // 
            // label_passRate
            // 
            this.label_passRate.BackColor = System.Drawing.Color.Transparent;
            this.label_passRate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_passRate.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_passRate.Location = new System.Drawing.Point(2, 27);
            this.label_passRate.Name = "label_passRate";
            this.label_passRate.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label_passRate.Size = new System.Drawing.Size(184, 45);
            this.label_passRate.Suffix = "%";
            this.label_passRate.TabIndex = 3;
            this.label_passRate.Text = "0";
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Dock = System.Windows.Forms.DockStyle.Top;
            this.label8.Location = new System.Drawing.Point(2, 2);
            this.label8.Name = "label8";
            this.label8.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label8.Size = new System.Drawing.Size(184, 25);
            this.label8.TabIndex = 2;
            this.label8.Text = "平均合格率";
            this.label8.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // panel3
            // 
            this.panel3.BorderWidth = 1F;
            this.panel3.Controls.Add(this.label_NGCount);
            this.panel3.Controls.Add(this.label6);
            this.panel3.Location = new System.Drawing.Point(3, 163);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(188, 74);
            this.panel3.TabIndex = 2;
            this.panel3.Text = "panel3";
            // 
            // label_NGCount
            // 
            this.label_NGCount.BackColor = System.Drawing.Color.Transparent;
            this.label_NGCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_NGCount.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_NGCount.Location = new System.Drawing.Point(2, 27);
            this.label_NGCount.Name = "label_NGCount";
            this.label_NGCount.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label_NGCount.Size = new System.Drawing.Size(184, 45);
            this.label_NGCount.TabIndex = 3;
            this.label_NGCount.Text = "0";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.Location = new System.Drawing.Point(2, 2);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label6.Size = new System.Drawing.Size(184, 25);
            this.label6.TabIndex = 2;
            this.label6.Text = "不良品数";
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // panel2
            // 
            this.panel2.BorderWidth = 1F;
            this.panel2.Controls.Add(this.label_InspectionCount);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(3, 83);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(188, 74);
            this.panel2.TabIndex = 1;
            this.panel2.Text = "panel2";
            // 
            // label_InspectionCount
            // 
            this.label_InspectionCount.BackColor = System.Drawing.Color.Transparent;
            this.label_InspectionCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_InspectionCount.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_InspectionCount.Location = new System.Drawing.Point(2, 27);
            this.label_InspectionCount.Name = "label_InspectionCount";
            this.label_InspectionCount.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label_InspectionCount.Size = new System.Drawing.Size(184, 45);
            this.label_InspectionCount.TabIndex = 3;
            this.label_InspectionCount.Text = "0";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(2, 2);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label4.Size = new System.Drawing.Size(184, 25);
            this.label4.TabIndex = 2;
            this.label4.Text = "累计检测数";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // panel1
            // 
            this.panel1.BackExtend = "";
            this.panel1.BorderWidth = 1F;
            this.panel1.Controls.Add(this.label_workOrderCount);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(188, 74);
            this.panel1.TabIndex = 0;
            this.panel1.Text = "panel1";
            // 
            // label_workOrderCount
            // 
            this.label_workOrderCount.BackColor = System.Drawing.Color.Transparent;
            this.label_workOrderCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label_workOrderCount.Font = new System.Drawing.Font("Microsoft YaHei UI", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_workOrderCount.Location = new System.Drawing.Point(2, 27);
            this.label_workOrderCount.Name = "label_workOrderCount";
            this.label_workOrderCount.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label_workOrderCount.Size = new System.Drawing.Size(184, 45);
            this.label_workOrderCount.TabIndex = 1;
            this.label_workOrderCount.Text = "0";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(2, 2);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.label1.Size = new System.Drawing.Size(184, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "工单数";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // chart4
            // 
            this.chart4.AxisColor = null;
            this.chart4.ChartType = AntdUI.TChartType.SplineArea;
            this.chart4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart4.GridColor = null;
            this.chart4.LegendBackColor = null;
            this.chart4.LegendBorderColor = null;
            this.chart4.Location = new System.Drawing.Point(11, 87);
            this.chart4.Name = "chart4";
            this.chart4.PieColors = null;
            this.chart4.Size = new System.Drawing.Size(1202, 320);
            this.chart4.TabIndex = 5;
            this.chart4.Text = "chart_Trend";
            this.chart4.Title = "合格率趋势";
            this.chart4.TitleColor = null;
            this.chart4.TitleFont = null;
            // 
            // stackPanel1
            // 
            this.stackPanel1.Controls.Add(this.button_Reset);
            this.stackPanel1.Controls.Add(this.button_Select);
            this.stackPanel1.Controls.Add(this.datePickerRange);
            this.stackPanel1.Controls.Add(this.label_date);
            this.stackPanel1.Controls.Add(this.input_WorkOrder);
            this.stackPanel1.Controls.Add(this.label_workOrder);
            this.stackPanel1.Location = new System.Drawing.Point(11, 11);
            this.stackPanel1.Name = "stackPanel1";
            this.stackPanel1.Size = new System.Drawing.Size(1418, 54);
            this.stackPanel1.TabIndex = 0;
            this.stackPanel1.Text = "stackPanel1";
            // 
            // button_Reset
            // 
            this.button_Reset.Dock = System.Windows.Forms.DockStyle.Left;
            this.button_Reset.Location = new System.Drawing.Point(991, 3);
            this.button_Reset.Name = "button_Reset";
            this.button_Reset.Size = new System.Drawing.Size(128, 48);
            this.button_Reset.TabIndex = 5;
            this.button_Reset.Text = "重置";
            // 
            // button_Select
            // 
            this.button_Select.Dock = System.Windows.Forms.DockStyle.Left;
            this.button_Select.Location = new System.Drawing.Point(857, 3);
            this.button_Select.Name = "button_Select";
            this.button_Select.Size = new System.Drawing.Size(128, 48);
            this.button_Select.TabIndex = 4;
            this.button_Select.Text = "筛选";
            this.button_Select.Type = AntdUI.TTypeMini.Primary;
            // 
            // datePickerRange
            // 
            this.datePickerRange.Dock = System.Windows.Forms.DockStyle.Left;
            this.datePickerRange.Location = new System.Drawing.Point(398, 3);
            this.datePickerRange.Name = "datePickerRange";
            this.datePickerRange.PlaceholderEnd = "结束日期";
            this.datePickerRange.PlaceholderStart = "开始日期";
            this.datePickerRange.Size = new System.Drawing.Size(453, 48);
            this.datePickerRange.TabIndex = 3;
            this.datePickerRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label_date
            // 
            this.label_date.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_date.Location = new System.Drawing.Point(300, 3);
            this.label_date.Name = "label_date";
            this.label_date.Size = new System.Drawing.Size(92, 48);
            this.label_date.TabIndex = 2;
            this.label_date.Text = "检测时间:";
            this.label_date.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // input_WorkOrder
            // 
            this.input_WorkOrder.Dock = System.Windows.Forms.DockStyle.Left;
            this.input_WorkOrder.Location = new System.Drawing.Point(84, 3);
            this.input_WorkOrder.Name = "input_WorkOrder";
            this.input_WorkOrder.PlaceholderText = "输入工单号";
            this.input_WorkOrder.Size = new System.Drawing.Size(210, 48);
            this.input_WorkOrder.TabIndex = 1;
            // 
            // label_workOrder
            // 
            this.label_workOrder.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_workOrder.Location = new System.Drawing.Point(3, 3);
            this.label_workOrder.Name = "label_workOrder";
            this.label_workOrder.Size = new System.Drawing.Size(75, 48);
            this.label_workOrder.TabIndex = 0;
            this.label_workOrder.Text = "工单号:";
            this.label_workOrder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DataReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel_main);
            this.Name = "DataReport";
            this.Size = new System.Drawing.Size(1440, 760);
            this.panel_main.ResumeLayout(false);
            this.gridPanel_main.ResumeLayout(false);
            this.gridPanel_Card.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.stackPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AntdUI.Panel panel_main;
        private AntdUI.GridPanel gridPanel_main;
        private AntdUI.Chart chart_Defect;
        private AntdUI.Chart chart_InspectionEfficiency;
        private AntdUI.GridPanel gridPanel_Card;
        private AntdUI.Chart chart4;
        private AntdUI.StackPanel stackPanel1;
        private AntdUI.Button button_Reset;
        private AntdUI.Button button_Select;
        private AntdUI.DatePickerRange datePickerRange;
        private AntdUI.Label label_date;
        private AntdUI.Input input_WorkOrder;
        private AntdUI.Label label_workOrder;
        private AntdUI.Panel panel4;
        private AntdUI.Panel panel3;
        private AntdUI.Panel panel2;
        private AntdUI.Panel panel1;
        private AntdUI.Label label_workOrderCount;
        private AntdUI.Label label1;
        private AntdUI.Label label_passRate;
        private AntdUI.Label label8;
        private AntdUI.Label label_NGCount;
        private AntdUI.Label label6;
        private AntdUI.Label label_InspectionCount;
        private AntdUI.Label label4;
    }
}
