using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant.Abstractions;
using System;
using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Data
{

    //public class ApplicationDbContext : IdentityDbContext
    //{
    //    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    //        : base(options)
    //    {
    //    }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Database=DatabaseTenant1;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;MultipleActiveResultSets=true");
    //        base.OnConfiguring(optionsBuilder);
    //    }
    //}

    public class ApplicationDbContext : MultiTenantIdentityDbContext
    {
        private readonly AppTenantInfo _tenantInfo;

        public ApplicationDbContext(IMultiTenantContextAccessor multiTenantContextAccessor) : base(multiTenantContextAccessor)
        {
            //_tenantInfo = multiTenantContextAccessor.MultiTenantContext.TenantInfo as AppTenantInfo;
            //if (multiTenantContextAccessor.MultiTenantContext?.TenantInfo != null)
            //{
            //    _tenantInfo = multiTenantContextAccessor.MultiTenantContext.TenantInfo as AppTenantInfo;
            //}
            //else
            //{
            //    _tenantInfo = new AppTenantInfo
            //    {
            //        ConnectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=DatabaseTenant1;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;MultipleActiveResultSets=true"
            //    };
            //}
        }

        public ApplicationDbContext(IMultiTenantContextAccessor multiTenantContextAccessor, DbContextOptions<ApplicationDbContext> options) :
            base(multiTenantContextAccessor, options)
        {
            //if (multiTenantContextAccessor.MultiTenantContext?.TenantInfo != null)
            //{
            //    _tenantInfo = multiTenantContextAccessor.MultiTenantContext.TenantInfo as AppTenantInfo;
            //}
            //else
            //{
            //    _tenantInfo = new AppTenantInfo
            //    {
            //        ConnectionString = "Data Source=(localdb)\\ProjectModels;Initial Catalog=DatabaseTenant1;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;MultipleActiveResultSets=true"
            //    };
            //}
        }

        public ApplicationDbContext(AppTenantInfo tenantInfo) : base(tenantInfo)
        {
            _tenantInfo = tenantInfo;
        }

        public ApplicationDbContext(AppTenantInfo tenantInfo, DbContextOptions options) : base(tenantInfo, options)
        {
            _tenantInfo = tenantInfo;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_tenantInfo != null &&!string.IsNullOrEmpty(_tenantInfo.ConnectionString))
            {
                optionsBuilder.UseSqlServer(_tenantInfo.ConnectionString ?? throw new InvalidOperationException());
            }
            base.OnConfiguring(optionsBuilder);
        }

        public virtual DbSet<IdentityUser> AspNetUsers { get; set; }
        public virtual DbSet<IdentityUserLogin<string>> AspNetUserLogins { get; set; }
    }
}
