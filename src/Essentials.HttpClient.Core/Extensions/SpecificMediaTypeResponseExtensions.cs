using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Serialization.Implementations;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для обработки Http ответов с определенным типом данных в теле
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public static class SpecificMediaTypeResponseExtensions
{
    #region Newtonsoft Json
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveJsonContentAsync<TData>(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.ReceiveContentAsync<TData, NewtonsoftJsonSerializer>();
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveJsonContentAsync<TData>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveJsonContentAsync<TData>();
    }
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveJsonContentUnsafeAsync<TData>(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.ReceiveContentUnsafeAsync<TData, NewtonsoftJsonSerializer>();
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием Newtonsoft.Json
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveJsonContentUnsafeAsync<TData>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveJsonContentUnsafeAsync<TData>();
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
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.ReceiveContentAsync<TData, NativeJsonSerializer>();
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveNativeJsonContentAsync<TData>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveNativeJsonContentAsync<TData>();
    }
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveNativeJsonContentUnsafeAsync<TData>(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.ReceiveContentUnsafeAsync<TData, NativeJsonSerializer>();
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Json с использованием System.Text.Json
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveNativeJsonContentUnsafeAsync<TData>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveNativeJsonContentUnsafeAsync<TData>();
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
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.ReceiveContentAsync<TData, XmlSerializer>();
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Xml
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveXmlContentAsync<TData>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveXmlContentAsync<TData>();
    }
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Xml
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveXmlContentUnsafeAsync<TData>(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.ReceiveContentUnsafeAsync<TData, XmlSerializer>();
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Xml
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип данных, в который потребуется десерилизовать ответ</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveXmlContentUnsafeAsync<TData>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveXmlContentUnsafeAsync<TData>();
    }

    #endregion
}