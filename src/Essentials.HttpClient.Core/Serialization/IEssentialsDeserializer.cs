namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Десериалайзер
/// </summary>
public interface IEssentialsDeserializer
{
    /// <summary>
    /// Десерилизует строку в объект
    /// </summary>
    /// <param name="string">Строка</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <returns>Объект</returns>
    T Deserialize<T>(string @string);
}