using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.ContentTypes.Application;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для обработки Http ответов с определенным типом данных в теле
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedType.Global")]
public static class SpecificContentTypeResponseExtensions
{
    #region Json
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Json
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveJsonContentAsync<TData>(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.ReceiveContentAsync<TData, Json>();
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Json
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveJsonContentAsync<TData>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveJsonContentAsync<TData>();
    }
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Json
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveJsonContentUnsafeAsync<TData>(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.ReceiveContentUnsafeAsync<TData, Json>();
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Json
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveJsonContentUnsafeAsync<TData>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveJsonContentUnsafeAsync<TData>();
    }

    #endregion
    
    #region Xml
    
    /// <summary>
    /// Возвращает содержимое ответа в формате Xml
    /// </summary>
    /// <param name="validation">Объект Validation с Http ответом</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <returns></returns>
    public static async Task<Validation<Error, TData>> ReceiveXmlContentAsync<TData>(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.ReceiveContentAsync<TData, Xml>();
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Xml
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
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
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveXmlContentUnsafeAsync<TData>(
        this Validation<Error, IEssentialsHttpResponse> validation)
    {
        return await validation.ReceiveContentUnsafeAsync<TData, Xml>();
    }

    /// <summary>
    /// Возвращает содержимое ответа в формате Xml
    /// </summary>
    /// <param name="task">Задача на получение объекта Validation с Http ответом</param>
    /// <typeparam name="TData">Тип содержимого</typeparam>
    /// <returns></returns>
    public static async Task<TData?> ReceiveXmlContentUnsafeAsync<TData>(
        this Task<Validation<Error, IEssentialsHttpResponse>> task)
    {
        var validation = await task;
        return await validation.ReceiveXmlContentUnsafeAsync<TData>();
    }

    #endregion
}