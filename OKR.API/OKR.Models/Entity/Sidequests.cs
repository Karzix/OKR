using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Infrastructure.Models.Entity;

namespace OKR.Models.Entity
{
    public class Sidequests : BaseEntity
    {
        public string Name { get; set; }
        public bool Status { get; set; } = false;
        [ForeignKey("KeyResults")]
        public Guid KeyResultsId { get; set; }
        [ForeignKey("KeyResultsId")]
        public KeyResults? KeyResults { get; set; }
    }
}
