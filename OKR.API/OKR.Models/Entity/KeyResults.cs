using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using MayNghien.Infrastructure.Models.Entity;
using OKR.Infrastructure.Enum;

namespace OKR.Models.Entity
{
    public class KeyResults : BaseEntity
    {
        public string Description { get; set; }
        public bool Active { get; set; } = true;
        public DateTime Deadline { get; set; }
        public TypeUnitKeyResult? Unit {  get; set; } // % Percent, # value
        public int CurrentPoint { get; set; }
        public int MaximunPoint { get; set; }
        public int Percentage { get; set; }
        public StatusObjectives Status { get; set; } = StatusObjectives.noStatus;


        [ForeignKey("Objectives")]
        public Guid ObjectivesId { get; set; }
        [ForeignKey("ObjectivesId")]
        public Objectives? Objectives { get; set; }
        public KeyResults()
        {
            if(this.Unit == null)
            {
                this.Unit = TypeUnitKeyResult.Percent;
            }

        }
    }
}
