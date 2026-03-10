using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wedjat.Model.DTO
{
    public class ReportSummaryDTO
    {
        public string WorkOrderCode { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ReportCardDataDTO CardData { get; set; }
        public List<ReportTrendDataDTO> TrendData { get; set; }
        public List<ReportDefectDataDTO> DefectData { get; set; }
        public double InspectionEfficiency { get; set; }
    }
}
