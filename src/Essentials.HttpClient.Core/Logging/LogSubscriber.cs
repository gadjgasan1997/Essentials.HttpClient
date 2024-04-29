﻿using System.Diagnostics.Contracts;
using static System.Environment;
using static System.DateTime;
using static Essentials.Serialization.Helpers.JsonHelpers;
using static Essentials.HttpClient.Logging.LogManager;
using static Essentials.HttpClient.Dictionaries.KnownDatesFormats;
using static Essentials.HttpClient.Events.EventsPublisher;
using static Essentials.HttpClient.Logging.LoggingOptions;
using Context = Essentials.HttpClient.HttpRequestContext;

namespace Essentials.HttpClient.Logging;

/// <summary>
/// Класс-подписчик на события для их логирования
/// </summary>
internal static class LogSubscriber
{
    public static void Subscribe()
    {
        if (DisableDefaultLogging)
            return;
        
        OnSerializeError += SerializeErrorHandler ?? DefaultSerializeErrorHandler;
        OnBeforeSend += BeforeSendHandler ?? DefaultBeforeSendHandler;
        OnSuccessSend += SuccessSendHandler ?? DefaultSuccessSendHandler;
        OnErrorSend += ErrorSendHandler ?? DefaultErrorSendHandler;
        OnBadStatusCode += BadStatusCodeHandler ?? DefaultBadStatusCodeHandler;
        OnErrorReadContent += ErrorReadContentHandler ?? DefaultErrorReadContentHandler;
        OnDeserializeError += DeserializeErrorHandler ?? DefaultDeserializeErrorHandler;
    }

    private static void DefaultSerializeErrorHandler()
    {
        TryLog(() =>
        {
            Contract.Assert(Context.Current is not null);

            var logger = GetCurrentRequestLogger();
            if (!logger.IsErrorEnabled) return;
            
            logger.Error(
                Context.Current.Exception,
                Context.Current.ErrorMessage);
        }, nameof(OnSerializeError));
    }

    private static void DefaultBeforeSendHandler()
    {
        TryLog(() =>
        {
            Contract.Assert(Context.Current is not null);
            Contract.Assert(Context.Current.RequestMessage is not null);
            
            var logger = GetCurrentRequestLogger();
            if (!logger.IsInfoEnabled)
                return;

            var request = Context.Current.Request;
            if (logger.IsDebugEnabled)
            {
                var requestMessage = Context.Current.RequestMessage;
                var requestString = requestMessage.Content?.ReadAsStringAsync().Result;
                
                logger.Debug(
                    $"Происходит отправка '{requestMessage.Method}' " +
                    $"запроса по адресу '{request.Uri}' " +
                    $"в '{Now.ToString(LogDateLongFormat)}'." +
                    $"{NewLine}Запрос: '{Serialize(requestMessage)}'" +
                    $"{NewLine}Строка запроса: '{requestString ?? "No http content"}'");
                
                return;
            }

            logger.Info(
                $"Происходит отправка '{Context.Current.RequestMessage.Method}' " +
                $"запроса по адресу '{Context.Current.Request.Uri}' " +
                $"в '{Now.ToString(LogDateLongFormat)}'.");
        }, nameof(OnBeforeSend));
    }

    private static void DefaultSuccessSendHandler()
    {
        TryLog(() =>
        {
            Contract.Assert(Context.Current is not null);
            Contract.Assert(Context.Current.ResponseMessage is not null);

            var logger = GetCurrentRequestLogger();
            if (!logger.IsInfoEnabled)
                return;

            var request = Context.Current.Request;
            var responseMessage = Context.Current.ResponseMessage;
            
            if (logger.IsDebugEnabled)
            {
                var responseString = responseMessage.Content.ReadAsStringAsync().Result;
            
                logger.Debug(
                    $"Запрос по адресу '{request.Uri}' " +
                    $"вернул код '{responseMessage.StatusCode}' в '{Now.ToString(LogDateLongFormat)}'. " +
                    GetElapsedTimeLogString(Context.Current.ElapsedMilliseconds) +
                    $"{NewLine}Ответ: '{Serialize(responseMessage)}'" +
                    $"{NewLine}Строка ответа: '{responseString}'");
                
                return;
            }

            logger.Info(
                $"Запрос по адресу '{request.Uri}' " +
                $"вернул код '{responseMessage.StatusCode}' в '{Now.ToString(LogDateLongFormat)}'. " +
                GetElapsedTimeLogString(Context.Current.ElapsedMilliseconds));
        }, nameof(OnSuccessSend));
    }

    private static void DefaultErrorSendHandler()
    {
        TryLog(() =>
        {
            Contract.Assert(Context.Current is not null);
            Contract.Assert(Context.Current.RequestMessage is not null);
            
            var logger = GetCurrentRequestLogger();
            if (!logger.IsErrorEnabled) return;
            
            var requestMessage = Context.Current.RequestMessage;
            var requestString = requestMessage.Content?.ReadAsStringAsync().Result;
            
            logger.Error(
                Context.Current.Exception,
                Context.Current.ErrorMessage +
                $"{NewLine}Дата и время получения исключения: '{Now.ToString(LogDateLongFormat)}'. " +
                GetElapsedTimeLogString(Context.Current.ElapsedMilliseconds) +
                $"{NewLine}Запрос: '{Serialize(requestMessage)}'" +
                $"{NewLine}Строка запроса: '{requestString ?? "No http content"}'");
        }, nameof(OnErrorSend));
    }

    private static void DefaultBadStatusCodeHandler()
    {
        TryLog(() =>
        {
            Contract.Assert(Context.Current is not null);
            Contract.Assert(Context.Current.ResponseMessage is not null);
            Contract.Assert(Context.Current.RequestMessage is not null);
            
            var logger = GetCurrentRequestLogger();
            if (!logger.IsErrorEnabled) return;

            var requestMessage = Context.Current.RequestMessage;
            var responseMessage = Context.Current.ResponseMessage;
            var requestString = requestMessage.Content?.ReadAsStringAsync().Result;

            logger.Error(
                $"В ответ на Http запрос был получен ошибочный Http код ответа: '{responseMessage.StatusCode}'" +
                $"{NewLine}Дата и время получения ответа: '{Now.ToString(LogDateLongFormat)}'. " +
                GetElapsedTimeLogString(Context.Current.ElapsedMilliseconds) +
                $"{NewLine}Запрос: '{Serialize(requestMessage)}'" +
                $"{NewLine}Строка запроса: '{requestString}'" +
                $"{NewLine}Ответ: '{Serialize(responseMessage)}'");
        }, nameof(OnBadStatusCode));
    }

    private static void DefaultErrorReadContentHandler()
    {
        TryLog(() =>
        {
            Contract.Assert(Context.Current is not null);
            
            var logger = GetCurrentRequestLogger();
            if (!logger.IsErrorEnabled) return;
            
            logger.Error(
                Context.Current.Exception,
                Context.Current.ErrorMessage +
                $"{NewLine}Ответ: '{Serialize(Context.Current.ResponseMessage)}'");
        }, nameof(OnErrorReadContent));
    }

    private static void DefaultDeserializeErrorHandler()
    {
        TryLog(() =>
        {
            Contract.Assert(Context.Current is not null);
            Contract.Assert(Context.Current.ResponseMessage is not null);
            
            var logger = GetCurrentRequestLogger();
            if (!logger.IsErrorEnabled) return;
            
            var responseString = Context.Current.ResponseMessage.Content.ReadAsStringAsync().Result;

            logger.Error(
                Context.Current.Exception,
                Context.Current.ErrorMessage +
                $"{NewLine}Ответ: '{Serialize(Context.Current.ResponseMessage)}'" +
                $"{NewLine}Строка ответа: '{responseString}'");
        }, nameof(OnDeserializeError));
    }
    
    private static void TryLog(Action action, string eventName)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            MainLogger.Error(ex, $"Во время записи лога события '{eventName}' произошло исключение");
        }
    }
    
    private static string GetElapsedTimeLogString(long? elapsedMilliseconds) =>
        string.IsNullOrWhiteSpace(elapsedMilliseconds?.ToString())
            ? "Не удалось засечь время выполнения запроса"
            : $"Время выполнения запроса, в милисекундах: '{elapsedMilliseconds}'.";
}