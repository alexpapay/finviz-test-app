using Finviz.TestApp.ImageNet.Domain.Entries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finviz.TestApp.ImageNet.Persistence.Configurations;

public class ImageNetEntryConfig : IEntityTypeConfiguration<ImageNetEntry>
{
    public void Configure(EntityTypeBuilder<ImageNetEntry> builder)
    {
        builder.HasKey(imageNetEntry => imageNetEntry.Id);

        builder.Property(imageNetEntry => imageNetEntry.Id)
            .ValueGeneratedOnAdd();

        builder.Property(imageNetEntry => imageNetEntry.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(imageNetEntry => imageNetEntry.FullPath)
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(imageNetEntry => imageNetEntry.Size)
            .IsRequired();
        
        builder.HasIndex(imageNetEntry => imageNetEntry.FullPath)
            .IsUnique();
        
        builder.HasIndex(imageNetEntry => imageNetEntry.ParentId);

        builder.HasOne(imageNetEntry => imageNetEntry.Parent)
            .WithMany(imageNetEntry => imageNetEntry.Children)
            .HasForeignKey(imageNetEntry => imageNetEntry.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}