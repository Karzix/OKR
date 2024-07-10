using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using OKR.Common.Models.Entity;

namespace OKR.Models.Entity
{
    public class KeyResults : BaseEntity
    {
        public string Description { get; set; }
        public bool Active { get; set; } = true;
        public DateTime Deadline { get; set; }
        public string? Unit {  get; set; } // % Percent, # value
        public int CurrentPoint { get; set; }
        public int MaximunPoint { get; set; }


        [ForeignKey("Objective")]
        public Guid ObjectiveId { get; set; }
        [ForeignKey("ObjectiveId")]
        public Objective? Objective { get; set; }
        public KeyResults()
        {
            if(this.Unit.IsNullOrEmpty())
            {
                this.Unit = "%";
            }

        }
    }
}
