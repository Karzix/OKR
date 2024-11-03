using MayNghien.Infrastructure.Models;
using OKR.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace OKR.DTO
{
    public class KeyResultDto : BaseDto
    {
        public string? Description { get; set; }
        public bool? Active { get; set; } = true;
        public DateTime? Deadline { get; set; }
        public TypeUnitKeyResult? Unit { get; set; } // % Percent, # value, 01 bool
        public int? CurrentPoint { get; set; }
        public int? MaximunPoint { get; set; }
        public string? Note { get; set; }
        public int? AddedPoints { get; set; }
        public int? Percentage {  get; set; }
    }
}
