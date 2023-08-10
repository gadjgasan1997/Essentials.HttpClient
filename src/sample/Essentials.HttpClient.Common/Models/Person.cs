#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Essentials.HttpClient.Common.Models;

public class Person
{
    public Person() { }
    
    public Person (string name, int age)
    {
        Name = name;
        Age = age;
    }
    
    public string Name { get; set; }
    
    public int Age { get; set; }
}

public static class PersonsStorage
{
    public static List<Person> DefaultPersons => new()
    {
        new Person("Gasan", 26),
        new Person("Umid", 24)
    };
}