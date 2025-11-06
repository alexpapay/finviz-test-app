using System.Text;
using System.Xml;
using Finviz.TestApp.Domain.Shared.Extensions;

namespace Finviz.TestApp.ImageNet.Domain.Entries;

public class ImageNetXmlParser(HttpClient httpClient)
{
    public async Task<List<ImageNetEntry>> ParseAsync(string xmlUrl)
    {
        const int initialCapacity = 61000;
        var results = new List<ImageNetEntry>(initialCapacity);

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            Async = true,
            IgnoreComments = true,
            IgnoreWhitespace = true
        };

        var pathBuilder = new StringBuilder(1024);
        var pathLengthStack = new Stack<int>(50);
        const string separator = " > ";

        await using var stream = await httpClient.GetStreamAsync(xmlUrl);
        using var reader = XmlReader.Create(stream, settings);

        while (await reader.ReadAsync())
        {
            if (reader is { NodeType: XmlNodeType.Element, Name: "synset"})
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

                results.Add(new ImageNetEntry
                {
                    Name = pathBuilder.ToString(),
                });

                if (reader.IsEmptyElement)
                    pathBuilder.Length = pathLengthStack.Pop();
            }
            else if (reader is { NodeType: XmlNodeType.EndElement, Name: "synset"})
            {
                if (pathLengthStack.Count > 0)
                    pathBuilder.Length = pathLengthStack.Pop();
            }
        }

        return results;
    }
    
    public List<ImageNetEntry> ComputeSizes(List<ImageNetEntry> entries)
    {
        var imageNetEntries = entries
            .DistinctBy(e => e.Name)
            .ToDictionary(e => e.Name, e => e);
        
        var orderedEntries = entries
            .OrderBy(e => e.Name.Count(c => c == '>'))
            .ToList();

        foreach (var entry in orderedEntries)
        {
            var path = entry.Name;
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