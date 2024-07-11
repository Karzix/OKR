using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MayNghien.Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OKR.Models.Entity;

namespace OKR.Models.Context
{
    public class OKRDBContext : BaseContext<ApplicationUser>
    {
        public OKRDBContext()
        {

        }
        public OKRDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Department> Department { get; set; }
        public DbSet<DepartmentObjectives> DepartmentObjectives { get; set; }
        public DbSet<EvaluateTarget> EvaluateTarget { get; set; }
        public DbSet<Objective> Objective { get; set; }
        public DbSet<KeyResults> KeyResults { get; set; }
        public DbSet<ProgressUpdates> ProgressUpdates { get; set; }
        public DbSet<Sidequests> Sidequests { get; set; }
        public DbSet<TargetType> TargetType { get; set; }
        public DbSet<UserObjectives> UserObjectives { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var appSetting = JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText("appsettings.json"));
                optionsBuilder.UseSqlServer(appSetting.ConnectionString);
            }


        }
    }
}
