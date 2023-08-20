namespace Essentials.HttpClient.Serialization.Helpers;

/// <summary>
/// Хелперы для сериализации
/// </summary>
internal static class SerializationHelpers
{
    /// <summary>
    /// Записывает строку в поток
    /// </summary>
    /// <param name="string">Строка</param>
    /// <returns>Поток</returns>
    public static Stream WriteToStream(string @string)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(@string);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}