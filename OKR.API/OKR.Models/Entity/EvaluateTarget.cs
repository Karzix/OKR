using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Infrastructure.Models.Entity;

namespace OKR.Models.Entity
{
    public class EvaluateTarget : BaseEntity
    {
        [ForeignKey("Objectives")]
        public Guid ObjectivesId { get; set; }
        [ForeignKey("ObjectivesId")]
        public Objectives? Objectives { get; set; }


        public string Content { get; set; }
    }
}
