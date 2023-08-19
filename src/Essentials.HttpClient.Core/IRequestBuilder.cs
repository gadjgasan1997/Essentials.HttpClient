using System.Text;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient;

/// <summary>
/// Билдер создания запроса
/// </summary>
public interface IRequestBuilder
{
    /// <summary>
    /// Добавляет заголовок к запросу
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="values">Значения заголовка</param>
    /// <returns>Билдер</returns>
    IRequestBuilder WithHeader(string name, params string?[] values);

    /// <summary>
    /// Добавляет заголовок к запросу, если его значение не пустое
    /// </summary>
    /// <param name="name">Название заголовка</param>
    /// <param name="values">Значения заголовка</param>
    /// <returns>Билдер</returns>
    IRequestBuilder WithNotEmptyHeader(string name, params string?[] values);

    /// <summary>
    /// Добавляет заголовки к запросу
    /// </summary>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер</returns>
    IRequestBuilder WithHeaders(params (string Name, IEnumerable<string?> Value)[] headers);

    /// <summary>
    /// Добавляет заголовки к запросу, если их значения не пустые
    /// </summary>
    /// <param name="headers">Список заголовков</param>
    /// <returns>Билдер</returns>
    IRequestBuilder WithNotEmptyHeaders(params (string Name, IEnumerable<string?> Value)[] headers);

    /// <summary>
    /// Устанавливает тип содержимого запроса
    /// </summary>
    /// <param name="mediaType">Тип содержимого</param>
    /// <param name="encoding">Кодировка</param>
    /// <returns>Билдер</returns>
    IRequestBuilder SetMediaType(string mediaType, Encoding? encoding = null);

    /// <summary>
    /// Настраивает Basic авторизацию
    /// </summary>
    /// <param name="userName">Логин</param>
    /// <param name="password">Пароль</param>
    /// <returns>Билдер</returns>
    IRequestBuilder WithBasicAuthentication(string userName, string password);

    /// <summary>
    /// Настраивает авторизацию на Jwt токенах
    /// </summary>
    /// <param name="token">Токен</param>
    /// <returns>Билдер</returns>
    IRequestBuilder WithJwtAuthentication(string token);

    /// <summary>
    /// Настраивает авторизацию
    /// </summary>
    /// <param name="scheme">Схема авторизации</param>
    /// <param name="parameter">Параметр</param>
    /// <returns>Билдер</returns>
    IRequestBuilder WithAuthentication(string scheme, string parameter);

    /// <summary>
    /// Меняет запрос поданным на вход делегатом
    /// </summary>
    /// <param name="action">Делегат для изменения запроса</param>
    /// <returns>Билдер</returns>
    IRequestBuilder ModifyRequest(Action<HttpRequestMessage?> action);

    /// <summary>
    /// Устанавливает таймаут запроса
    /// </summary>
    /// <param name="timeout">Таймаут</param>
    /// <returns>Билдер</returns>
    IRequestBuilder SetTimeout(TimeSpan timeout);

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns>Запрос</returns>
    Validation<Error, IRequest> Build(string? clientName = null);

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <param name="clientName">Название Http клиента</param>
    /// <returns>Запрос</returns>
    Task<Validation<Error, IRequest>> BuildAsync(string? clientName = null);

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <typeparam name="TClient">Тип клиента</typeparam>
    /// <returns>Запрос</returns>
    Validation<Error, IRequest> Build<TClient>();

    /// <summary>
    /// Создает запрос
    /// </summary>
    /// <typeparam name="TClient">Тип клиента</typeparam>
    /// <returns>Запрос</returns>
    Task<Validation<Error, IRequest>> BuildAsync<TClient>();
}