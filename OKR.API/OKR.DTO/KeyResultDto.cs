﻿using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class KeyResultDto : BaseDto
    {
        public string Description { get; set; }
        public bool? Active { get; set; } = true;
        public DateTime? Deadline { get; set; }
        public string? Unit { get; set; } // % Percent, # value, 01 bool
        public int? CurrentPoint { get; set; }
        public int? MaximunPoint { get; set; }

        public List<SidequestsDto>? Sidequests { get; set; }
    }
}