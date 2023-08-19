using System.Collections.Specialized;
using LanguageExt;
using LanguageExt.Common;

namespace Essentials.HttpClient;

/// <summary>
/// Билдер создания адреса запроса
/// </summary>
public interface IUriBuilder
{
    /// <summary>
    /// Список сегментов запроса
    /// </summary>
    public List<string> Segments { get; }
    
    /// <summary>
    /// Билдер
    /// </summary>
    public UriBuilder? UriBuilder { get; }
    
    /// <summary>
    /// Параметры запроса
    /// </summary>
    public NameValueCollection? Query { get; }

    /// <summary>
    /// Добавляет сегмент к адресу
    /// </summary>
    /// <param name="segment">Сегмент</param>
    /// <returns>Билдер</returns>
    IUriBuilder WithSegment(string segment);

    /// <summary>
    /// Добавляет сегменты к адресу
    /// </summary>
    /// <param name="segments">Сегменты</param>
    /// <returns>Билдер</returns>
    IUriBuilder WithSegments(params string[] segments);

    /// <summary>
    /// Добавляет параметр к адресу
    /// </summary>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение</param>
    /// <returns>Билдер</returns>
    IUriBuilder WithUriParam(string name, string? value);

    /// <summary>
    /// Добавляет параметр к адресу, если он не пустой
    /// </summary>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение</param>
    /// <returns>Билдер</returns>
    IUriBuilder WithNotEmptyUriParam(string name, string? value);

    /// <summary>
    /// Добавляет параметры к адресу
    /// </summary>
    /// <param name="parameters">Список параметров</param>
    /// <returns>Билдер</returns>
    IUriBuilder WithUriParams(params (string, string?)[] parameters);

    /// <summary>
    /// Добавляет непустые параметры к адресу
    /// </summary>
    /// <param name="parameters">Список параметров</param>
    /// <returns>Билдер</returns>
    IUriBuilder WithNotEmptyUriParams(params (string, string?)[] parameters);

    /// <summary>
    /// Создает адрес
    /// </summary>
    /// <returns>Адрес запроса</returns>
    Validation<Error, Uri> Build();

    /// <summary>
    /// Создает адрес
    /// </summary>
    /// <returns>Адрес запроса</returns>
    Task<Validation<Error, Uri>> BuildAsync();
}