namespace Finviz.TestApp.ImageNet.Domain.Entries;

public class ImageNetService
{
    // The algorithm iterates through the list twice and uses a dictionary for constant-time parent lookup.
    // Each node is processed exactly once, making the overall complexity linear in the number of entries.
    // Time complexity: O(n)
    // Space complexity: O(n)
    public List<ImageNetEntry> BuildTree(IEnumerable<ImageNetEntry> entries)
    {
        var lookup = entries.ToDictionary(x => x.Id);
        var roots = new List<ImageNetEntry>();

        foreach (var entry in lookup.Values)
        {
            if (entry.ParentId is null)
            {
                roots.Add(entry);
            }
            else if (lookup.TryGetValue(entry.ParentId.Value, out var parent))
            {
                parent.Children.Add(entry);
            }
        }

        return roots;
    }
}