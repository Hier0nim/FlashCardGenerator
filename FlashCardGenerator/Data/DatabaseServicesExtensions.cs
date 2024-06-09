namespace FlashCardGenerator.Data
{
  using Microsoft.EntityFrameworkCore;

  public static class DatabaseServicesExtensions
  {
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
      var connectionString = configuration.GetConnectionString("DefaultConnection")
                             ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

      services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
      services.AddDatabaseDeveloperPageExceptionFilter();

      services.AddTransient<DatabaseInitializer>();
    }
  }
}
