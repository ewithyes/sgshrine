using Microsoft.EntityFrameworkCore;
public class ComicDatabaseContext : DbContext
{

public ComicDatabaseContext(DbContextOptions<ComicDatabaseContext> options) : base(options){}
        public DbSet<Comic> Comics => Set<Comic>();
        public DbSet<Chapter> Chapters => Set<Chapter>();
        public DbSet<Page> Pages => Set<Page>();
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comic>()
        .HasMany(c => c.Chapters)
        .WithOne(ch => ch.Comic)
        .HasForeignKey(ch => ch.ComicId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Chapter>()
        .HasMany(ch => ch.Pages)
        .WithOne(p => p.Chapter)
        .HasForeignKey(p => p.ChapterId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Chapter>()
            .HasIndex(ch => new { ch.ComicId, ch.Order });

        modelBuilder.Entity<Page>()
            .HasIndex(p => new { p.ChapterId, p.PageNumber });
    }

}