using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using static Essentials.HttpClient.Common.Models.PersonsStorage;
using static Essentials.HttpClient.Sample.Server.Helpers.SerializationHelpers;
using static Essentials.HttpClient.MediaTypes.Storage;

namespace Essentials.HttpClient.Sample.Server.Controllers;

[ApiController, Route("[controller]")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class GetController
{
    [HttpGet(nameof(GetPersonsInJson))]
    public ContentResult GetPersonsInJson([FromHeader] string? personName, [FromQuery] int? age)
    {
        var persons = DefaultPersons;
        if (!string.IsNullOrWhiteSpace(personName))
            persons = persons.Where(person => person.Name.Contains(personName)).ToList();
        if (age.HasValue)
            persons = persons.Where(person => person.Age == age.Value).ToList();

        return new ContentResult
        {
            Content = SerializeInJson(persons),
            ContentType = Application.Json.ToString()
        };
    }
}