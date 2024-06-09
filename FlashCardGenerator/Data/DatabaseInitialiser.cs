namespace FlashCardGenerator.Data
{
  using Microsoft.EntityFrameworkCore;

  public class DatabaseInitializer(
    ApplicationDbContext context,
    ILogger<DatabaseInitializer> logger)
  {
    /// <summary>
    /// Updates the database.
    /// </summary>
    public void Migrate()
    {
      logger.LogInformation("Checking for database migrations...");
      var pendingMigrationsCount = context.Database.GetPendingMigrations().Count();
      if (pendingMigrationsCount > 0)
      {
        logger.LogInformation("Found {count} pending migration(s).", pendingMigrationsCount);
        logger.LogInformation("Applying pending migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrated successfully.");
      }
      else
      {
        logger.LogInformation("No pending migrations found.");
      }
    }
  }
}
