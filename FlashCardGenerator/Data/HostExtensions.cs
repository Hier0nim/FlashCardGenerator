namespace FlashCardGenerator.Data
{
    public static class HostExtensions
    {
        public static void InitializeDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var initializer = services.GetService<DatabaseInitializer>();
            initializer?.Migrate();
        }
    }
}
