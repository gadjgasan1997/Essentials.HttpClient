namespace Essentials.HttpClient.ContentTypes.Interfaces;

/// <summary>
/// Тип содержимого запроса
/// </summary>
public interface IContentType
{
    /// <summary>
    /// Название типа содержимого запроса
    /// </summary>
    string ContentTypeName { get; }
}