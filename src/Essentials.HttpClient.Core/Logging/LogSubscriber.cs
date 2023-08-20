using NLog;
using Essentials.HttpClient.Events.Args;
using Essentials.HttpClient.Extensions;
using static System.Environment;
using static Essentials.Func.Utils.Helpers.JsonHelpers;
using static Essentials.HttpClient.Events.EventsPublisher;

namespace Essentials.HttpClient.Logging;

/// <summary>
/// Класс-подписчик на события для их логирования
/// </summary>
internal class LogSubscriber
{
    private readonly LoggingOptions _options;
    private static readonly Logger _defaultLogger = LogManager.GetLogger("Essentials.HttpClient.MainLogger");

    public LogSubscriber(LoggingOptions? options = null)
    {
        _options = options ?? new LoggingOptions();
    }
    
    public void SubscribeToLogEvents()
    {
        OnSuccessCreateUri += _options.SuccessCreateUriHandler ?? OnSuccessCreateUriHandler;
        OnErrorCreateUri += _options.ErrorCreateUriHandler ?? OnErrorCreateUriHandler;
        OnSuccessCreateRequest += _options.SuccessCreateRequestHandler ?? OnSuccessCreateRequestHandler;
        OnErrorCreateRequest += _options.ErrorCreateRequestHandler ?? OnErrorCreateRequestHandler;
        OnBadRequest += _options.BadRequestHandler ?? OnBadRequestHandler;
        OnSerializeError += _options.SerializeErrorHandler ?? OnSerializeErrorHandler;
        OnSuccessSend += _options.SuccessSendHandler ?? OnSuccessSendHandler;
        OnErrorSend += _options.ErrorSendHandler ?? OnErrorSendHandler;
        OnBadStatusCode += _options.BadStatusCodeHandler ?? OnBadStatusCodeHandler;
        OnErrorReadContent += _options.ErrorReadContentHandler ?? OnErrorReadContentHandler;
        OnDeserializeError += _options.DeserializeErrorHandler ?? OnDeserializeErrorHandler;
    }

    private static void OnSuccessCreateUriHandler(SuccessCreateUriEventArgs args)
    {
        _defaultLogger.Debug($"Uri был успешно создан. Uri: '{args.Uri}'");
    }

    private static void OnErrorCreateUriHandler(ErrorCreateUriEventArgs args)
    {
        _defaultLogger.Error(
            args.Exception,
            message: $"{args.ErrorMessage}{NewLine}Используемый базовый адрес: '{args.Address}'.");
    }

    private static void OnSuccessCreateRequestHandler(SuccessCreateRequestEventArgs args)
    {
        _defaultLogger.Debug(
            $"Запрос по адресу '{args.Request.Uri}' был успешно создан." +
            $"{NewLine}Используемый Http клиент: '{args.Request.ClientName}'");
        
        _defaultLogger.Trace(
            $"Запрос по адресу '{args.Request.Uri}' был успешно создан." +
            $"{NewLine}Запрос: '{Serialize(args.Request)}'");
    }

    private static void OnErrorCreateRequestHandler(ErrorCreateRequestEventArgs args)
    {
        _defaultLogger.Error(
            args.Exception,
            message: $"{args.ErrorMessage}{NewLine}Используемый адрес: '{args.Uri}'.");
    }

    private static void OnBadRequestHandler(BadRequestEventArgs args)
    {
        _defaultLogger.Error(args.Exception);
    }

    private static void OnSerializeErrorHandler(SerializeErrorEventArgs args)
    {
        _defaultLogger.Error(
            args.Exception,
            args.ErrorMessage +
            $"{NewLine}Запрос: '{Serialize(args.Request)}'" +
            $"{NewLine}Содержимое запроса: '{Serialize(args.Content)}'");
    }

    private static void OnSuccessSendHandler(SuccessSendRequestEventArgs args)
    {
        _defaultLogger.Info(
            $"Запрос по адресу '{args.Request.Uri}' был успешно отправлен. " +
            $"Код ответа: '{args.Response.ResponseMessage.StatusCode}'");
        
        _defaultLogger.Debug(
            $"Запрос по адресу '{args.Request.Uri}' был успешно отправлен. " +
            $"Код ответа: '{args.Response.ResponseMessage.StatusCode}'" +
            $"{NewLine}Запрос: '{Serialize(args.Request)}'" +
            $"{NewLine}Ответ: '{Serialize(args.Response)}'");
    }

    private static void OnErrorSendHandler(ErrorSendRequestEventArgs args)
    {
        _defaultLogger.Error(
            args.Exception,
            args.ErrorMessage +
            $"{NewLine}Запрос: '{Serialize(args.Request)}'");
    }

    private static void OnBadStatusCodeHandler(BadStatusCodeEventArgs args)
    {
        _defaultLogger.Error(
            $"В ответ на Http запрос был получен ошибочный Http код ответа: '{args.ErrorCode}'" +
            $"{NewLine}Запрос: '{Serialize(args.Request)}'" +
            $"{NewLine}Ответ: '{Serialize(args.ResponseMessage)}'");
    }

    private static void OnErrorReadContentHandler(ErrorReadContentEventArgs args)
    {
        _defaultLogger.Error(
            args.Exception,
            args.ErrorMessage +
            $"{NewLine}Ответ: '{Serialize(args.ResponseMessage)}'");
    }

    private static void OnDeserializeErrorHandler(DeserializeErrorEventArgs args)
    {
        var responseString = args.Response.ResponseMessage.ReceiveStringUnsafeAsync().Result;

        _defaultLogger.Error(
            args.Exception,
            args.ErrorMessage +
            $"{NewLine}Ответ: '{Serialize(args.Response)}'" +
            $"{NewLine}Строка ответа: '{responseString}'");
    }
}