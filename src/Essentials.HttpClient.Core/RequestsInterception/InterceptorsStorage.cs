using Essentials.HttpClient.Events;
using Essentials.HttpClient.Logging;

namespace Essentials.HttpClient.RequestsInterception;

/// <summary>
/// Хранилище интерсепторов
/// </summary>
internal static class InterceptorsStorage
{
    private static readonly List<Type> _interceptors = [
        typeof(RequestsTimerInterceptor),
        typeof(LoggingInterceptor)];

    /// <summary>
    /// Пытается добавить интерсептор для регистрации, если он еще не был добавлен
    /// </summary>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    public static void TryAddInterceptor<TInterceptor>()
        where TInterceptor : IRequestInterceptor
    {
        if (_interceptors.Contains(typeof(TInterceptor)))
            return;
        
        _interceptors.Add(typeof(TInterceptor));
    }

    /// <summary>
    /// Проверяет, что интерсептор с указанным типом был зарегистрирован, в противном случае выкидывает исключение 
    /// </summary>
    /// <typeparam name="TInterceptor">Тип интерсептора</typeparam>
    /// <exception cref="KeyNotFoundException">Исключение о незарегистрированном интерсепторе</exception>
    public static void CheckInterceptorIsRegistered<TInterceptor>()
        where TInterceptor : IRequestInterceptor
    {
        if (_interceptors.Contains(typeof(TInterceptor)))
            return;
        
        throw new KeyNotFoundException(
            $"Интерсептор с типом '{typeof(TInterceptor)}' не был зарегистрирован. " +
            $"Зарегистрируйте его с помощью конфигуратора http клиента");
    }
    
    /// <summary>
    /// Возвращает список интерсепторов для регистрации
    /// </summary>
    /// <returns>Список интерсепторов</returns>
    public static List<Type> GetInterceptorsToRegister() => _interceptors;
}