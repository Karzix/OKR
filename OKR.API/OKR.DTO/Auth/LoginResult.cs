﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.DTO.Auth
{
    public class LoginResult
    {
        public string? UserName { get; set; }
        public List<string>? Roles { get; set; }
        public string? RefreshToken { get; set; }
        public string? Token { get; set; }
    }
}
