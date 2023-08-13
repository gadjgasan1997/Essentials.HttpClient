using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using Essentials.Func.Utils.Helpers;

namespace Essentials.HttpClient.Serialization.Implementations;

/// <summary>
/// Сериалайзер Xml с использованием System.Xml.Serialization
/// </summary>
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class XmlSerializer : IEssentialsBothSerializer
{
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="serializeOptions">Опции серилизации</param>
    /// <param name="deserializeOptions">Опции десерилизации</param>
    /// <param name="textWriterGetter">Делегат получения райтера для серилизации события</param>
    public XmlSerializer(
        XmlWriterSettings? serializeOptions = null,
        XmlReaderSettings? deserializeOptions = null,
        Func<TextWriter>? textWriterGetter = null)
    {
        SerializeOptions = serializeOptions ?? new XmlWriterSettings();
        DeserializeOptions = deserializeOptions ?? new XmlReaderSettings();
        TextWriterGetter = textWriterGetter ?? (() => new Utf8StringWriter());
    }

    /// <summary>
    /// Опции серилизации
    /// </summary>
    protected virtual XmlWriterSettings SerializeOptions { get; }

    /// <summary>
    /// Опции десерилизации
    /// </summary>
    protected virtual XmlReaderSettings DeserializeOptions { get; }

    /// <summary>
    /// Делегат получения райтера для серилизации события
    /// </summary>
    protected virtual Func<TextWriter> TextWriterGetter { get; }

    /// <inheritdoc cref="IEssentialsSerializer.Serialize{T}" />
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

    /// <inheritdoc cref="IEssentialsDeserializer.Deserialize{T}" />
    public T Deserialize<T>(ReadOnlySpan<byte> data)
    {
        var stream = new MemoryStream(data.ToArray());
        using var reader = XmlReader.Create(stream, DeserializeOptions);
        
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        if (xmlSerializer.Deserialize(reader) is not { } obj)
        {
            // TODO Check message
            throw new InvalidDataException("Объект после десерилизации равен null.");
        }

        return (T) obj;
    }

    private class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}