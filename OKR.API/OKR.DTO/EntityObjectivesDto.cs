using MayNghien.Infrastructure.Models;
using OKR.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class EntityObjectivesDto : BaseDto
    {
        public string Name { get; set; }
        public Quarter Quarter { get; set; }
        public int Year { get; set; }
        //public int MaximunPoint { get; set; }
        public TargetType? TargetType { get; set; }
        public string? TargetTypeName { get; set; }
        public List<KeyResultRespone>? ListKeyResults { get; set; }
        public double? Point { get; set; }
        public Guid? ObjectivesId { get; set; }
        public string? CreateBy { get; set; }
        public Status? Status { get; set; }
        public int? NumberOfPendingUpdates { get; set; }
    }
}
