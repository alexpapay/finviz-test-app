namespace Finviz.TestApp.ImageNet.Domain.Entries;

public interface IImageNetRepository
{
    Task<List<ImageNetEntry>> GetAllAsync();
    Task<List<ImageNetEntry>> GetRootAsync();
    Task<List<ImageNetEntry>> GetPathAsync(int id);
    Task<List<ImageNetEntry>> GetChildrenAsync(int parentId);
    Task<(List<ImageNetEntry> Items, int Total)> SearchAsync(string query, int skip = 0, int take = 100);
    Task BulkInsertAsync(IEnumerable<ImageNetEntry> entries);
}
