using Essentials.HttpClient.Events;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Metrics;
using Microsoft.Extensions.DependencyInjection;

namespace Essentials.HttpClient.RequestsInterception;

/// <summary>
/// Хранилище интерсепторов
/// </summary>
internal static class InterceptorsStorage
{
    private static ServiceLifetime DefaultServiceLifetime => ServiceLifetime.Singleton;
    
    private static readonly List<Type> _globalInterceptors = [];
    
    private static readonly Dictionary<Type, ServiceLifetime> _interceptorsToRegister = new()
    {
        [typeof(RequestsTimerInterceptor)] = DefaultServiceLifetime,
        [typeof(LoggingInterceptor)] = DefaultServiceLifetime,
        [typeof(MetricsInterceptor)] = DefaultServiceLifetime
    };
    
    /// <summary>
    /// Пытается добавить по-умолчанию, который будет автоматически добавляться ко всем запросам
    /// </summary>
    /// <param name="serviceLifetime">Требуемое время жизни интерсептора, по-умолчанию <see cref="ServiceLifetime.Singleton" /></param>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    public static void TryAttachGlobalInterceptor<TInterceptor>(ServiceLifetime? serviceLifetime = null)
        where TInterceptor : IRequestInterceptor
    {
        if (_globalInterceptors.Contains(typeof(TInterceptor)))
            return;
        
        TryAddInterceptorToRegister<TInterceptor>(serviceLifetime);
        
        _globalInterceptors.Add(typeof(TInterceptor));
    }

    /// <summary>
    /// Пытается добавить интерсептор для регистрации, если он еще не был добавлен
    /// </summary>
    /// <param name="serviceLifetime">Требуемое время жизни интерсептора, по-умолчанию <see cref="ServiceLifetime.Singleton" /></param>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    public static void TryAddInterceptorToRegister<TInterceptor>(ServiceLifetime? serviceLifetime = null)
        where TInterceptor : IRequestInterceptor
    {
        if (_interceptorsToRegister.ContainsKey(typeof(TInterceptor)))
            return;
        
        _interceptorsToRegister.Add(typeof(TInterceptor), serviceLifetime ?? DefaultServiceLifetime);
    }

    /// <summary>
    /// Проверяет, что интерсептор с указанным типом был зарегистрирован, в противном случае выкидывает исключение 
    /// </summary>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    /// <exception cref="KeyNotFoundException">Исключение о незарегистрированном интерсепторе</exception>
    public static void CheckInterceptorIsRegistered<TInterceptor>()
        where TInterceptor : IRequestInterceptor
    {
        if (_interceptorsToRegister.ContainsKey(typeof(TInterceptor)))
            return;
        
        throw new KeyNotFoundException(
            $"Интерсептор с типом '{typeof(TInterceptor)}' не был зарегистрирован. " +
            $"Зарегистрируйте его с помощью конфигуратора http клиента");
    }

    /// <summary>
    /// Возвращает список интерсепторов по-умолчанию
    /// </summary>
    /// <param name="requestInterceptors">Список перехватчиков запросов</param>
    /// <param name="ignoredGlobalInterceptors">Список глобальных перехватчиков запросов, которые необходимо игнорировать</param>
    /// <returns></returns>
    public static List<Type> GetInterceptorsToAttach(
        IEnumerable<Type> requestInterceptors,
        IEnumerable<Type> ignoredGlobalInterceptors)
    {
        return _globalInterceptors
            .Except(ignoredGlobalInterceptors)
            .Concat(requestInterceptors)
            .Distinct()
            .ToList();
    }
    
    /// <summary>
    /// Возвращает список интерсепторов для регистрации
    /// </summary>
    /// <returns>Список интерсепторов</returns>
    public static List<Type> GetInterceptorsToRegister() => _interceptorsToRegister.Keys.ToList();
}