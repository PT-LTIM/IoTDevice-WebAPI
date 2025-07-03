using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;

public class IoTHubService
{
    private readonly RegistryManager _registryManager;

    public IoTHubService(IConfiguration config)
    {
        var connectionString = config["AzureIoTHub:ConnectionString"];
        _registryManager = RegistryManager.CreateFromConnectionString(connectionString);
    }

    public async Task<Device> CreateDeviceAsync(string deviceId)
    {
        var device = new Device(deviceId);
        return await _registryManager.AddDeviceAsync(device);
    }

    public async Task<Device> GetDeviceAsync(string deviceId)
    {
        return await _registryManager.GetDeviceAsync(deviceId);
    }

    public async Task<Device> UpdateDeviceAsync(Device device)
    {
        return await _registryManager.UpdateDeviceAsync(device);
    }

    public async Task DeleteDeviceAsync(string deviceId)
    {
        await _registryManager.RemoveDeviceAsync(deviceId);
    }

    public async Task UpdateDesiredPropertiesAsync(string deviceId, TwinCollection desiredProperties)
    {
        var twin = await _registryManager.GetTwinAsync(deviceId);
        twin.Properties.Desired = desiredProperties;
        await _registryManager.UpdateTwinAsync(deviceId, twin, twin.ETag);
    }

    public async Task<Twin> GetReportedPropertiesAsync(string deviceId)
    {
        return await _registryManager.GetTwinAsync(deviceId);
    }
}
