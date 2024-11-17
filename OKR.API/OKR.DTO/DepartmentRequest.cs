using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class DepartmentRequest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentDepartmentId { get; set; }
    }
}
