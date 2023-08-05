namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Сериалайзер
/// </summary>
public interface IEssentialsSerializer
{
    /// <summary>
    /// Серилизует объект в строку
    /// </summary>
    /// <param name="obj">Объект</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <returns>Строка</returns>
    string Serialize<T>(T obj);
}