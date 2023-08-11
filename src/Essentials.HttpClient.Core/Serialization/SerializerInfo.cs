namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Информация о сериайлазере/десериайлазере
/// </summary>
/// <param name="Serializer">Сериайлайзер</param>
/// <param name="Key">Необязательный ключ, по умолчанию равный полному названию типа сериайлайзера</param>
public record SerializerInfo(IEssentialsBothSerializer Serializer, string? Key = null)
{
    /// <summary>
    /// Возвращает ключ для добавления/поиска сериайлазера
    /// </summary>
    /// <returns></returns>
    internal string GetKey() => Key ?? Serializer.GetType().FullName!;
}