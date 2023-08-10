using System.Diagnostics.CodeAnalysis;
using Essentials.HttpClient.Common.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using static Essentials.HttpClient.Common.Models.PersonsStorage;
using static Essentials.HttpClient.Sample.Server.Helpers.SerializationHelpers;
using static Essentials.HttpClient.ContentTypes.Storage;

namespace Essentials.HttpClient.Sample.Server.Controllers;

[ApiController, Route("[controller]")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class PostController
{
    [HttpPost(nameof(GetPersonsInJson))]
    public ContentResult GetPersonsInJson([FromBody] GetPersonsInJsonRequest request)
    {
        var persons = DefaultPersons;
        if (!string.IsNullOrWhiteSpace(request.Name))
            persons = persons.Where(person => person.Name.Contains(request.Name)).ToList();
        if (request.Age.HasValue)
            persons = persons.Where(person => person.Age == request.Age.Value).ToList();

        return new ContentResult
        {
            Content = SerializeInJson(persons),
            ContentType = Application.Json.ToString()
        };
    }
}