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
    public class Objectives : BaseEntity
    {
        public string Name { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime Deadline { get; set; }
        public TargetType TargetType { get; set; } = 0;
        public StatusObjectives status { get; set; } = StatusObjectives.working;
        public ICollection<UserObjectives>? UserObjectives { get; set; }
        public ICollection<DepartmentObjectives>? DepartmentObjectives { get; set; }
    }
}
