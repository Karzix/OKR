using MayNghien.Infrastructure.Models;
using OKR.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class KeyResultRequest : BaseDto
    {
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; } = false;
        public TypeUnitKeyResult? Unit { get; set; } // % Percent, # value, 01 bool
        public int? CurrentPoint { get; set; }
        public int? MaximunPoint { get; set; }
        public string? Note { get; set; }
        public int? AddedPoints { get; set; }
        public int? Percentage { get; set; }
        public Status Status { get; set; }
        public Guid? ObjectivesId { get; set; }
        public DateTime? EndDay { get; set; }
        public DateTime? StartDay { get; set; }
    }
}
