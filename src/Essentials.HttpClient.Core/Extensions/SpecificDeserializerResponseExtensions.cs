using LanguageExt;
using LanguageExt.Common;
using System.Diagnostics.CodeAnalysis;
using Essentials.Serialization.Deserializers;
using static Essentials.HttpClient.Dictionaries.KnownHttpClientDeserializers;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для обработки Http ответов с определенным типом десериалайзера
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public static class SpecificDeserializerResponseExtensions
{
    #region Newtonsoft Json
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveJsonContentAsync<TData>(
        this Validation<Error, IResponse> validation)
    {
        return await validation
            .ReceiveContentAsync<TData, NewtonsoftJsonDeserializer>(NEWTONSOFT_JSON)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveJsonContentAsync<TData>(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveJsonContentAsync<TData>().ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveJsonContentUnsafeAsync<TData>(
        this Validation<Error, IResponse> validation)
    {
        return await validation
            .ReceiveContentUnsafeAsync<TData, NewtonsoftJsonDeserializer>(NEWTONSOFT_JSON)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveJsonContentUnsafeAsync<TData>(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveJsonContentUnsafeAsync<TData>().ConfigureAwait(false);
    }

    #endregion
    
    #region Native Json
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveNativeJsonContentAsync<TData>(
        this Validation<Error, IResponse> validation)
    {
        return await validation
            .ReceiveContentAsync<TData, NativeJsonDeserializer>(NATIVE_JSON)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveNativeJsonContentAsync<TData>(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveNativeJsonContentAsync<TData>().ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveNativeJsonContentUnsafeAsync<TData>(
        this Validation<Error, IResponse> validation)
    {
        return await validation
            .ReceiveContentUnsafeAsync<TData, NativeJsonDeserializer>(NATIVE_JSON)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveNativeJsonContentUnsafeAsync<TData>(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveNativeJsonContentUnsafeAsync<TData>().ConfigureAwait(false);
    }

    #endregion
    
    #region Xml
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Xml
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveXmlContentAsync<TData>(
        this Validation<Error, IResponse> validation)
    {
        return await validation.ReceiveContentAsync<TData, XmlDeserializer>(XML).ConfigureAwait(false);
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Xml
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveXmlContentAsync<TData>(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveXmlContentAsync<TData>().ConfigureAwait(false);
    }
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Xml
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveXmlContentUnsafeAsync<TData>(
        this Validation<Error, IResponse> validation)
    {
        return await validation.ReceiveContentUnsafeAsync<TData, XmlDeserializer>(XML).ConfigureAwait(false);
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Xml
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveXmlContentUnsafeAsync<TData>(
        this Task<Validation<Error, IResponse>> task)
    {
        var validation = await task.ConfigureAwait(false);
        return await validation.ReceiveXmlContentUnsafeAsync<TData>().ConfigureAwait(false);
    }

    #endregion
}