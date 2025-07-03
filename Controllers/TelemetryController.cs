using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryService _telemetryService;

    public TelemetryController(TelemetryService telemetryService)
    {
        _telemetryService = telemetryService;
    }

    /// <summary>
    /// Sends Telemetry messages to a device in Azure IoT Hub.
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> SendTelemetry([FromBody] TelemetryModel model)
    {
        await _telemetryService.SendTelemetryAsync(model.DeviceConnectionString, model.Message);
        return Ok("Telemetry sent.");
    }
}
