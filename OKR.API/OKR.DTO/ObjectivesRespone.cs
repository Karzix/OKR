﻿using MayNghien.Infrastructure.Models;
using OKR.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class ObjectivesRespone : BaseDto
    {
        public string Name { get; set; }
        public DateTime? StartDay { get; set; }
        public DateTime? EndDay { get; set; }
        public TargetType TargetType { get; set; } = 0;
        public string? TargetTypeName { get; set; }
        public Status status { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? ApplicationUserId { get; set; }

        public bool IsPublic { get; set; } = true;
        public bool IsUserObjectives { get; set; } = true;
        public List<KeyResultRespone> KeyResults { get; set; }
        public string? Period {  get; set; }
        public int? Year {  get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastProgressUpdate { get; set; }
        public int Point {  get; set; }
        public int NumberOfPendingUpdates { get; set; }
    }
}