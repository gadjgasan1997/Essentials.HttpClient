namespace Essentials.HttpClient.Dictionaries;

/// <summary>
/// Известные схемы авторизации
/// </summary>
internal static class KnownAuthenticationSchemes
{
    /// <summary>
    /// Basic
    /// </summary>
    public const string BASIC = "Basic";

    /// <summary>
    /// Jwt токены
    /// </summary>
    public const string JWT = "Bearer";
}