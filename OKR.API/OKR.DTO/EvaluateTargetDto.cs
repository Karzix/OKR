﻿using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class EvaluateTargetDto: BaseDto
    {
        public Guid? DepartmentObjectivesId { get; set; }
        public Guid? UserObjectivesId { get; set; }
        public string Content { get; set; }
        public DateTime? CreateOn { get; set; }
        public string? CreateBy { get; set; }
    }
}
