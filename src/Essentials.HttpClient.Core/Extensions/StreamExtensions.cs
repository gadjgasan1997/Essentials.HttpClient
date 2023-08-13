namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для потоков
/// </summary>
internal static class StreamExtensions
{
    /// <summary>
    /// Преобразует поток в массив байтов
    /// </summary>
    /// <param name="stream">Поток</param>
    /// <returns>Массив байтов</returns>
    public static byte[] ToByteArray(this Stream stream)
    {
        var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}