using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebApplication2.Data;

public class SharedDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var tenantInfo = new AppTenantInfo { ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=DatabaseTenant1;Trusted_Connection=True;MultipleActiveResultSets=true" };
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        return new ApplicationDbContext(tenantInfo, optionsBuilder.Options);
    }
}
