using System.Diagnostics.Contracts;
using Essentials.HttpClient.RequestsInterception;
using static System.Environment;
using static System.DateTime;
using static Essentials.Utils.Dictionaries.KnownDatesFormats;
using static Essentials.Serialization.Helpers.JsonHelpers;
using static Essentials.HttpClient.Logging.LogManager;
using Context = Essentials.HttpClient.HttpRequestContext;
// ReSharper disable ClassNeverInstantiated.Global

namespace Essentials.HttpClient.Logging;

/// <summary>
/// Интерсептор логирования
/// </summary>
public sealed class LoggingInterceptor : IRequestInterceptor
{
    /// <inheritdoc cref="IRequestInterceptor.Intercept" />
    public async Task<HttpResponseMessage> Intercept(NextRequestDelegate next)
    {
        Contract.Assert(Context.Current is not null);
        Contract.Assert(Context.Current.RequestMessage is not null);
        
        var request = Context.Current.Request;
        var requestMessage = Context.Current.RequestMessage;
        
        string? requestString = null;
        
        var logger = GetCurrentRequestLogger();
        if (logger.IsDebugEnabled)
        {
            requestString ??= await ReadRequestDataAsync(requestMessage).ConfigureAwait(false);

            logger.Debug(
                $"Происходит отправка '{requestMessage.Method}' " +
                $"запроса по адресу '{request.Uri}' " +
                $"в '{Now.ToString(LogDateLongFormat)}'." +
                $"{NewLine}Запрос: '{Serialize(requestMessage)}'" +
                $"{NewLine}Строка запроса: '{requestString}'");
        }
        else if (logger.IsInfoEnabled)
        {
            logger.Info(
                $"Происходит отправка '{requestMessage.Method}' " +
                $"запроса по адресу '{request.Uri}' " +
                $"в '{Now.ToString(LogDateLongFormat)}'.");
        }
        
        HttpResponseMessage responseMessage;
        try
        {
            responseMessage = await next().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            requestString ??= await ReadRequestDataAsync(requestMessage).ConfigureAwait(false);
            
            logger.Error(
                exception,
                exception.Message +
                $"{NewLine}Дата и время получения исключения: '{Now.ToString(LogDateLongFormat)}'. " +
                GetElapsedTimeLogString(Context.Current.ElapsedMilliseconds) +
                $"{NewLine}Запрос: '{Serialize(requestMessage)}'" +
                $"{NewLine}Строка запроса: '{requestString}'");
            
            throw;
        }
        
        if (!responseMessage.IsSuccessStatusCode)
        {
            requestString ??= await ReadRequestDataAsync(requestMessage).ConfigureAwait(false);

            logger.Error(
                $"В ответ на Http запрос был получен ошибочный Http код ответа: '{responseMessage.StatusCode}'" +
                $"{NewLine}Дата и время получения ответа: '{Now.ToString(LogDateLongFormat)}'. " +
                GetElapsedTimeLogString(Context.Current.ElapsedMilliseconds) +
                $"{NewLine}Запрос: '{Serialize(requestMessage)}'" +
                $"{NewLine}Строка запроса: '{requestString}'" +
                $"{NewLine}Ответ: '{Serialize(responseMessage)}'");

            return responseMessage;
        }

        if (logger.IsDebugEnabled)
        {
            var responseString = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            logger.Debug(
                $"Запрос по адресу '{request.Uri}' " +
                $"вернул код '{responseMessage.StatusCode}' в '{Now.ToString(LogDateLongFormat)}'. " +
                GetElapsedTimeLogString(Context.Current.ElapsedMilliseconds) +
                $"{NewLine}Ответ: '{Serialize(responseMessage)}'" +
                $"{NewLine}Строка ответа: '{responseString}'");
        }
        else if (logger.IsInfoEnabled)
        {
            logger.Info(
                $"Запрос по адресу '{request.Uri}' " +
                $"вернул код '{responseMessage.StatusCode}' в '{Now.ToString(LogDateLongFormat)}'. " +
                GetElapsedTimeLogString(Context.Current.ElapsedMilliseconds));
        }

        return responseMessage;
    }

    private static async Task<string> ReadRequestDataAsync(HttpRequestMessage requestMessage) =>
        requestMessage.Content is null
            ? "No http content"
            : await requestMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
    
    private static string GetElapsedTimeLogString(long? elapsedMilliseconds) =>
        string.IsNullOrWhiteSpace(elapsedMilliseconds?.ToString())
            ? "Не удалось засечь время выполнения запроса"
            : $"Время выполнения запроса, в милисекундах: '{elapsedMilliseconds}'.";
}