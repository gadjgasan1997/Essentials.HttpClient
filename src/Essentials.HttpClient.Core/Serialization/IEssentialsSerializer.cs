namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Сериалайзер
/// </summary>
public interface IEssentialsSerializer
{
    /// <summary>
    /// Серилизует объект в поток
    /// </summary>
    /// <param name="obj">Объект</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <returns>Поток</returns>
    Stream Serialize<T>(T obj);
}