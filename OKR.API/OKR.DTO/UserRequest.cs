using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO
{
    public class UserRequest
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? ManagerName { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}
