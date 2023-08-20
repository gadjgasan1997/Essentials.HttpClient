using Essentials.HttpClient.Events.Args;
using Essentials.HttpClient.Extensions;
using static System.Environment;
using static Essentials.Func.Utils.Helpers.JsonHelpers;
using static Essentials.HttpClient.Dictionaries.Loggers;
using static Essentials.HttpClient.Events.EventsPublisher;

namespace Essentials.HttpClient.Logging;

/// <summary>
/// Класс-подписчик на события для их логирования
/// </summary>
internal class LogSubscriber
{
    private readonly LoggingOptions _options;

    public LogSubscriber(LoggingOptions? options = null)
    {
        _options = options ?? new LoggingOptions();
    }
    
    public void SubscribeToLogEvents()
    {
        OnSuccessCreateUri += _options.SuccessCreateUriHandler ?? SuccessCreateUriHandler;
        OnErrorCreateUri += _options.ErrorCreateUriHandler ?? ErrorCreateUriHandler;
        OnSuccessCreateRequest += _options.SuccessCreateRequestHandler ?? SuccessCreateRequestHandler;
        OnErrorCreateRequest += _options.ErrorCreateRequestHandler ?? ErrorCreateRequestHandler;
        OnBadRequest += _options.BadRequestHandler ?? BadRequestHandler;
        OnSerializeError += _options.SerializeErrorHandler ?? SerializeErrorHandler;
        OnSuccessSend += _options.SuccessSendHandler ?? SuccessSendHandler;
        OnErrorSend += _options.ErrorSendHandler ?? ErrorSendHandler;
        OnBadStatusCode += _options.BadStatusCodeHandler ?? BadStatusCodeHandler;
        OnErrorReadContent += _options.ErrorReadContentHandler ?? ErrorReadContentHandler;
        OnDeserializeError += _options.DeserializeErrorHandler ?? DeserializeErrorHandler;
    }

    private static void SuccessCreateUriHandler(SuccessCreateUriEventArgs args)
    {
        MainLogger.Debug($"Uri был успешно создан. Uri: '{args.Uri}'");
    }

    private static void ErrorCreateUriHandler(ErrorCreateUriEventArgs args)
    {
        MainLogger.Error(
            args.Exception,
            message: $"{args.ErrorMessage}{NewLine}Используемый базовый адрес: '{args.Address}'.");
    }

    private static void SuccessCreateRequestHandler(SuccessCreateRequestEventArgs args)
    {
        MainLogger.Debug(
            $"Запрос по адресу '{args.Request.Uri}' был успешно создан." +
            $"{NewLine}Используемый Http клиент: '{args.Request.ClientName}'");
        
        MainLogger.Trace(
            $"Запрос по адресу '{args.Request.Uri}' был успешно создан." +
            $"{NewLine}Запрос: '{Serialize(args.Request)}'");
    }

    private static void ErrorCreateRequestHandler(ErrorCreateRequestEventArgs args)
    {
        MainLogger.Error(
            args.Exception,
            message: $"{args.ErrorMessage}{NewLine}Используемый адрес: '{args.Uri}'.");
    }

    private static void BadRequestHandler(BadRequestEventArgs args)
    {
        MainLogger.Error(args.Exception);
    }

    private static void SerializeErrorHandler(SerializeErrorEventArgs args)
    {
        MainLogger.Error(
            args.Exception,
            args.ErrorMessage +
            $"{NewLine}Запрос: '{Serialize(args.Request)}'" +
            $"{NewLine}Содержимое запроса: '{Serialize(args.Content)}'");
    }

    private static void SuccessSendHandler(SuccessSendRequestEventArgs args)
    {
        MainLogger.Info(
            $"Запрос по адресу '{args.Request.Uri}' был успешно отправлен. " +
            $"Код ответа: '{args.Response.ResponseMessage.StatusCode}'");
        
        MainLogger.Debug(
            $"Запрос по адресу '{args.Request.Uri}' был успешно отправлен. " +
            $"Код ответа: '{args.Response.ResponseMessage.StatusCode}'" +
            $"{NewLine}Запрос: '{Serialize(args.Request)}'" +
            $"{NewLine}Ответ: '{Serialize(args.Response)}'");
    }

    private static void ErrorSendHandler(ErrorSendRequestEventArgs args)
    {
        MainLogger.Error(
            args.Exception,
            args.ErrorMessage +
            $"{NewLine}Запрос: '{Serialize(args.Request)}'");
    }

    private static void BadStatusCodeHandler(BadStatusCodeEventArgs args)
    {
        MainLogger.Error(
            $"В ответ на Http запрос был получен ошибочный Http код ответа: '{args.ErrorCode}'" +
            $"{NewLine}Запрос: '{Serialize(args.Request)}'" +
            $"{NewLine}Ответ: '{Serialize(args.ResponseMessage)}'");
    }

    private static void ErrorReadContentHandler(ErrorReadContentEventArgs args)
    {
        MainLogger.Error(
            args.Exception,
            args.ErrorMessage +
            $"{NewLine}Ответ: '{Serialize(args.ResponseMessage)}'");
    }

    private static void DeserializeErrorHandler(DeserializeErrorEventArgs args)
    {
        var responseString = args.Response.ResponseMessage.ReceiveStringUnsafeAsync().Result;

        MainLogger.Error(
            args.Exception,
            args.ErrorMessage +
            $"{NewLine}Ответ: '{Serialize(args.Response)}'" +
            $"{NewLine}Строка ответа: '{responseString}'");
    }
}