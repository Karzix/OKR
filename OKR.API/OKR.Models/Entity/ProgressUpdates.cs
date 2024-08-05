using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Infrastructure.Models.Entity;

namespace OKR.Models.Entity
{
    public class ProgressUpdates : BaseEntity
    {
        public string? Note {  get; set; }

        [ForeignKey("KeyResults")]
        public Guid KeyResultId { get; set; }
        [ForeignKey("KeyResultId")]
        public KeyResults? KeyResults { get; set; }

        public int NewPoint { get; set; }
        public int OldPoint { get; set; }
    }
}
