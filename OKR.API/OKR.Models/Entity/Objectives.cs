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
        public DateTime EndDay { get; set; }
        public TargetType TargetType { get; set; } = 0;
        public ObjectivesStatusClose? StatusClose { get; set; }
        public Status status { get; set; } = Status.noStatus;
        [ForeignKey("Department")]
        public Guid? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }


        [ForeignKey("ApplicationUser")]
        public string? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        public bool IsPublic { get; set; } = true;
        public bool IsUserObjectives { get; set; } = true;

        public ICollection<KeyResults>? KeyResults { get; set; }
        public string Period { get; set; }
        public int Year { get; set; }
    }
}
