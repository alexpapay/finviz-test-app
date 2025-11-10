namespace Finviz.TestApp.ImageNet.Api.Responses;

public class ImageNetResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Size { get; set; }
    public bool HasChildren { get; set; }
}
