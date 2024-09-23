using OKR.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Infrastructure
{
    public class MessageWeightUpdate
    {
        public Guid KeyresultId { get; set; }
        public string? Note { get; set; }
        public int? AddedPoints { get; set; }
        public string? CreateBy { get; set; }
    }
}
