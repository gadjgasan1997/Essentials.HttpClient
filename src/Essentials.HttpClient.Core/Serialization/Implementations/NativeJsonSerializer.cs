using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Essentials.Func.Utils.Helpers;

namespace Essentials.HttpClient.Serialization.Implementations;

/// <summary>
/// Сериалайзер Json с использованием System.Text.Json
/// </summary>
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public class NativeJsonSerializer : IEssentialsBothSerializer
{
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="serializeOptions">Опции серилизации</param>
    /// <param name="deserializeOptions">Опции десерилизации</param>
    public NativeJsonSerializer(
        JsonSerializerOptions? serializeOptions = null,
        JsonSerializerOptions? deserializeOptions = null)
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
    public T Deserialize<T>(ReadOnlySpan<byte> data)
    {
        return JsonSerializer.Deserialize<T>(data, DeserializeOptions)
               ?? throw new InvalidDataException("Объект после десерилизации равен null.");
    }
}