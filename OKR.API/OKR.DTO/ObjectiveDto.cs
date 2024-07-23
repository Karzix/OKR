using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class ObjectiveDto : BaseDto
    {
        public string Name { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime Deadline { get; set; }
        //public int MaximunPoint { get; set; }
        public Guid? TargetTypeId { get; set; }
        public string? TargetTypeName { get; set; }
        public List<KeyResultDto>? ListKeyResults { get; set; }
    }
}
