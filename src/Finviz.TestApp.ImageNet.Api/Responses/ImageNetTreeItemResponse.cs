namespace Finviz.TestApp.ImageNet.Api.Responses;

public class ImageNetTreeItemResponse
{
    public string Name { get; set; } = null!;
    public int Size { get; set; }

    public ICollection<ImageNetTreeItemResponse> Children { get; set; } = new List<ImageNetTreeItemResponse>();
}
