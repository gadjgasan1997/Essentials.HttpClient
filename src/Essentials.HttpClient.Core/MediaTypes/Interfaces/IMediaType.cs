namespace Essentials.HttpClient.MediaTypes.Interfaces;

/// <summary>
/// Тип содержимого запроса (Json, Xml, ...)
/// </summary>
public interface IMediaType
{
    /// <summary>
    /// Название типа содержимого запроса
    /// </summary>
    string TypeName { get; }
}