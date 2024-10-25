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
    public class UserObjectives : BaseEntity
    {

        [ForeignKey("Objectives")]
        public Guid ObjectivesId { get; set; }
        [ForeignKey("ObjectivesId")]
        public Objectives? Objectives { get; set; }


        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
