using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Models;
using Blazored.LocalStorage;

public class DeviceService : BaseApiService
{
    public DeviceService(HttpClient httpClient, ILocalStorageService localStorage) 
        : base(httpClient, localStorage)
    {
    }

    public async Task<List<Device>> GetDevicesAsync()
    {
        var response = await GetAsync("api/Devices");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<Device>>();
        }
        return null;
    }

    public async Task<Device> GetDeviceAsync(string id)
    {
        var response = await GetAsync($"api/Devices/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Device>();
        }
        return null;
    }

    public async Task<bool> CreateDeviceAsync(DeviceCreateDTO deviceCreateDTO)
    {
        var response = await PostAsync("api/Devices", deviceCreateDTO);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateDeviceAsync(string id, Device device)
    {
        var response = await PutAsync($"api/Devices/{id}", device);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteDeviceAsync(string id)
    {
        var response = await DeleteAsync($"api/Devices/{id}");
        return response.IsSuccessStatusCode;
    }
}
