﻿using Maynghien.Infrastructure.Repository;
using OKR.Models.Context;
using OKR.Models.Entity;
using OKR.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKR.Repository.Implementation
{
    public class SidequestsRepository : GenericRepository<Sidequests, OKRDBContext, ApplicationUser>, ISidequestsRepository
    {
        public SidequestsRepository(OKRDBContext unitOfWork) : base(unitOfWork)
        {
        }
    }
}
