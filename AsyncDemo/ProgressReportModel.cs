using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncDemo
{
    public class ProgressReportModel
    {
        public List<DataModel> SitesDownloaded { get; set; } = new List<DataModel>();
        public int PercentageComplete { get; set; } = 0;
    }
}
 