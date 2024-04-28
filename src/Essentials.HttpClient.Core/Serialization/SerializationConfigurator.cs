using Essentials.Utils.Extensions;
using Essentials.Serialization.Serializers;
using Essentials.Serialization.Deserializers;

namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Конфигуратор сериализации/десериализации
/// </summary>
public class SerializationConfigurator
{
    internal SerializationConfigurator()
    { }

    /// <summary>
    /// Устанавливает сериалайзер в XML по-умолчанию
    /// </summary>
    /// <param name="serializer">Сериалайзер</param>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Конфигуратор сериализации/десериализации</returns>
    public SerializationConfigurator SetupDefaultXmlSerializer<TSerializer>(TSerializer serializer)
        where TSerializer : XmlSerializer
    {
        serializer.CheckNotNull("Сериалайзер для XML не может быть null");
        
        SerializersManager.XmlSerializer = serializer;
        return this;
    }

    /// <summary>
    /// Устанавливает сериалайзер в Json с использованием System.Text.Json по-умолчанию
    /// </summary>
    /// <param name="serializer">Сериалайзер</param>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Конфигуратор сериализации/десериализации</returns>
    public SerializationConfigurator SetupDefaultNativeJsonSerializer<TSerializer>(TSerializer serializer)
        where TSerializer : NativeJsonSerializer
    {
        serializer.CheckNotNull("Сериалайзер в Json не может быть null");

        SerializersManager.NativeJsonSerializer = serializer;
        return this;
    }

    /// <summary>
    /// Устанавливает сериалайзер в Json с использованием System.Text.Json по-умолчанию
    /// </summary>
    /// <param name="serializer">Сериалайзер</param>
    /// <typeparam name="TSerializer">Тип сериалайзера</typeparam>
    /// <returns>Конфигуратор сериализации/десериализации</returns>
    public SerializationConfigurator SetupDefaultNewtonsoftJsonSerializer<TSerializer>(TSerializer serializer)
        where TSerializer : NewtonsoftJsonSerializer
    {
        serializer.CheckNotNull("Сериалайзер в Json не может быть null");

        SerializersManager.NewtonsoftJsonSerializer = serializer;
        return this;
    }

    /// <summary>
    /// Устанавливает десериалайзер в XML по-умолчанию
    /// </summary>
    /// <param name="deserializer">Десериалайзер</param>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns>Конфигуратор сериализации/десериализации</returns>
    public SerializationConfigurator SetupDefaultXmlDeserializer<TDeserializer>(TDeserializer deserializer)
        where TDeserializer : XmlDeserializer
    {
        deserializer.CheckNotNull("Десериалайзер из Xml не может быть null");

        SerializersManager.XmlDeserializer = deserializer;
        return this;
    }

    /// <summary>
    /// Устанавливает десериалайзер в Json с использованием System.Text.Json по-умолчанию
    /// </summary>
    /// <param name="deserializer">Десериалайзер</param>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns>Конфигуратор сериализации/десериализации</returns>
    public SerializationConfigurator SetupDefaultNativeJsonDeserializer<TDeserializer>(TDeserializer deserializer)
        where TDeserializer : NativeJsonDeserializer
    {
        deserializer.CheckNotNull("Десериалайзер из Json не может быть null");

        SerializersManager.NativeJsonDeserializer = deserializer;
        return this;
    }

    /// <summary>
    /// Устанавливает десериалайзер в Json с использованием System.Text.Json по-умолчанию
    /// </summary>
    /// <param name="deserializer">Десериалайзер</param>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns>Конфигуратор сериализации/десериализации</returns>
    public SerializationConfigurator SetupDefaultNewtonsoftJsonDeserializer<TDeserializer>(TDeserializer deserializer)
        where TDeserializer : NewtonsoftJsonDeserializer
    {
        deserializer.CheckNotNull("Десериалайзер из Json не может быть null");

        SerializersManager.NewtonsoftJsonDeserializer = deserializer;
        return this;
    }
}