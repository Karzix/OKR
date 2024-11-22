using MayNghien.Infrastructure.Models.Entity;
using OKR.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Models.Entity
{
    public class DepartmentProgressApproval : BaseEntity
    {
        public Guid KeyResultsId { get; set; }
        // Navigation property to KeyResults
        [ForeignKey(nameof(KeyResultsId))] // Sửa tên thành KeyResultsId
        public virtual KeyResults? KeyResults { get; set; }
        public string Note { get; set; } = "";
        public int AddedPoints { get; set; } = 0;
        public TypeUnitKeyResult Unit { get; set; } 
        public bool IsCompleted { get; set; } = false;
    }
}
