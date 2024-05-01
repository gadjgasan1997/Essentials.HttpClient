using System.Text;
using Essentials.Utils.Extensions;

namespace Essentials.HttpClient.Models;

/// <summary>
/// Id запроса
/// </summary>
public record RequestId
{
    private RequestId(string value, bool isDefault)
    {
        Value = value;
        IsDefault = isDefault;
    }
    
    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; }
    
    /// <summary>
    /// Признак, что Id был создан по-умолчанию
    /// </summary>
    public bool IsDefault { get; }

    /// <summary>
    /// Создает Id запроса из переданного
    /// </summary>
    /// <param name="id">Требуемый Id</param>
    /// <returns>Id запроса</returns>
    internal static RequestId CreateManual(string id) => new(CheckId(id), false);

    /// <summary>
    /// Создает Id запроса по-умолчанию
    /// </summary>
    /// <returns>Id запроса</returns>
    internal static RequestId CreateDefault() => new(GetDefaultId(), true);

    /// <summary>
    /// Обновляет Id запроса, если он был создан как Id по-умолчанию (т.е. не был задан явно)
    /// </summary>
    internal void RefreshIdIfDefault()
    {
        if (!IsDefault)
            return;
        
        Value = GetDefaultId();
    }
    
    /// <summary>
    /// Проверяет Id запроса
    /// </summary>
    /// <param name="requestId">Id запроса</param>
    /// <returns>Валидный Id запроса</returns>
    private static string CheckId(string requestId) =>
        requestId
            .CheckNotNullOrEmpty($"Id запроса не может быть пустым. Id запроса: '{requestId}'")
            .FullTrim()
            .ToLowerInvariant();

    /// <summary>
    /// Возвращает Id запроса по-умолчанию
    /// </summary>
    /// <returns>Id запроса</returns>
    private static string GetDefaultId() => Guid.NewGuid().ToString();

    /// <summary>
    /// Преобразует Id в строку
    /// </summary>
    /// <returns>Строка</returns>
    public override string ToString()
    {
        var builder = new StringBuilder();

        builder
            .Append("{ ")
            .Append("\"Id запроса\": \"")
            .Append(Value)
            .Append("\", ")
            .Append("\"Способ создания Id запроса\": \"")
            .Append(IsDefault ? "Id был сгенерирован" : "Id был задан вручную")
            .Append("\" }");
        
        return builder.ToString();
    }
}