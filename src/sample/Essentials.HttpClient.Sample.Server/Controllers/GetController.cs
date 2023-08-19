using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using static Essentials.HttpClient.Sample.Server.Helpers.PersonsHelpers;
using static Essentials.HttpClient.Sample.Server.Helpers.ResponseHelpers;
using static Essentials.HttpClient.Common.Helpers.SerializationHelpers;

namespace Essentials.HttpClient.Sample.Server.Controllers;

[ApiController, Route("[controller]")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class GetController
{
    [HttpGet(nameof(GetPersonsInJson))]
    public ContentResult GetPersonsInJson([FromHeader] string? personName, [FromQuery] int? age)
    {
        try
        {
            return new ContentResult
            {
                Content = SerializeInJson(GetPersons(personName, age)),
                ContentType = MediaTypeNames.Application.Json
            };
        }
        catch (Exception ex)
        {
            return GetErrorContent(ex);
        }
    }
    
    [HttpGet(nameof(GetPersonsInXml))]
    public ContentResult GetPersonsInXml([FromHeader] string? personName, [FromQuery] int? age)
    {
        try
        {
            return new ContentResult
            {
                Content = SerializeInXml(GetPersons(personName, age)),
                ContentType = MediaTypeNames.Application.Xml
            };
        }
        catch (Exception ex)
        {
            return GetErrorContent(ex);
        }
    }
}