using LanguageExt;
using LanguageExt.Common;
using Essentials.Serialization;

namespace Essentials.HttpClient;

/// <summary>
/// Http ответ
/// </summary>
public interface IResponse
{
    /// <summary>
    /// Запрос
    /// </summary>
    public IRequest Request { get; }
    
    /// <summary>
    /// Сообщение ответа
    /// </summary>
    HttpResponseMessage ResponseMessage { get; }
    
    /// <summary>
    /// Возвращает строку с содержимым
    /// </summary>
    /// <returns></returns>
    Task<Validation<Error, string>> ReceiveStringAsync();
    
    /// <summary>
    /// Возвращает поток с содержимым
    /// </summary>
    /// <returns></returns>
    Task<Validation<Error, Stream>> ReceiveStreamAsync();
    
    /// <summary>
    /// Возвращает массив байтов
    /// </summary>
    /// <returns></returns>
    Task<Validation<Error, byte[]>> ReceiveBytesAsync();

    /// <summary>
    /// Возвращает объект
    /// </summary>
    /// <param name="deserializerKey">Ключ десериалайзера</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <typeparam name="TDeserializer">Тип десериалайзера</typeparam>
    /// <returns></returns>
    Task<Validation<Error, TData>> ReceiveContentAsync<TData, TDeserializer>(string? deserializerKey = null)
        where TDeserializer : IEssentialsDeserializer;
}