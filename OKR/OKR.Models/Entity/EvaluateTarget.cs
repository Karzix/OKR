using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OKR.Common.Models.Entity;

namespace OKR.Models.Entity
{
    public class EvaluateTarget : BaseEntity
    {
        [ForeignKey("DepartmentObjectives")]
        public Guid? DepartmentObjectivesId { get; set; }
        [ForeignKey("DepartmentObjectivesId")]
        public DepartmentObjectives? DepartmentObjectives { get; set; }

        [ForeignKey("UserObjectives")]
        public Guid? UserObjectivesId { get; set; }
        [ForeignKey("UserObjectivesId")]
        public UserObjectives? UserObjectives { get; set; }


        public string Content { get; set; }
    }
}
