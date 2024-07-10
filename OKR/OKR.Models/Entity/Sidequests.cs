using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OKR.Common.Models.Entity;

namespace OKR.Models.Entity
{
    public class Sidequests : BaseEntity
    {
        public string Name { get; set; }
        public bool Status { get; set; } = false;
        [ForeignKey("Objective")]
        public Guid ObjectiveId { get; set; }
        [ForeignKey("ObjectiveId")]
        public Objective? Objective { get; set; }
    }
}
