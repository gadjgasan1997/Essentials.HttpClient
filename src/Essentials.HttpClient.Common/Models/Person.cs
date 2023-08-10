namespace Essentials.HttpClient.Common.Models;

public record Person(string Name, int Age);

public static class PersonsStorage
{
    public static List<Person> DefaultPersons => new()
    {
        new Person("Gasan", 26),
        new Person("Umid", 24)
    };
}