using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Infrastructure.Models.Entity;
using OKR.Infrastructure.Enum;

namespace OKR.Models.Entity
{
    public class DepartmentObjectives : BaseEntity
    {
        public StatusObjectives status { get; set; } = StatusObjectives.working;

        [ForeignKey("Objectives")]
        public Guid ObjectivesId { get; set; }
        [ForeignKey("ObjectivesId")]
        public Objectives? Objectives { get; set; }


        [ForeignKey("Department")]
        public Guid DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
    }
}
