using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class EvaluateTargetRequest :BaseDto
    {
        public Guid? ObjectivesId { get; set; }
        public string Content { get; set; }
    }
}
