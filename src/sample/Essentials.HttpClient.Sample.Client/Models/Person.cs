#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Essentials.HttpClient.Sample.Client.Models;

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