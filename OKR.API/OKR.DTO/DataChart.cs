using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class DataChart
    {
        public int? OldPoint { get; set; }
        public int? NewPoint { get; set; }
        public string Label { get; set;}
        public DateTime? Date { get; set; }
        public string UserName { get; set; }
        public int ObjectivesCompletionRate { get; set; }
        public int KeyresultCompletionRate { get; set; }
    }
}
