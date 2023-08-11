using Essentials.HttpClient.Sample.Server.Models;

namespace Essentials.HttpClient.Sample.Server.Helpers;

public static class PersonsHelpers
{
    public static List<Person> GetPersons(string? personName, int? age)
    {
        var persons = PersonsStorage.DefaultPersons;
        if (!string.IsNullOrWhiteSpace(personName))
            persons = persons.Where(person => person.Name.Contains(personName)).ToList();
        if (age.HasValue)
            persons = persons.Where(person => person.Age == age.Value).ToList();
        
        return persons;
    }
}