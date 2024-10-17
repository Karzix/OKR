﻿using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class DepartmentProgressApprovalDto : BaseDto
    {
        public Guid KeuresultID { get; set; }
        public string Note { get; set; } = "";
        public int AddedPoints = 0;
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
    }
}
