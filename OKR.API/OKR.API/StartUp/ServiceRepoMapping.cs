﻿using OKR.Service.Contract;
using OKR.Service.Implementation;

namespace OKR.API.StartUp
{
    public class ServiceRepoMapping
    {
        public ServiceRepoMapping() { }
        public void Mapping(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IAuthencationService, AuthencationService>();   
        }
    }
}
