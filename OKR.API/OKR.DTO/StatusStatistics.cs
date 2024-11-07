using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class StatusStatistics
    {
        public int noStatus {  get; set; }
        public int onTrack { get; set; }
        public int atRisk { get; set; }
        public int offTrack { get; set; }
        public int closed { get; set; }
        public int total { get; set; }
    }
}
