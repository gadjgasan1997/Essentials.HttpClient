using System.Diagnostics.CodeAnalysis;
using Essentials.Utils.Extensions;
using static System.Environment;
using static Essentials.Serialization.Helpers.JsonHelpers;
using static Essentials.HttpClient.Logging.LogManager;
using static Essentials.HttpClient.Dictionaries.ErrorMessages;

namespace Essentials.HttpClient;

/// <summary>
/// Контекст запроса
/// </summary>
public static class HttpRequestContext
{
    private static readonly AsyncLocal<RequestContext> _context = new();
    
    /// <summary>
    /// Контекст запроса
    /// </summary>
    public static RequestContext? Current
    {
        get => _context.Value;
        private set => _context.Value = value.CheckNotNull();
    }

    /// <summary>
    /// Создает контекст Http запроса
    /// </summary>
    /// <param name="request">Запрос</param>
    [MemberNotNull(nameof(Current))]
    internal static IDisposable CreateContext(IRequest request)
    {
        try
        {
            Current ??= new RequestContext(request);
            
            return GetCurrentRequestLogger().PushScopeProperties(new []
            {
                new KeyValuePair<string, object>("http_request_id", request.Id),
                new KeyValuePair<string, object>("http_request_client_name", request.ClientName),
                new KeyValuePair<string, object>("http_request_type_id", request.TypeId)
            });
        }
        catch (Exception exception)
        {
            MainLogger.Error(
                exception,
                "Во время создания контекста запроса произошло исключение." +
                $"{NewLine}Запрос: '{Serialize(request)}'");
            
            throw;
        }
    }

    /// <summary>
    /// Восстанавливает контекст запроса из полученного ответа
    /// </summary>
    /// <param name="response">Ответ</param>
    [MemberNotNull(nameof(Current))]
    internal static IDisposable RestoreContext(IResponse response)
    {
        try
        {
            var scope = CreateContext(response.Request);
            Current.SetResponse(response.ResponseMessage);
            return scope;
        }
        catch (Exception exception)
        {
            MainLogger.Error(
                exception,
                "Во время восстановления контекста запроса из ответа произошло исключение." +
                $"{NewLine}Ответ: '{Serialize(response)}'");
            
            throw;
        }
    }
    
    /// <summary>
    /// Контекст запроса
    /// </summary>
    public class RequestContext
    {
        internal RequestContext(IRequest request)
        {
            Request = request.CheckNotNull();
        }
    
        /// <summary>
        /// Запрос
        /// </summary>
        public IRequest Request { get; }
    
        /// <summary>
        /// Сообщение запроса
        /// </summary>
        public HttpRequestMessage? RequestMessage { get; internal set; }
        
        /// <summary>
        /// Сообщение с ответом
        /// </summary>
        public HttpResponseMessage? ResponseMessage { get; private set; }
    
        /// <summary>
        /// Возникше исключение
        /// </summary>
        public Exception? Exception { get; private set; }
    
        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string? ErrorMessage { get; private set; }
    
        /// <summary>
        /// Затраченное время на обработку запроса, в мс
        /// </summary>
        public long? ElapsedMilliseconds { get; private set; }

        /// <summary>
        /// Устанавливает ответ
        /// </summary>
        /// <param name="responseMessage">Сообщение с ответом</param>
        internal void SetResponse(HttpResponseMessage responseMessage)
        {
            ResponseMessage = responseMessage.CheckNotNull();
        }

        /// <summary>
        /// Устанавливает ошибку
        /// </summary>
        /// <param name="exception">Возникше исключение</param>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        internal void SetError(
            Exception exception,
            string? errorMessage = null)
        {
            Exception = exception.CheckNotNull();
            ErrorMessage = errorMessage ?? ErrorSendRequest;
        }

        /// <summary>
        /// Устанавливает затраченное время на обработку запроса, в мс
        /// </summary>
        /// <param name="elapsedMilliseconds">Затраченное время на обработку запроса, в мс</param>
        internal void SetElapsedTime(long elapsedMilliseconds) => ElapsedMilliseconds = elapsedMilliseconds;
    }
}