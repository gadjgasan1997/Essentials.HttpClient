using System.Diagnostics.CodeAnalysis;
using Essentials.Func.Utils.Helpers;
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
        JsonSerializerSettings? deserializeOptions = null,
        JsonSerializerSettings? serializeOptions = null)
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

    /// <inheritdoc cref="IEssentialsBothSerializer.Serialize{T}" />
    public virtual string Serialize<T>(T? obj)
    {
        var result = JsonConvert.SerializeObject(obj, SerializeOptions);
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
        if (JsonConvert.DeserializeObject(@string, DeserializeOptions) is not T obj)
        {
            // TODO Check message
            throw new InvalidCastException(
                "Не удалось десерилизовать строку в объект. " +
                $"Исходная строка: '{@string}'");
        }

        return obj;
    }
}