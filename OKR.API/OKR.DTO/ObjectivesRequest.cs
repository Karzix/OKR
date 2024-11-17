using MayNghien.Infrastructure.Models;
using OKR.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class ObjectivesRequest :BaseDto
    {
        public string Name { get; set; }
        public TargetType TargetType { get; set; } = 0;
        public string? TargetTypeName { get; set; }
        public Status status { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? ApplicationUserId { get; set; }

        public bool IsPublic { get; set; } = true;
        public string? Period { get; set; }
        public int? Year { get; set; }
        public List<KeyResultRequest> KeyResults { get; set; }
    }
}
