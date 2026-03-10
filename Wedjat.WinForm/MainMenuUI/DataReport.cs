using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wedjat.BLL;
using Wedjat.Model.DTO;

namespace Wedjat.WinForm.MainMenuUI
{
    #region  纯AI生成的代码，勿参考，AntdUI中没有Chart的使用方法，我也不会，就是单纯为了使用AntdUI的Chart控件而使用，不如LiveChart
    public partial class DataReport : UserControl
    {
        private ReportBLL _reportBLL;
        private int _chartMode = 0; // 图表模式：0=折线图，1=堆积柱状图
        private int _defectChartType = 0; // 缺陷图表模式：0=饼图，1=柱状图

        // 缺陷图表颜色定义
        private readonly Color[] _defectChartColors = new Color[]
        {
            Color.FromArgb(255, 99, 132),   // 红色
            Color.FromArgb(54, 162, 235),   // 蓝色
            Color.FromArgb(255, 206, 86),   // 黄色
            Color.FromArgb(75, 192, 192),   // 青色
            Color.FromArgb(153, 102, 255),  // 紫色
            Color.FromArgb(255, 159, 64),   // 橙色
            Color.FromArgb(199, 199, 199),  // 灰色
            Color.FromArgb(83, 102, 255),   // 深蓝
            Color.FromArgb(40, 159, 64),    // 深绿
            Color.FromArgb(210, 199, 199)   // 浅灰
        };

        // 趋势图颜色定义
        private readonly Color _totalColor = Color.FromArgb(52, 152, 219);    // 蓝色
        private readonly Color _okColor = Color.FromArgb(46, 204, 113);       // 绿色
        private readonly Color _ngColor = Color.FromArgb(231, 76, 60);        // 红色

        public DataReport()
        {
            InitializeComponent();

            // 初始化BLL层
            InitializeBLL();

            // 初始化事件
            InitializeEvents();

            // 初始化图表样式
            InitializeCharts();

            // 添加图表切换按钮
            AddChartToggleButtons();

            // 初始化标题
            UpdateStatsTitle();
        }

        #region 初始化方法

        private void InitializeBLL()
        {
            try
            {
                _reportBLL = new ReportBLL();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"初始化报表服务失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeEvents()
        {
            button_Select.Click += Button_Select_Click;
            button_Reset.Click += Button_Reset_Click;
            input_WorkOrder.KeyDown += Input_WorkOrder_KeyDown;
            Load += DataReport_Load;

            // 添加日期范围选择事件
            datePickerRange.ValueChanged += DatePickerRange_ValueChanged;
        }

        private void InitializeCharts()
        {
            // 初始化趋势图
            InitializeTrendChart();

            // 初始化缺陷分布图
            InitializeDefectChart();

            // 初始化统计图
            InitializeStatsChart();
        }

        private void InitializeTrendChart()
        {
            chart4.Title = "检测趋势分析";
            chart4.ShowLegend = true;
            chart4.LegendPosition = ContentAlignment.TopRight;
            chart4.ShowGrid = true;
            chart4.ShowAxes = true;
            chart4.EnableAnimation = true;
            chart4.AnimationDuration = 800;
            chart4.GridColor = Color.FromArgb(220, 220, 220);
            chart4.AxisColor = Color.FromArgb(120, 120, 120);
            chart4.ChartType = AntdUI.TChartType.Line;
        }

        private void InitializeDefectChart()
        {
            chart_Defect.Title = "缺陷类型分布";
            chart_Defect.ShowLegend = true;
            chart_Defect.LegendPosition = ContentAlignment.BottomCenter;
            chart_Defect.ShowAxes = false;
            chart_Defect.ShowGrid = false;
            chart_Defect.PieColors = _defectChartColors;
            chart_Defect.EnableAnimation = true;
            chart_Defect.ChartType = AntdUI.TChartType.Pie;
        }

        private void InitializeStatsChart()
        {
            chart_InspectionEfficiency.Title = "产线检测统计";
            chart_InspectionEfficiency.ShowLegend = true;
            chart_InspectionEfficiency.LegendPosition = ContentAlignment.TopRight;
            chart_InspectionEfficiency.ShowGrid = true;
            chart_InspectionEfficiency.ShowAxes = true;
            chart_InspectionEfficiency.GridColor = Color.FromArgb(220, 220, 220);
            chart_InspectionEfficiency.AxisColor = Color.FromArgb(120, 120, 120);
            chart_InspectionEfficiency.ChartType = AntdUI.TChartType.Bar; // 使用普通柱状图
        }

        private void AddChartToggleButtons()
        {
            // 趋势图切换按钮
            var trendModeButton = new AntdUI.Button
            {
                Text = "切换为堆积柱状图",
                Size = new Size(150, 40),
                Location = new Point(1050, 60),
                Type = AntdUI.TTypeMini.Default
            };

            trendModeButton.Click += (sender, e) =>
            {
                _chartMode = (_chartMode + 1) % 2;
                trendModeButton.Text = _chartMode == 0 ? "切换为堆积柱状图" : "切换为折线图";

                // 重新加载趋势数据
                var workOrderCode = input_WorkOrder.Text?.Trim();
                DateTime? startDate = null;
                DateTime? endDate = null;

                var dateRange = datePickerRange.Value;
                if (dateRange != null && dateRange.Length == 2)
                {
                    startDate = dateRange[0];
                    endDate = dateRange[1];
                }

                _ = LoadTrendDataBasedOnMode(workOrderCode, startDate, endDate);
            };

            // 缺陷图切换按钮
            var defectModeButton = new AntdUI.Button
            {
                Text = "切换为柱状图",
                Size = new Size(150, 40),
                Location = new Point(850, 60),
                Type = AntdUI.TTypeMini.Default
            };

            defectModeButton.Click += (sender, e) =>
            {
                _defectChartType = (_defectChartType + 1) % 2;
                defectModeButton.Text = _defectChartType == 0 ? "切换为柱状图" : "切换为饼图";

                // 重新加载缺陷数据
                var workOrderCode = input_WorkOrder.Text?.Trim();
                DateTime? startDate = null;
                DateTime? endDate = null;

                var dateRange = datePickerRange.Value;
                if (dateRange != null && dateRange.Length == 2)
                {
                    startDate = dateRange[0];
                    endDate = dateRange[1];
                }

                _ = LoadDefectData(workOrderCode, startDate, endDate);
            };

            this.Controls.Add(trendModeButton);
            this.Controls.Add(defectModeButton);
        }

        private void UpdateStatsTitle()
        {
            // 根据选择的日期范围更新标题
            var dateRange = datePickerRange?.Value;
            if (dateRange != null && dateRange.Length == 2)
            {
                var startDate = dateRange[0].ToString("yyyy-MM-dd");
                var endDate = dateRange[1].ToString("yyyy-MM-dd");
                chart_InspectionEfficiency.Title = $"产线检测统计 ({startDate} 至 {endDate})";
            }
            else
            {
                chart_InspectionEfficiency.Title = "产线检测统计";
            }
        }

        #endregion

        #region 事件处理方法

        private async void DataReport_Load(object sender, EventArgs e)
        {
            // 设置默认日期范围（最近30天）
            datePickerRange.Value = new DateTime[]
            {
                DateTime.Now.AddDays(-30).Date,
                DateTime.Now.Date
            };

            await LoadData();
        }

        private async void Button_Select_Click(object sender, EventArgs e)
        {
            await LoadData();
        }

        private void Button_Reset_Click(object sender, EventArgs e)
        {
            // 重置工单号
            input_WorkOrder.Text = string.Empty;

            // 重置日期范围到最近30天
            datePickerRange.Value = new DateTime[]
            {
                DateTime.Now.AddDays(-30).Date,
                DateTime.Now.Date
            };

            UpdateStatsTitle();
        }

        private void DatePickerRange_ValueChanged(object sender, AntdUI.DateTimesEventArgs e)
        {
            // 更新标题
            UpdateStatsTitle();

            // 当日期范围改变时，自动加载数据
            if (e.Value != null && e.Value.Length == 2)
            {
                _ = LoadData(); // 异步加载数据
            }
        }

        private async void Input_WorkOrder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                await LoadData();
            }
        }

        #endregion

        #region 数据加载方法

        private async Task LoadData()
        {
            try
            {
                // 显示加载状态
                this.Enabled = false;
                Cursor = Cursors.WaitCursor;

                // 获取筛选条件
                var workOrderCode = input_WorkOrder.Text?.Trim();
                DateTime? startDate = null;
                DateTime? endDate = null;

                // 从DatePickerRange获取日期范围
                var dateRange = datePickerRange.Value;
                if (dateRange != null && dateRange.Length == 2)
                {
                    startDate = dateRange[0];
                    endDate = dateRange[1];
                }
                else
                {
                    // 如果没有选择日期，使用默认的最近30天
                    startDate = DateTime.Now.AddDays(-30);
                    endDate = DateTime.Now;
                }

                // 并行加载所有数据
                var tasks = new Task[]
                {
                    Task.Run(async () => await LoadCardData(workOrderCode, startDate, endDate)),
                    Task.Run(async () => await LoadTrendDataBasedOnMode(workOrderCode, startDate, endDate)),
                    Task.Run(async () => await LoadDefectData(workOrderCode, startDate, endDate)),
                    Task.Run(async () => await LoadStatsData(workOrderCode, startDate, endDate))
                };

                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async Task LoadCardData(string workOrderCode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var cardData = await _reportBLL.GetCardData(workOrderCode, startDate, endDate);

                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateCardUI(cardData)));
                }
                else
                {
                    UpdateCardUI(cardData);
                }
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                        MessageBox.Show($"加载卡片数据失败: {ex.Message}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
        }

        private void UpdateCardUI(ReportCardDataDTO cardData)
        {
            label_workOrderCount.Text = cardData.WorkOrderCount.ToString("N0");
            label_InspectionCount.Text = cardData.TotalInspectionCount.ToString("N0");
            label_NGCount.Text = cardData.DefectiveCount.ToString("N0");
            label_passRate.Text = cardData.AveragePassRate.ToString("F2");

            // 根据合格率设置颜色
            if (cardData.AveragePassRate >= 95)
                label_passRate.ForeColor = Color.FromArgb(46, 204, 113); // 绿色
            else if (cardData.AveragePassRate >= 90)
                label_passRate.ForeColor = Color.FromArgb(241, 196, 15); // 黄色
            else
                label_passRate.ForeColor = Color.FromArgb(231, 76, 60);  // 红色
        }

        private async Task LoadTrendDataBasedOnMode(string workOrderCode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var trendData = await _reportBLL.GetTrendData(workOrderCode, startDate, endDate);

                if (InvokeRequired)
                {
                    Invoke(new Action(() => RenderTrendChart(trendData)));
                }
                else
                {
                    RenderTrendChart(trendData);
                }
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                        MessageBox.Show($"加载趋势数据失败: {ex.Message}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
        }

        private async Task LoadDefectData(string workOrderCode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var defectData = await _reportBLL.GetDefectData(workOrderCode, startDate, endDate);

                if (InvokeRequired)
                {
                    Invoke(new Action(() => RenderDefectChart(defectData)));
                }
                else
                {
                    RenderDefectChart(defectData);
                }
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                        MessageBox.Show($"加载缺陷数据失败: {ex.Message}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
        }

        private async Task LoadStatsData(string workOrderCode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var stats = await _reportBLL.GetInspectionStats(workOrderCode, startDate, endDate);

                if (InvokeRequired)
                {
                    Invoke(new Action(() => RenderStatsChart(stats)));
                }
                else
                {
                    RenderStatsChart(stats);
                }
            }
            catch (Exception ex)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() =>
                        MessageBox.Show($"加载检测统计数据失败: {ex.Message}", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
        }

        #endregion

        #region 图表渲染方法

        private void RenderTrendChart(List<ReportTrendDataDTO> trendData)
        {
            chart4.Datasets.Clear();

            if (trendData == null || !trendData.Any())
            {
                RenderNoDataChart(chart4, "检测趋势分析 (无数据)");
                return;
            }

            if (_chartMode == 0)
            {
                RenderLineChart(trendData);
            }
            else
            {
                RenderStackedBarChart(trendData);
            }
        }

        private void RenderLineChart(List<ReportTrendDataDTO> trendData)
        {
            chart4.ChartType = AntdUI.TChartType.Line;

            // 创建三个数据集
            var totalDataset = new AntdUI.ChartDataset("检测总数")
            {
                FillColor = Color.Transparent,
                BorderColor = _totalColor,
                BorderWidth = 3,
                LegendBoxFillColor = _totalColor,
                Opacity = 1.0f
            };

            var okDataset = new AntdUI.ChartDataset("合格数")
            {
                FillColor = Color.Transparent,
                BorderColor = _okColor,
                BorderWidth = 2,
                LegendBoxFillColor = _okColor,
                Opacity = 0.9f
            };

            var ngDataset = new AntdUI.ChartDataset("不合格数")
            {
                FillColor = Color.Transparent,
                BorderColor = _ngColor,
                BorderWidth = 2,
                LegendBoxFillColor = _ngColor,
                Opacity = 0.9f
            };

            // 添加数据点
            for (int i = 0; i < trendData.Count; i++)
            {
                var data = trendData[i];

                totalDataset.AddPoint(new AntdUI.ChartDataPoint(
                    data.PeriodName,
                    i,
                    data.TotalCount
                ));

                okDataset.AddPoint(new AntdUI.ChartDataPoint(
                    data.PeriodName,
                    i,
                    data.OKCount
                ));

                ngDataset.AddPoint(new AntdUI.ChartDataPoint(
                    data.PeriodName,
                    i,
                    data.NGCount
                ));
            }

            chart4.Datasets.Add(totalDataset);
            chart4.Datasets.Add(okDataset);
            chart4.Datasets.Add(ngDataset);

            chart4.Title = "检测趋势分析 (折线图)";
            chart4.Refresh();
        }

        private void RenderStackedBarChart(List<ReportTrendDataDTO> trendData)
        {
            chart4.ChartType = AntdUI.TChartType.StackedBar;

            // 创建两个数据集用于堆积柱状图
            var okDataset = new AntdUI.ChartDataset("合格数")
            {
                FillColor = _okColor,
                BorderColor = Color.FromArgb(39, 174, 96),
                BorderWidth = 1,
                LegendBoxFillColor = _okColor,
                Opacity = 0.8f
            };

            var ngDataset = new AntdUI.ChartDataset("不合格数")
            {
                FillColor = _ngColor,
                BorderColor = Color.FromArgb(192, 57, 43),
                BorderWidth = 1,
                LegendBoxFillColor = _ngColor,
                Opacity = 0.8f
            };

            // 添加数据点
            for (int i = 0; i < trendData.Count; i++)
            {
                var data = trendData[i];

                okDataset.AddPoint(new AntdUI.ChartDataPoint(
                    data.PeriodName,
                    i,
                    data.OKCount
                ));

                ngDataset.AddPoint(new AntdUI.ChartDataPoint(
                    data.PeriodName,
                    i,
                    data.NGCount
                ));
            }

            chart4.Datasets.Clear();
            chart4.Datasets.Add(okDataset);
            chart4.Datasets.Add(ngDataset);

            chart4.Title = "检测趋势分析 (堆积柱状图)";
            chart4.Refresh();
        }

        private void RenderDefectChart(List<ReportDefectDataDTO> defectData)
        {
            chart_Defect.Datasets.Clear();

            if (defectData == null || !defectData.Any())
            {
                RenderNoDataChart(chart_Defect, "缺陷类型分布 (无数据)");
                return;
            }

           
                RenderDefectPieChart(defectData);
           
        }

        private void RenderDefectPieChart(List<ReportDefectDataDTO> defectData)
        {
            chart_Defect.ChartType = AntdUI.TChartType.Pie;
            chart_Defect.Datasets.Clear();

            if (defectData == null || !defectData.Any())
            {
                RenderNoDataChart(chart_Defect, "缺陷类型分布 (无数据)");
                return;
            }

            // 创建数据集
            var dataset = new AntdUI.ChartDataset("缺陷分布");

            // 处理缺陷数据
            var processedDefects = ProcessDefectDataForDisplay(defectData);
            var totalDefects = processedDefects.Sum(d => d.DefectCount);

            // 添加数据点 - 确保每个数据点都有明确的标签
            for (int i = 0; i < processedDefects.Count; i++)
            {
                var defect = processedDefects[i];
                var color = GetDefectColor(i);

                double percentage = totalDefects > 0 ?
                    (double)defect.DefectCount / totalDefects * 100 : 0;

                // 方案1：标签只显示缺陷名称
                string label = defect.DefectTypeName;

                // 方案2：标签显示缺陷名称和数量
                // string label = $"{defect.DefectTypeName} ({defect.DefectCount})";

                // 方案3：标签显示缺陷名称和百分比
                // string label = $"{defect.DefectTypeName}\n{percentage:F1}%";

                var dataPoint = new AntdUI.ChartDataPoint(
                    label,              // 标签
                    i,                  // X轴位置（饼图可能不使用，但需要占位）
                    defect.DefectCount, // Y轴值（决定扇形大小）
                    color               // 颜色
                );

                dataset.AddPoint(dataPoint);
            }

            // 将数据集添加到图表
            chart_Defect.Datasets.Add(dataset);

            // 配置饼图显示
            ConfigurePieChartDisplay(totalDefects);

            chart_Defect.Refresh();
        }
        private void ConfigurePieChartDisplay(int totalDefects)
        {
            // 显示图例
            chart_Defect.ShowLegend = true;
            chart_Defect.LegendPosition = ContentAlignment.BottomCenter;
          
            // 设置标题
            chart_Defect.Title = $"缺陷类型分布\n总计: {totalDefects} 个缺陷";
        }

        private void RenderStatsChart(ReportStatsDTO stats)
        {
            chart_InspectionEfficiency.Datasets.Clear();

            if (stats == null || stats.InspectionCount == 0)
            {
                RenderNoDataChart(chart_InspectionEfficiency, "产线检测统计 (无数据)");
                return;
            }

            // 使用一个数据集显示三个数据点
            var dataset = new AntdUI.ChartDataset("检测统计")
            {
                FillColor = Color.FromArgb(52, 152, 219),
                BorderColor = Color.FromArgb(41, 128, 185),
                BorderWidth = 2,
                Opacity = 0.9f
            };

            // 添加三个数据点：检测总数、合格数、不合格数
            dataset.AddPoint(new AntdUI.ChartDataPoint(
                $"检测总数\n{stats.InspectionCount:N0}片",
                0,
                stats.InspectionCount
            ));

            dataset.AddPoint(new AntdUI.ChartDataPoint(
                $"合格数\n{stats.OKCount:N0}片",
                1,
                stats.OKCount
            ));

            dataset.AddPoint(new AntdUI.ChartDataPoint(
                $"不合格数\n{stats.NGCount:N0}片",
                2,
                stats.NGCount
            ));

            chart_InspectionEfficiency.Datasets.Add(dataset);

            // 设置图表选项
            chart_InspectionEfficiency.ShowLegend = true;
            chart_InspectionEfficiency.LegendPosition = ContentAlignment.TopRight;
            chart_InspectionEfficiency.ShowGrid = true;
            chart_InspectionEfficiency.ShowAxes = true;
            chart_InspectionEfficiency.GridColor = Color.FromArgb(220, 220, 220);
            chart_InspectionEfficiency.AxisColor = Color.FromArgb(120, 120, 120);

            // 更新标题
            UpdateStatsChartTitle(stats);

            chart_InspectionEfficiency.Refresh();
        }

        #endregion

        #region 辅助方法

        private void RenderNoDataChart(AntdUI.Chart chart, string title)
        {
            chart.Datasets.Clear();

            var emptyDataset = new AntdUI.ChartDataset("无数据")
            {
                FillColor = Color.LightGray,
                BorderColor = Color.Gray,
                BorderWidth = 1
            };

            emptyDataset.AddPoint(new AntdUI.ChartDataPoint("无数据", 0, 0));
            chart.Datasets.Add(emptyDataset);
            chart.Title = title;
            chart.Refresh();
        }

        private List<ReportDefectDataDTO> ProcessDefectDataForDisplay(List<ReportDefectDataDTO> defectData)
        {
            var result = new List<ReportDefectDataDTO>();

            // 取前7个主要缺陷（饼图显示不宜太多）
            var topDefects = defectData
                .Where(d => !string.IsNullOrEmpty(d.DefectTypeName))
                .Take(5)
                .ToList();

            // 添加主要缺陷
            result.AddRange(topDefects);

            // 如果有其他缺陷，合并为"其他"
            if (defectData.Count > 5)
            {
                var otherDefects = defectData.Skip(5).ToList();
                int otherCount = otherDefects.Sum(d => d.DefectCount);
                double otherPercentage = otherDefects.Sum(d => d.DefectPercentage);

                result.Add(new ReportDefectDataDTO
                {
                    DefectTypeName = "其他缺陷",
                    DefectCount = otherCount,
                    DefectPercentage = otherPercentage
                });
            }

            return result;
        }

        private Color GetDefectColor(int index)
        {
            if (index < _defectChartColors.Length)
            {
                return _defectChartColors[index];
            }

            // 生成随机颜色
            var random = new Random(index * 100);
            return Color.FromArgb(
                random.Next(100, 255),
                random.Next(100, 255),
                random.Next(100, 255)
            );
        }

        private string FormatDefectLabel(ReportDefectDataDTO defect, bool forPieChart)
        {
            if (forPieChart)
            {
                // 饼图标签需要简短
                if (defect.DefectTypeName.Length > 8)
                {
                    return defect.DefectTypeName.Substring(0, 6) + "...";
                }
                return defect.DefectTypeName;
            }
            else
            {
                // 柱状图标签可以稍长
                if (defect.DefectTypeName.Length > 12)
                {
                    return defect.DefectTypeName.Substring(0, 10) + "...";
                }
                return defect.DefectTypeName;
            }
        }

        private string FormatDefectTooltip(ReportDefectDataDTO defect)
        {
            return $"{defect.DefectTypeName}\n" +
                   $"数量: {defect.DefectCount}\n" +
                   $"占比: {defect.DefectPercentage:F1}%";
        }

        private void UpdateDefectChartTitle(int totalDefects)
        {
            var dateRange = datePickerRange.Value;
            if (dateRange != null && dateRange.Length == 2)
            {
                var startDate = dateRange[0].ToString("yyyy-MM-dd");
                var endDate = dateRange[1].ToString("yyyy-MM-dd");
                chart_Defect.Title = $"缺陷类型分布 ({startDate} 至 {endDate})";
            }
            else
            {
                chart_Defect.Title = "缺陷类型分布";
            }

            // 添加总数信息
            chart_Defect.Title += $"\n总计: {totalDefects} 个缺陷";
        }

        private void UpdateStatsChartTitle(ReportStatsDTO stats)
        {
            var dateRange = datePickerRange.Value;
            if (dateRange != null && dateRange.Length == 2)
            {
                var startDate = dateRange[0].ToString("yyyy-MM-dd");
                var endDate = dateRange[1].ToString("yyyy-MM-dd");
                chart_InspectionEfficiency.Title = $"产线检测统计 ({startDate} 至 {endDate})";
            }
            else
            {
                chart_InspectionEfficiency.Title = "产线检测统计";
            }

            // 添加统计信息
            chart_InspectionEfficiency.Title += $"\n合格率: {stats.PassRate:F2}% | 效率: {stats.Efficiency:F2} 个/小时";
        }

        #endregion

        #region 公共方法

        public async void RefreshData()
        {
            await LoadData();
        }

        public void ExportToExcel()
        {
            try
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Excel文件 (*.xlsx)|*.xlsx";
                    saveDialog.Title = "导出报表数据";
                    saveDialog.FileName = $"PCB检测报表_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show($"数据已导出到: {saveDialog.FileName}",
                            "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void PrintReport()
        {
            try
            {
                // 创建打印预览
                var printDialog = new PrintDialog();
                var printDocument = new System.Drawing.Printing.PrintDocument();

                printDialog.Document = printDocument;
                printDocument.PrintPage += (sender, e) =>
                {
                    // 绘制报表内容到打印页面
                    e.Graphics.DrawString("PCB检测报表",
                        new Font("微软雅黑", 16, FontStyle.Bold),
                        Brushes.Black,
                        new PointF(100, 100));
                };

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印失败: {ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 工具方法

        private string FormatNumber(long number)
        {
            if (number >= 1000000)
                return $"{(number / 1000000.0):F1}M";
            else if (number >= 1000)
                return $"{(number / 1000.0):F1}K";
            else
                return number.ToString();
        }

        #endregion

        #region 日期范围处理

        /// <summary>
        /// 获取当前选择的日期范围
        /// </summary>
        public (DateTime? StartDate, DateTime? EndDate) GetDateRange()
        {
            var dateRange = datePickerRange.Value;
            if (dateRange != null && dateRange.Length == 2)
            {
                return (dateRange[0], dateRange[1]);
            }
            return (null, null);
        }

        /// <summary>
        /// 设置日期范围
        /// </summary>
        public void SetDateRange(DateTime startDate, DateTime endDate)
        {
            datePickerRange.Value = new DateTime[] { startDate, endDate };
            UpdateStatsTitle();
        }

        /// <summary>
        /// 设置日期范围为最近N天
        /// </summary>
        public void SetRecentDays(int days)
        {
            datePickerRange.Value = new DateTime[]
            {
                DateTime.Now.AddDays(-days).Date,
                DateTime.Now.Date
            };
            UpdateStatsTitle();
        }

        /// <summary>
        /// 设置日期范围为最近一个月
        /// </summary>
        public void SetRecentMonth()
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            datePickerRange.Value = new DateTime[]
            {
                startDate.Date,
                DateTime.Now.Date
            };
            UpdateStatsTitle();
        }

        /// <summary>
        /// 设置日期范围为最近一周
        /// </summary>
        public void SetRecentWeek()
        {
            var startDate = DateTime.Now.AddDays(-7);
            datePickerRange.Value = new DateTime[]
            {
                startDate.Date,
                DateTime.Now.Date
            };
            UpdateStatsTitle();
        }

        #endregion
    }
    #endregion
}