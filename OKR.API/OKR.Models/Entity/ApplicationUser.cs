﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OKR.Models.Entity
{
    public class ApplicationUser : IdentityUser
    {
        [ForeignKey("Department")]
        public Guid? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        //[ForeignKey("Manager")]
        //public string? ManagerId { get; set; }
        //[ForeignKey("ManagerId")]
        //public ApplicationUser? Manager { get; set; }
        public string? ManagerName { get; set; }

        ICollection<ApplicationUser>? Employees { get; set; }

        public bool IsLocked { get; set; } = false;
    }
}
