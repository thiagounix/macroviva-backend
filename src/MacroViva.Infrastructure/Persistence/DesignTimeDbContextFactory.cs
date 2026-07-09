using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MacroViva.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MacroVivaDbContext>
{
    public MacroVivaDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Server=localhost,1433;Database=MacroViva;User Id=sa;Password=MacroViva_dev_12345!;Encrypt=True;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<MacroVivaDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new MacroVivaDbContext(options);
    }
}
