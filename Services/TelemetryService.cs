using Microsoft.Azure.Devices.Client;
using System.Text;

public class TelemetryService
{
    public async Task SendTelemetryAsync(string deviceConnectionString, string message)
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);
        var telemetryMessage = new Message(Encoding.UTF8.GetBytes(message));
        await deviceClient.SendEventAsync(telemetryMessage);
    }
}
