using System.Text;
using System.Xml;
using Essentials.Func.Utils.Helpers;

namespace Essentials.HttpClient.Serialization.Implementations;

/// <summary>
/// Сериалайзер Xml с использованием System.Xml.Serialization
/// </summary>
public class XmlSerializer : IEssentialsBothSerializer
{
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="deserializeOptions">Опции десерилизации</param>
    /// <param name="textReaderGetter">Делегат получения ридера для десерилизации сообщения</param>
    /// <param name="serializeOptions">Опции серилизации</param>
    /// <param name="textWriterGetter">Делегат получения райтера для серилизации события</param>
    public XmlSerializer(
        XmlReaderSettings? deserializeOptions = null,
        Func<string, TextReader>? textReaderGetter = null,
        XmlWriterSettings? serializeOptions = null,
        Func<TextWriter>? textWriterGetter = null)
    {
        SerializeOptions = serializeOptions ?? new XmlWriterSettings();
        TextWriterGetter = textWriterGetter ?? (() => new Utf8StringWriter());
        DeserializeOptions = deserializeOptions ?? new XmlReaderSettings();
        TextReaderGetter = textReaderGetter ?? (message => new StringReader(message));
    }

    /// <summary>
    /// Опции серилизации
    /// </summary>
    protected virtual XmlWriterSettings SerializeOptions { get; }

    /// <summary>
    /// Делегат получения райтера для серилизации события
    /// </summary>
    protected virtual Func<TextWriter> TextWriterGetter { get; }

    /// <summary>
    /// Опции десерилизации
    /// </summary>
    protected virtual XmlReaderSettings DeserializeOptions { get; }

    /// <summary>
    /// Делегат получения ридера для десерилизации сообщения
    /// </summary>
    /// <returns></returns>
    protected virtual Func<string, TextReader> TextReaderGetter { get; }

    /// <inheritdoc cref="IEssentialsBothSerializer.Serialize{T}" />
    public virtual string Serialize<T>(T? obj)
    {
        using var textWriter = TextWriterGetter();
        using var writer = XmlWriter.Create(textWriter, SerializeOptions);

        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        xmlSerializer.Serialize(writer, obj);

        var result = textWriter.ToString();
        if (string.IsNullOrWhiteSpace(result))
        {
            // TODO Check message
            throw new ArgumentException(
                "Строка пуста после серлизиации. " +
                $"Исходный объект: '{JsonHelpers.Serialize(obj)}'");
        }

        return result;
    }

    /// <inheritdoc cref="IEssentialsBothSerializer.Deserialize{T}" />
    public virtual T Deserialize<T>(string @string)
    {
        using var textReader = TextReaderGetter(@string);
        using var reader = XmlReader.Create(textReader, DeserializeOptions);

        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        if (xmlSerializer.Deserialize(reader) is not T obj)
        {
            // TODO Check message
            throw new InvalidCastException(
                "Не удалось десерилизовать строку в объект. " +
                $"Исходная строка: '{@string}'");
        }

        return obj;
    }

    private class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}