namespace Finviz.TestApp.ImageNet.Domain.Entries;

public interface IImageNetRepository
{
    Task<List<ImageNetEntry>> GetAllAsync();
    Task<List<ImageNetEntry>> GetRootAsync();
    Task<List<ImageNetEntry>> GetChildrenAsync(int parentId);
    Task<List<ImageNetEntry>> SearchAsync(string query);
    Task BulkInsertAsync(IEnumerable<ImageNetEntry> entries);
}