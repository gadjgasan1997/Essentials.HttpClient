using System.Diagnostics.CodeAnalysis;
using Essentials.Func.Utils.Helpers;
using Essentials.HttpClient.Serialization.Helpers;
using Newtonsoft.Json;

namespace Essentials.HttpClient.Serialization.Implementations;

/// <summary>
/// Сериалайзер Json с использованием Newtonsoft.Json
/// </summary>
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class NewtonsoftJsonSerializer : IEssentialsBothSerializer
{
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="serializeOptions">Опции серилизации</param>
    /// <param name="deserializeOptions">Опции десерилизации</param>
    public NewtonsoftJsonSerializer(
        JsonSerializerSettings? serializeOptions = null,
        JsonSerializerSettings? deserializeOptions = null)
    {
        SerializeOptions = serializeOptions ?? new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        DeserializeOptions = deserializeOptions ?? new JsonSerializerSettings();
    }

    /// <summary>
    /// Опции серилизации
    /// </summary>
    protected virtual JsonSerializerSettings SerializeOptions { get; }

    /// <summary>
    /// Опции десерилизации
    /// </summary>
    protected virtual JsonSerializerSettings DeserializeOptions { get; }

    /// <inheritdoc cref="IEssentialsSerializer.Serialize{T}" />
    public virtual Stream Serialize<T>(T? obj)
    {
        var resultString = JsonConvert.SerializeObject(obj, SerializeOptions);
        if (string.IsNullOrWhiteSpace(resultString))
        {
            // TODO Check message
            throw new ArgumentException(
                "Строка пуста после сериализации. " +
                $"Исходный объект: '{JsonHelpers.Serialize(obj)}'");
        }

        return SerializationHelpers.WriteToStream(resultString);
    }

    /// <inheritdoc cref="IEssentialsDeserializer.Deserialize{T}" />
    public T Deserialize<T>(ReadOnlySpan<byte> data)
    {
        var stream = new MemoryStream(data.ToArray());
        var reader = new JsonTextReader(new StreamReader(stream));

        var serializer = JsonSerializer.CreateDefault(DeserializeOptions);
        return serializer.Deserialize<T>(reader)
               ?? throw new InvalidDataException("Объект после десерилизации равен null.");
    }
}