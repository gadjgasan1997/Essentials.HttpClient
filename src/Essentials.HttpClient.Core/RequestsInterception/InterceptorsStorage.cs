using Essentials.HttpClient.Events;
using Essentials.HttpClient.Logging;
using Essentials.HttpClient.Metrics;

namespace Essentials.HttpClient.RequestsInterception;

/// <summary>
/// Хранилище интерсепторов
/// </summary>
internal static class InterceptorsStorage
{
    private static readonly List<Type> _defaultInterceptors = [];
    
    private static readonly List<Type> _interceptorsToRegister = [
        typeof(RequestsTimerInterceptor),
        typeof(LoggingInterceptor),
        typeof(MetricsInterceptor)];
    
    /// <summary>
    /// Пытается добавить по-умолчанию, который будет автоматически добавляться ко всем запросам
    /// </summary>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    public static void TryAttachDefaultInterceptor<TInterceptor>()
        where TInterceptor : IRequestInterceptor
    {
        TryAddInterceptorToRegister<TInterceptor>();
        
        if (_defaultInterceptors.Contains(typeof(TInterceptor)))
            return;
        
        _defaultInterceptors.Add(typeof(TInterceptor));
    }

    /// <summary>
    /// Пытается добавить интерсептор для регистрации, если он еще не был добавлен
    /// </summary>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    public static void TryAddInterceptorToRegister<TInterceptor>()
        where TInterceptor : IRequestInterceptor
    {
        if (_interceptorsToRegister.Contains(typeof(TInterceptor)))
            return;
        
        _interceptorsToRegister.Add(typeof(TInterceptor));
    }

    /// <summary>
    /// Проверяет, что интерсептор с указанным типом был зарегистрирован, в противном случае выкидывает исключение 
    /// </summary>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    /// <exception cref="KeyNotFoundException">Исключение о незарегистрированном интерсепторе</exception>
    public static void CheckInterceptorIsRegistered<TInterceptor>()
        where TInterceptor : IRequestInterceptor
    {
        if (_interceptorsToRegister.Contains(typeof(TInterceptor)))
            return;
        
        throw new KeyNotFoundException(
            $"Интерсептор с типом '{typeof(TInterceptor)}' не был зарегистрирован. " +
            $"Зарегистрируйте его с помощью конфигуратора http клиента");
    }

    /// <summary>
    /// Возвращает список интерсепторов по-умолчанию
    /// </summary>
    /// <returns></returns>
    public static List<Type> GetInterceptorsToAttach(IEnumerable<Type> requestInterceptors) =>
        _defaultInterceptors.Concat(requestInterceptors).ToList();
    
    /// <summary>
    /// Возвращает список интерсепторов для регистрации
    /// </summary>
    /// <returns>Список интерсепторов</returns>
    public static List<Type> GetInterceptorsToRegister() => _interceptorsToRegister;
}