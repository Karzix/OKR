﻿using MayNghien.Infrastructure.Models;
using OKR.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace OKR.DTO
{
    public class KeyResultDto : BaseDto
    {
        public string? Description { get; set; }
        public bool? Active { get; set; } = true;
        public TypeUnitKeyResult? Unit { get; set; } // % Percent, # value, 01 bool
        public int? CurrentPoint { get; set; }
        public int? MaximunPoint { get; set; }
        public string? Note { get; set; }
        public int? AddedPoints { get; set; }
        public int? Percentage {  get; set; }
        public StatusObjectives Status { get; set; }
        public List<ProgressUpdatesDto>? ProgressUpdates { get; set; }
        public Guid? ObjectivesId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastProgressUpdate { get; set; }
        public DateTime? EndDay { get; set; }
        public DateTime? StartDay { get; set; }
    }
}
