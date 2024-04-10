using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using LanguageExt.Common;
using Essentials.Utils.Collections.Extensions;
using static LanguageExt.Prelude;
using static Essentials.HttpClient.Dictionaries.CommonConsts;

namespace Essentials.HttpClient.Extensions;

/// <summary>
/// Методы расширения для <see cref="HttpUriBuilder" />
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class UriBuilderExtensions
{
    /// <summary>
    /// Добавляет сегмент к адресу
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="segment">Сегмент</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpUriBuilder> WithSegment(
        this Validation<Error, HttpUriBuilder> validation,
        string segment)
    {
        return validation.ModifyUri(builder => () => builder.AddSegment(segment));
    }
    
    /// <summary>
    /// Добавляет сегменты к адресу
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="segments">Сегменты</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpUriBuilder> WithSegments(
        this Validation<Error, HttpUriBuilder> validation,
        params string[] segments)
    {
        return validation.ModifyUri(builder => () => segments.ForEach(builder.AddSegment));
    }

    /// <summary>
    /// Добавляет параметр к адресу
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpUriBuilder> WithUriParam(
        this Validation<Error, HttpUriBuilder> validation,
        string name,
        string? value)
    {
        return validation.ModifyUri(builder => () => builder.AddUriParam(name, value));
    }

    /// <summary>
    /// Добавляет параметр к адресу, если он не пустой
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="name">Название параметра</param>
    /// <param name="value">Значение</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpUriBuilder> WithNotEmptyUriParam(
        this Validation<Error, HttpUriBuilder> validation,
        string name,
        string? value)
    {
        return validation.ModifyUri(builder => () =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            builder.AddUriParam(name, value);
        });
    }

    /// <summary>
    /// Добавляет параметры к адресу
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="parameters">Список параметров</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpUriBuilder> WithUriParams(
        this Validation<Error, HttpUriBuilder> validation,
        params (string Name, string? Value)[] parameters)
    {
        return validation.ModifyUri(builder =>
            () => parameters.ForEach(tuple => builder.AddUriParam(tuple.Name, tuple.Value)));
    }

    /// <summary>
    /// Добавляет непустые параметры к адресу
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="parameters">Список параметров</param>
    /// <returns>Билдер</returns>
    public static Validation<Error, HttpUriBuilder> WithNotEmptyUriParams(
        this Validation<Error, HttpUriBuilder> validation,
        params (string Name, string? Value)[] parameters)
    {
        return validation.ModifyUri(builder =>
            () =>
            {
                foreach (var (name, value) in parameters)
                {
                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    builder.AddUriParam(name, value);
                }
            });
    }
    
    /// <summary>
    /// Создает адрес
    /// </summary>
    /// <param name="validation"></param>
    /// <returns>Адрес запроса</returns>
    public static Validation<Error, Uri> Build(this Validation<Error, HttpUriBuilder> validation) =>
        validation.Bind(BuildPrivate);

    /// <summary>
    /// Создает адрес
    /// </summary>
    /// <param name="validation"></param>
    /// <returns>Адрес запроса</returns>
    public static Task<Validation<Error, Uri>> BuildAsync(this Validation<Error, HttpUriBuilder> validation) =>
        validation.Build().AsTask();

    /// <summary>
    /// Изменяет Uri
    /// </summary>
    /// <param name="validation"></param>
    /// <param name="func">Действие по изменению Uri</param>
    /// <returns></returns>
    private static Validation<Error, HttpUriBuilder> ModifyUri(
        this Validation<Error, HttpUriBuilder> validation,
        Func<HttpUriBuilder, Action> func)
    {
        return validation.Bind(builder =>
        {
            return Try(() =>
                {
                    var action = func(builder);
                    action();
                    return builder;
                })
                .Match(
                    Succ: uriBuilder => uriBuilder,
                    Fail: exception => Fail<Error, HttpUriBuilder>(
                        Error.New("Во время изменения Uri произошла ошибка", exception)));
        });
    }

    /// <summary>
    /// Собирает Uri
    /// </summary>
    /// <param name="builder">Билдер создания Uri</param>
    /// <returns></returns>
    private static Validation<Error, Uri> BuildPrivate(HttpUriBuilder builder)
    {
        try
        {
            builder.UriBuilder.Path = string.Join(SEPARATOR, builder.Segments);
            builder.UriBuilder.Query = builder.Query.ToString();
            
            var uri = new Uri(builder.UriBuilder.ToString());
            uri.Validate();
            
            return uri;
        }
        catch (Exception ex)
        {
            return Fail<Error, Uri>(ex);
        }
    }
}