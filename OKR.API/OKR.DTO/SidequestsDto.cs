using MayNghien.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class SidequestsDto : BaseDto
    {
        public string Name { get; set; }
        public bool Status { get; set; } = false;
        public Guid? KeyResultsId { get; set; }


    }
}
