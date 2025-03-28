using Microsoft.AspNetCore.Mvc;
using Services.ServicesHelpers.GoogleMeetService;
using Services.ApiModels;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class GoogleMeetController : ControllerBase
{
    private readonly GoogleMeetService _googleMeetService;

    public GoogleMeetController(GoogleMeetService googleMeetService)
    {
        _googleMeetService = googleMeetService;
    }

    [HttpGet("get-auth-url")]
    public IActionResult GetAuthorizationUrl()
    {
        var url = _googleMeetService.GetAuthorizationUrl();
        return Ok(url);
    }

    [HttpPost("exchange-token")]
    public async Task<IActionResult> ExchangeToken([FromQuery] string code)
    {
        var result = await _googleMeetService.ExchangeCodeForToken(code);
        return Ok(result);
    }
   
    [HttpPost("create-meet")]
    public async Task<IActionResult> CreateGoogleMeet([FromBody] MeetRequest request)
    {
        var meetLink = await _googleMeetService.CreateGoogleMeet(request);
        return Ok(meetLink);
    }
}
