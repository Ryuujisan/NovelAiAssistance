using System;
using System.IO;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class NovelAiDbContextFactory : IDesignTimeDbContextFactory<NovelAiDbContext>
{
    public NovelAiDbContext CreateDbContext(string[] args)
    {

            // użyj ENV lub fallback na stały connection string
            var conn = Environment.GetEnvironmentVariable("DB_CONN")
                       ?? "Host=localhost;Database=ainovel_db;Username=izumi;Password=1234";

            var optionsBuilder = new DbContextOptionsBuilder<NovelAiDbContext>();
            optionsBuilder.UseNpgsql(conn);

            return new NovelAiDbContext(optionsBuilder.Options);
        
    }
}
