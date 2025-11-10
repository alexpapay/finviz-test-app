using Finviz.TestApp.ImageNet.Domain.Entries;
using Microsoft.EntityFrameworkCore;

namespace Finviz.TestApp.ImageNet.Persistence.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ImageNetEntry> ImageNetEntries => Set<ImageNetEntry>();
}
