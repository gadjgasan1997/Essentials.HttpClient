namespace Essentials.HttpClient.Serialization;

/// <summary>
/// Десериалайзер
/// </summary>
public interface IEssentialsDeserializer
{
    /// <summary>
    /// Десерилизует полученные данные в объект
    /// </summary>
    /// <param name="data">Данные</param>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <returns>Объект</returns>
    T Deserialize<T>(ReadOnlySpan<byte> data);
}