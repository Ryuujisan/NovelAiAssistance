using Microsoft.EntityFrameworkCore;

namespace API.Extension;

public static class ServicesExtension
{
    /// <summary>
    /// Adding DbContext
    /// </summary>
    /// <param name="services"> </param>
    /// <param name="configuration"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddDb<T>(this IServiceCollection services, IConfiguration configuration) where T : DbContext
    {
        var connectionString = configuration.GetConnectionString("Postgres");
        services.AddDbContext<T>(o => o.UseNpgsql(connectionString)
        .UseLazyLoadingProxies());

    }
}