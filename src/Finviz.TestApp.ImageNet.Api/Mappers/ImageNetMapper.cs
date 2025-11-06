using Finviz.TestApp.ImageNet.Api.Responses;
using Finviz.TestApp.ImageNet.Domain.Entries;
using Finviz.TestApp.ImageNet.Infrastructure.Parsers;

namespace Finviz.TestApp.ImageNet.Api.Mappers;

public static class ImageNetMapper
{
    public static ImageNetEntry MapNewEntity(this ImageNetDto dto)
    {
        return new ImageNetEntry
        {
            FullPath = dto.FullPath,
            Name = dto.Name,
            Size = dto.Size
        };
    }
    
    public static ImageNetResponse MapToResponse(this ImageNetEntry entity)
    {
        return new ImageNetResponse
        {
            Name = entity.Name,
            Size = entity.Size,
        };
    }
    
    public static ImageNetTreeItemResponse MapToTreeItemResponse(this ImageNetEntry entity)
    {
        return new ImageNetTreeItemResponse
        {
            Name = entity.Name,
            Size = entity.Size,
            Children = entity.Children.Select(MapToTreeItemResponse).ToList()
        };
    }
}