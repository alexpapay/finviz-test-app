using Finviz.TestApp.ImageNet.Api.Responses;
using Finviz.TestApp.ImageNet.Domain.Entries;
using Finviz.TestApp.ImageNet.Infrastructure.Parsers;

namespace Finviz.TestApp.ImageNet.Api.Mappers;

public static class ImageNetMapper
{
    public static ImageNetEntry MapNewEntity(this ImageNetDto dto)
        => new()
        {
            FullPath = dto.FullPath,
            Name = dto.Name,
            Size = dto.Size,
        };

    public static ImageNetResponse MapToResponse(this ImageNetEntry entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Size = entity.Size,
            HasChildren = entity.Size != 0,
        };

    public static ImageNetTreeItemResponse MapToTreeItemResponse(this ImageNetEntry entity)
        => new()
        {
            Name = entity.Name,
            Size = entity.Size,
            Children = entity.Children.Select(MapToTreeItemResponse).ToList(),
        };
}
