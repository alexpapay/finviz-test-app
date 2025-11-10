namespace Finviz.TestApp.ImageNet.Domain.Entries;

public class ImageNetEntry
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string FullPath { get; set; } = null!;
    public int Size { get; set; }
    public int? ParentId { get; set; }

    public ImageNetEntry? Parent { get; set; }
    public ICollection<ImageNetEntry> Children { get; set; } = new List<ImageNetEntry>();
}
