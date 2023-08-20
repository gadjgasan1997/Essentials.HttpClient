using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text;
using Essentials.HttpClient.Sample.Server.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using static Essentials.HttpClient.Sample.Server.Helpers.PersonsHelpers;
using static Essentials.HttpClient.Sample.Server.Helpers.ResponseHelpers;
using static Essentials.HttpClient.Common.Helpers.SerializationHelpers;

namespace Essentials.HttpClient.Sample.Server.Controllers;

[ApiController, Route("[controller]")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class PostController : Controller
{
    [HttpPost(nameof(GetPersonsInJson))]
    public ContentResult GetPersonsInJson([FromBody] GetPersonsInJsonRequest request)
    {
        try
        {
            return new ContentResult
            {
                Content = SerializeInJson(GetPersons(request.Name, request.Age)),
                ContentType = MediaTypeNames.Application.Json
            };
        }
        catch (Exception ex)
        {
            return GetErrorContent(ex);
        }
    }
    
    [HttpPost(nameof(GetPersonsInXml))]
    public ContentResult GetPersonsInXml([FromBody] GetPersonsInXmlRequest request)
    {
        try
        {
            return new ContentResult
            {
                Content = SerializeInXml(GetPersons(request.Name, request.Age)),
                ContentType = MediaTypeNames.Application.Json
            };
        }
        catch (Exception ex)
        {
            return GetErrorContent(ex);
        }
    }
    
    [HttpPost(nameof(GetPersonsInPlainXmlText))]
    public async Task<ContentResult> GetPersonsInPlainXmlText()
    {
        try
        {
            string xmlString;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                xmlString = await reader.ReadToEndAsync();
            }

            var request = DeserializeXml<GetPersonsInXmlRequest>(xmlString);
            return new ContentResult
            {
                Content = SerializeInXml(GetPersons(request.Name, request.Age)),
                ContentType = MediaTypeNames.Application.Json
            };
        }
        catch (Exception ex)
        {
            return GetErrorContent(ex);
        }
    }
}