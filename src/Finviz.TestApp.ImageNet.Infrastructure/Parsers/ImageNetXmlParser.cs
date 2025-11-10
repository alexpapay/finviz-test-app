using System.Text;
using System.Xml;
using Finviz.TestApp.Domain.Shared.Extensions;

namespace Finviz.TestApp.ImageNet.Infrastructure.Parsers;

public class ImageNetXmlParser(HttpClient httpClient)
{
    public async Task<List<ImageNetDto>> ParseAsync(string xmlUrl)
    {
        const int initialCapacity = 61000;
        var results = new List<ImageNetDto>(initialCapacity);

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            Async = true,
            IgnoreComments = true,
            IgnoreWhitespace = true,
        };

        var pathBuilder = new StringBuilder(capacity: 1024);
        var pathLengthStack = new Stack<int>(capacity: 50);
        const string separator = " > ";

        await using var stream = await httpClient.GetStreamAsync(xmlUrl);
        using var reader = XmlReader.Create(stream, settings);

        while (await reader.ReadAsync())
        {
            if (reader is { NodeType: XmlNodeType.Element, Name: "synset" })
            {
                var name = reader.GetAttribute("words");

                if (name.IsNullOrEmptyString())
                {
                    continue;
                }

                pathLengthStack.Push(pathBuilder.Length);

                if (pathBuilder.Length > 0)
                {
                    pathBuilder.Append(separator);
                }

                pathBuilder.Append(name);

                results.Add(new ImageNetDto
                {
                    FullPath = pathBuilder.ToString(),
                    Name = name,
                });

                if (reader.IsEmptyElement)
                {
                    pathBuilder.Length = pathLengthStack.Pop();
                }
            }
            else if (reader is { NodeType: XmlNodeType.EndElement, Name: "synset" })
            {
                if (pathLengthStack.Count > 0)
                {
                    pathBuilder.Length = pathLengthStack.Pop();
                }
            }
        }

        return results;
    }

    public List<ImageNetDto> ComputeSizes(List<ImageNetDto> entries)
    {
        var imageNetEntries = entries
            .DistinctBy(e => e.FullPath)
            .ToDictionary(e => e.FullPath, e => e);

        var orderedEntries = entries
            .OrderBy(e => e.FullPath.Count(c => c == '>'))
            .ToList();

        foreach (var entry in orderedEntries)
        {
            var path = entry.FullPath;
            var index = path.LastIndexOf(" > ", StringComparison.Ordinal);

            while (index > 0)
            {
                var parent = path[..index];
                if (imageNetEntries.TryGetValue(parent, out var parentNode))
                    parentNode.Size++;

                index = parent.LastIndexOf(" > ", StringComparison.Ordinal);
            }
        }

        return entries;
    }
}
