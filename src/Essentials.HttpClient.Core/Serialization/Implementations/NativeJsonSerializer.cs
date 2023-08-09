using System.Text.Json;
using Essentials.Func.Utils.Helpers;

namespace Essentials.HttpClient.Serialization.Implementations;

/// <summary>
/// Сериалайзер Json с использованием System.Text.Json
/// </summary>
public class NativeJsonSerializer : IEssentialsBothSerializer
{
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="deserializeOptions">Опции десерилизации</param>
    /// <param name="serializeOptions">Опции серилизации</param>
    public NativeJsonSerializer(
        JsonSerializerOptions? deserializeOptions = null,
        JsonSerializerOptions? serializeOptions = null)
    {
        SerializeOptions = serializeOptions ?? new JsonSerializerOptions();
        DeserializeOptions = deserializeOptions ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Опции серилизации
    /// </summary>
    protected virtual JsonSerializerOptions SerializeOptions { get; }
    
    /// <summary>
    /// Опции десерилизации
    /// </summary>
    protected virtual JsonSerializerOptions DeserializeOptions { get; }
    
    /// <inheritdoc cref="IEssentialsSerializer.Serialize{T}" />
    public string Serialize<T>(T obj)
    {
        var result = JsonSerializer.Serialize(obj, SerializeOptions);
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
    public T Deserialize<T>(string @string)
    {
        if (JsonSerializer.Deserialize(@string, typeof(T), DeserializeOptions) is not { } obj)
        {
            // TODO Check message
            throw new InvalidCastException(
                "Объект после десерилизации равен null. " +
                $"Исходная строка: '{@string}'");
        }

        return (T) obj;
    }
}