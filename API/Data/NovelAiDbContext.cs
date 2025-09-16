using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class NovelAiDbContext(DbContextOptions<NovelAiDbContext> options) : DbContext
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<World> Worlds => Set<World>();
    public DbSet<Timeline> Timelines => Set<Timeline>();
    public DbSet<User> Users => Set<User>();
    public DbSet<WriteSettings> WriteSettings => Set<WriteSettings>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Character> Characters => Set<Character>();
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // gdy design-time (dotnet ef) nie poda provider'a
        if (!optionsBuilder.IsConfigured)
        {
            var conn = Environment.GetEnvironmentVariable("DB_CONN")
                       ?? "Host=localhost;Database=ainovel_db;Username=izumi;Password=1234";
            optionsBuilder.UseNpgsql(conn);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // Project ↔ World (1:1)
        // FK: World.ProjectId (unikalny)
        // =========================
        modelBuilder.Entity<Project>()
            .HasOne(p => p.World)
            .WithOne(w => w.Project)
            .HasForeignKey<World>(w => w.ProjectId)   // FK po stronie World
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<World>()
            .HasIndex(w => w.ProjectId)
            .IsUnique(); // to faktycznie wymusza 1:1 w DB

        // =========================
        // Project ↔ Timeline (1:1)
        // FK: Project.TimelineId (unikalny)
        // Uwaga: jeśli Timeline nie ma nawigacji do Project, używamy .WithOne()
        // =========================
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Timeline)
            .WithOne() // brak nawigacji w Timeline
            .HasForeignKey<Project>(p => p.TimelineId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.TimelineId)
            .IsUnique();
        // Jeśli chcesz ograniczyć unikalność tylko dla wartości nie-null (opcjonalnie, PostgreSQL):
        // modelBuilder.Entity<Project>()
        //     .HasIndex(p => p.TimelineId)
        //     .IsUnique()
        //     .HasFilter("\"TimelineId\" IS NOT NULL");

        // Alternatywnie (jeśli wolisz, by FK był po stronie Timeline):
        // - dodaj int ProjectId w Timeline
        // - zamień FK na .HasForeignKey<Timeline>(t => t.ProjectId)
        // - i daj .HasIndex(t => t.ProjectId).IsUnique()

        // =========================
        // User ↔ WriteSettings (1:1)
        // FK: User.WriteSettingsId (unikalny)
        // =========================
        modelBuilder.Entity<User>()
            .HasOne(u => u.WriteSettings)
            .WithOne(ws => ws.User)
            .HasForeignKey<User>(u => u.WriteSettingsId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.WriteSettingsId)
            .IsUnique();

        // (Dobra praktyka) e-mail unikalny:
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // =========================
        // Chapter: unikalny porządek w ramach projektu
        // =========================
        modelBuilder.Entity<Chapter>()
            .HasIndex(c => new { c.ProjectId, c.Order })
            .IsUnique();

        modelBuilder.Entity<Project>().Property(p => p.Name).HasMaxLength(200).IsRequired();
        modelBuilder.Entity<User>().Property(u => u.Email).HasMaxLength(320).IsRequired();
        modelBuilder.Entity<Character>().Property(c => c.Name).HasMaxLength(120).IsRequired();
    }

    public async Task SeedDataAsync(CancellationToken ct = default)
    {
        await Database.MigrateAsync(ct);

        if (await Users.AnyAsync(ct) || await Projects.AnyAsync(ct) || await Worlds.AnyAsync(ct))
        {
            return;
        }
       await Seed(ct);
    }

    private async Task Seed(CancellationToken ct = default)
    {
        var seed = await ReadFromJsonSeedDb();
        if (seed is null) return;

        // 0) „wyzeruj” Id, by DB nadała je sama (identity)
        seed.WriteSettings.Id = 0;
        seed.User.Id = 0;
        seed.Project.Id = 0;
        seed.World.Id = 0;
        seed.Timeline.Id = 0;
        foreach (var ch in seed.Characters) ch.Id = 0;

        // 1) WriteSettings → wymagane przez User.WriteSettingsId
        await WriteSettings.AddAsync(seed.WriteSettings, ct);
        await SaveChangesAsync(ct);

        // 2) User → wymaga WriteSettingsId
        seed.User.WriteSettingsId = seed.WriteSettings.Id; // User ma required WriteSettingsId. :contentReference[oaicite:0]{index=0}
        await Users.AddAsync(seed.User, ct);
        await SaveChangesAsync(ct);

        // 3) Timeline (pusta – ale Project ma required TimelineId)
        await Timelines.AddAsync(seed.Timeline, ct);
        await SaveChangesAsync(ct);

        // 4) Project → wymaga UserId i TimelineId (oba required). :contentReference[oaicite:1]{index=1}
        seed.Project.UserId = seed.User.Id;
        seed.Project.TimelineId = seed.Timeline.Id;
        await Projects.AddAsync(seed.Project, ct);
        await SaveChangesAsync(ct);

        // 5) World → wymaga ProjectId (1:1 z Project). :contentReference[oaicite:2]{index=2}
        seed.World.ProjectId = seed.Project.Id;
        await Worlds.AddAsync(seed.World, ct);
        await SaveChangesAsync(ct);

        // 6) Characters → każdy wymaga ProjectId. :contentReference[oaicite:3]{index=3}
        foreach (var ch in seed.Characters)
            ch.ProjectId = seed.Project.Id;

        await Characters.AddRangeAsync(seed.Characters, ct);
        await SaveChangesAsync(ct);
        
    }

    private async Task<SeedDb?> ReadFromJsonSeedDb()
    {
        var json = await File.ReadAllTextAsync("seedDb.json");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: true));

        // wczytuje cały pakiet danych
        var seed = JsonSerializer.Deserialize<SeedDb>(json, options);

        if (seed is not null)
        {
            Console.WriteLine($"Projekt: {seed.Project.Name}, autor: {seed.User.Name}");
            Console.WriteLine($"Postacie: {string.Join(", ", seed.Characters.Select(c => c.Name))}");
        }

        return seed;
    }

    #region HelperClasses
    public class SeedDb
    {
        public User User { get; set; }
        public WriteSettings WriteSettings { get; set; }
        public Project Project { get; set; }
        public World World { get; set; }
        public Timeline Timeline { get; set; }
        public List<Character> Characters { get; set; }

    }
    #endregion HelperClasses
}