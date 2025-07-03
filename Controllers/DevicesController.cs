using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using TransportType = Microsoft.Azure.Devices.Client.TransportType;

[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IoTHubService _iotHubService;
    private readonly IConfiguration _configuration;
    public DevicesController(IoTHubService iotHubService, IConfiguration configuration)
    {
        _iotHubService = iotHubService;
        _configuration = configuration;
    }

    /// <summary>
    /// Creates a new device in Azure IoT Hub.
    /// </summary>
    [HttpPost("create")]
    public async Task<IActionResult> CreateDevice([FromBody] DeviceModel model)
    {
        var device = await _iotHubService.CreateDeviceAsync(model.DeviceId);
        return Ok(device);
    }

    /// <summary>
    /// Get a device in Azure IoT Hub.
    /// </summary>
    [HttpGet("{deviceId}")]
    public async Task<IActionResult> GetDevice(string deviceId)
    {
        var device = await _iotHubService.GetDeviceAsync(deviceId);
        return Ok(device);
    }

    /// <summary>
    /// Updates a device in Azure IoT Hub.
    /// </summary>
    [HttpPut("update/{deviceId}")]
    public async Task<IActionResult> UpdateDevice([FromBody] Device device, string deviceId)
    {
        var deviceUpdate = await _iotHubService.GetDeviceAsync(deviceId);
            deviceUpdate.StatusReason = device.StatusReason;
            deviceUpdate.Status = device.Status;
        var updated = await _iotHubService.UpdateDeviceAsync(deviceUpdate);
        return Ok(updated);

    }

    /// <summary>
    /// Deletes a device in Azure IoT Hub.
    /// </summary>
    [HttpDelete("{deviceId}")]
    public async Task<IActionResult> DeleteDevice(string deviceId)
    {
        await _iotHubService.DeleteDeviceAsync(deviceId);
        return NoContent();
    }

    /// <summary>
    /// Update DesiredProperties of a device in Azure IoT Hub.
    /// </summary>
    [HttpPut("desired/{deviceId}")]
    public async Task<IActionResult> UpdateDesiredProperties(string deviceId, [FromBody] Dictionary<string, object> properties)
    {
        var twinCollection = new TwinCollection();
        foreach (var prop in properties)
            twinCollection[prop.Key] = prop.Value;
        await _iotHubService.UpdateDesiredPropertiesAsync(deviceId, twinCollection);
        return Ok();
    }

    /// <summary>
    /// Update ReportedProperties of a device in Azure IoT Hub.
    /// </summary>
     [HttpPut("reported/{deviceId}")]
    public async Task<IActionResult> UpdateReportedProperties(string deviceId, [FromBody] Dictionary<string, object> properties)
    {
        
         var deviceClient = DeviceClient.CreateFromConnectionString(GetDeviceConnectionString(deviceId).Result, TransportType.Mqtt);
            TwinCollection reportedProperties = new TwinCollection();
         foreach (var prop in properties)
            reportedProperties[prop.Key] = prop.Value;
            await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);
        return Ok();
    }

    /// <summary>
    /// Get ReportedProperties of a device in Azure IoT Hub.
    /// </summary>
    [HttpGet("reported/{deviceId}")]
    public async Task<IActionResult> GetReportedProperties(string deviceId)
    {
        var twin = await _iotHubService.GetReportedPropertiesAsync(deviceId);
        return Ok(twin.Properties.Reported);
    }
    
    private async Task<string> GetDeviceConnectionString(string deviceId)
        {
           var iotHubConnectionString = _configuration["AzureIoTHub:ConnectionString"];
            var registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
            var device = await registryManager.GetDeviceAsync(deviceId);

            if (device == null)
            {
                Console.WriteLine("Device not found.");
                return null;
            }

            string hostName = iotHubConnectionString.Split(';')[0].Split('=')[1];
            string key = device.Authentication.SymmetricKey.PrimaryKey;

            return $"HostName={hostName};DeviceId={deviceId};SharedAccessKey={key}";
        }

}
