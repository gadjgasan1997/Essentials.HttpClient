using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Web;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Essentials.HttpClient;

/// <summary>
/// Билдер создания адреса запроса
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class EssentialsUriBuilder
{
    private const string SEPARATOR = "/";

    /// <summary>
    /// Список сегментов запроса
    /// </summary>
    public List<string> Segments { get; private set; } = new();
    
    /// <summary>
    /// Билдер
    /// </summary>
    public UriBuilder? UriBuilder { get; private set; }
    
    /// <summary>
    /// Параметры запроса
    /// </summary>
    public NameValueCollection? Query { get; private set; }
    
    private EssentialsUriBuilder()
    { }
    
    /// <summary>
    /// Создает экземпляр билдера
    /// </summary>
    /// <param name="address">Базовый адрес запроса</param>
    public static EssentialsUriBuilder CreateBuilder(string address)
    {
        if (Uri.TryCreate(address, UriKind.Absolute, out var uri))
            return new EssentialsUriBuilder().InitPrivateFields(uri);

        // TODO Log
        return new EssentialsUriBuilder();
    }
    
    /// <summary>
    /// Создает экземпляр билдера
    /// </summary>
    /// <param name="uri">Базовый адрес запроса</param>
    public static EssentialsUriBuilder CreateBuilder(Uri uri)
    {
        if (Uri.IsWellFormedUriString(uri.ToString(), UriKind.RelativeOrAbsolute))
            return new EssentialsUriBuilder().InitPrivateFields(uri);

        // TODO Log
        return new EssentialsUriBuilder();
    }
    
    /// <summary>
    /// Инициализирует поля билдера
    /// </summary>
    /// <param name="uri">Базовый адрес запроса</param>
    private EssentialsUriBuilder InitPrivateFields(Uri uri)
    {
        try
        {
            UriBuilder = new UriBuilder(uri);
            Query = HttpUtility.ParseQueryString(UriBuilder.Query);
            Segments = UriBuilder.Path.Split(SEPARATOR).Where(@string => !string.IsNullOrWhiteSpace(@string)).ToList();
        }
        catch (Exception ex)
        {
            // TODO Log
        }

        return this;
    }
    
    /// <summary>
    /// Добавляет сегмент к адресу
    /// </summary>
    /// <param name="segment">Сегмент</param>
    public EssentialsUriBuilder WithSegment(string segment) => ModifyRequest(() => AddSegment(segment));

    /// <summary>
    /// Добавляет сегменты к адресу
    /// </summary>
    /// <param name="segments">Сегменты</param>
    public EssentialsUriBuilder WithSegments(IEnumerable<string> segments)
    {
        return ModifyRequest(() =>
        {
            foreach (var segment in segments)
                AddSegment(segment);
        });
    }

    /// <summary>
    /// Добавляет сегмент к адресу
    /// </summary>
    /// <param name="segment">Сегмент</param>
    private void AddSegment(string segment)
    {
        if (string.IsNullOrWhiteSpace(segment))
            return;

        Segments.AddRange(segment.Split(SEPARATOR));
    }
    
    /// <summary>
    /// Добавляет параметр к адресу
    /// </summary>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение</param>
    /// <returns>Билдер</returns>
    public EssentialsUriBuilder WithUriParam(string name, string? value) => ModifyRequest(() => AddUriParam(name, value));

    /// <summary>
    /// Добавляет параметр к адресу, если он не пустой
    /// </summary>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение</param>
    /// <returns>Билдер</returns>
    public EssentialsUriBuilder WithNotEmptyUriParam(string name, string value)
    {
        return ModifyRequest(() =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            AddUriParam(name, value);
        });
    }

    /// <summary>
    /// Добавляет параметры к адресу
    /// </summary>
    /// <param name="parameters">Список параметров</param>
    /// <returns>Билдер</returns>
    public EssentialsUriBuilder WithUriParams(IEnumerable<(string, string?)> parameters)
    {
        return ModifyRequest(() =>
        {
            foreach (var (name, value) in parameters)
                AddUriParam(name, value);
        });
    }

    /// <summary>
    /// Добавляет непустые параметры к адресу
    /// </summary>
    /// <param name="parameters">Список параметров</param>
    /// <returns>Билдер</returns>
    public EssentialsUriBuilder WithNotEmptyUriParams(IEnumerable<(string, string)> parameters)
    {
        return ModifyRequest(() =>
        {
            foreach (var (name, value) in parameters)
            {
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                AddUriParam(name, value);
            }
        });
    }

    /// <summary>
    /// Добавляет параметр к адресу
    /// </summary>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение</param>
    private void AddUriParam(string name, string? value)
    {
        Contract.Assert(Query is not null, "Query must not null here!");

        if (string.IsNullOrWhiteSpace(name))
            return;

        Query[name] = value;
    }
    
    /// <summary>
    /// Создает адрес
    /// </summary>
    /// <returns></returns>
    public Validation<Error, Uri> Build()
    {
        if (UriBuilder is null || Query is null)
        {
            const string message =
                "Во время содания Http запроса произошла ошибка. Не инициализированы обязательные параметры.";

            // TODO Log
            return Fail<Error, Uri>(message);
        }

        UriBuilder.Path = string.Join(SEPARATOR, Segments);
        UriBuilder.Query = Query.ToString();

        if (!Uri.TryCreate(UriBuilder.ToString(), UriKind.RelativeOrAbsolute, out var uri))
        {
            var message = $"Во время построения адреса запроса произошла ошибка. Некорректный адрес: '{UriBuilder}'.";
            
            // TODO Log
            return Fail<Error, Uri>(message);
        }

        // TODO Log Trace
        return uri;
    }

    /// <summary>
    /// Создает адрес
    /// </summary>
    /// <returns></returns>
    public Task<Validation<Error, Uri>> BuildAsync() => Build().AsTask();

    /// <summary>
    /// Меняет адрес запроса
    /// </summary>
    /// <param name="modifyAction">Действие изменения адреса</param>
    /// <returns></returns>
    private EssentialsUriBuilder ModifyRequest(Action modifyAction)
    {
        if (UriBuilder is null || Query is null)
            return this;

        try
        {
            modifyAction();
        }
        catch (Exception ex)
        {
            // TODO Log
        }

        return this;
    }
}