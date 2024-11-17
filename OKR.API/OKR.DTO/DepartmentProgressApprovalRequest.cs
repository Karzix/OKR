using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class DepartmentProgressApprovalRequest : BaseDto
    {
        //public Guid KeyresultID { get; set; }
        //public string Note { get; set; } = "";
        //public int AddedPoints { get; set; } = 0;
        public bool IsApproved { get; set; }
    }
}
