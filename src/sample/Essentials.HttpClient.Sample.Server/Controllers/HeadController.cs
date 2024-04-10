using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace Essentials.HttpClient.Sample.Server.Controllers;

[ApiController, Route("[controller]")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class HeadController
{
    [HttpHead(nameof(Head))]
    public IActionResult Head() => new OkResult();
}