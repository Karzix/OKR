using MayNghien.Infrastructure.Models;
using OKR.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class ProgressUpdatesDto : BaseDto
    {
        public string? Note { get; set; }
        public Guid? KeyResultId { get; set; }
        public int? NewPoint { get; set; }
        public int? OldPoint { get; set; }
        public DateTime? CreateOn { get; set; }
        public string? CreateBy { get; set; }
        public TypeUnitKeyResult? Unit { get; set; }
        
    }
}
