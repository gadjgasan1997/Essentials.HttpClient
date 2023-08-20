namespace Essentials.HttpClient.Events;

/// <summary>
/// Делегат-обработчик события
/// </summary>
/// <typeparam name="TEventArgs">Тип аргументов</typeparam>
public delegate void Handler<in TEventArgs>(TEventArgs args) where TEventArgs : IEventArgs;