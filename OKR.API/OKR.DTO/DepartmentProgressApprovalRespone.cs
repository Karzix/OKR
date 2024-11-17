using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class DepartmentProgressApprovalRespone : BaseDto
    {
        public Guid KeyresultID { get; set; }
        public string Note { get; set; } = "";
        public int AddedPoints { get; set; } = 0;
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public bool IsApproved { get; set; }
    }
}
