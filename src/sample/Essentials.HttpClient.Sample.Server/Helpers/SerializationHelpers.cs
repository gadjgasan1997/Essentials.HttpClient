using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Essentials.HttpClient.Sample.Server.Helpers;

public static class SerializationHelpers
{
    public static string SerializeJson<T>(T? obj) => JsonSerializer.Serialize(obj);

    public static string SerializeXml<T>(T? obj)
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