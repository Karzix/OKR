using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OKR.Common.Models.Entity;

namespace OKR.Models.Entity
{
    public class Department : BaseEntity
    {
        public string Name { get; set; }

        [ForeignKey("Department")]
        public Guid? ParentDepartmentId { get; set; }
        [ForeignKey("ParentDepartmentId")]
        public Department? ParentDepartment { get; set; }
    }
}
