using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;
#pragma warning disable CS8604 // Possible null reference argument.

namespace Essentials.HttpClient.Common.Helpers;

/// <summary>
/// Хелперы для серилизации
/// </summary>
public static class SerializationHelpers
{
    public static string SerializeInJson<T>(T? obj) => JsonSerializer.Serialize(obj);

    public static T DeserializeXml<T>(string? str)
    {
        using var textReader = new StringReader(str);
        using var reader = XmlReader.Create(textReader, new XmlReaderSettings());

        var xmlSerializer = new XmlSerializer(typeof(T));
        if (xmlSerializer.Deserialize(reader) is not { } obj)
            throw new InvalidDataException();

        return (T) obj;
    }

    public static string SerializeInXml<T>(T? obj)
    {
        using var textWriter = new Utf8StringWriter();
        using var writer = XmlWriter.Create(textWriter);

        var xmlSerializer = new XmlSerializer(typeof(T));
        xmlSerializer.Serialize(writer, obj);

        return textWriter.ToString();
    }
    
    private class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}