using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Infrastructure.Models.Entity;

namespace OKR.Models.Entity
{
    public class Objectives : BaseEntity
    {
        public string Name { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime Deadline { get; set; }
        //public int MaximunPoint { get; set; }

        [ForeignKey("TargetType")]
        public Guid TargetTypeId { get; set; }
        [ForeignKey("TargetTypeId")]
        public TargetType? TargetType { get; set; }
    }
}
