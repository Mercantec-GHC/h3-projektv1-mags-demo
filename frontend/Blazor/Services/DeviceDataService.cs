using System.Net.Http.Json;
using API.Models;
using Blazored.LocalStorage;

public class DeviceDataService : BaseApiService
{
    public DeviceDataService(HttpClient httpClient, ILocalStorageService localStorage) 
        : base(httpClient, localStorage)
    {
    }

    public async Task<List<DeviceDataReadDTO>> GetDeviceDataAsync()
    {
        var response = await GetAsync("api/DeviceDatas");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<DeviceDataReadDTO>>();
        }
        return null;
    }

    public async Task<DeviceDataReadDTO> GetDeviceDataByIdAsync(string id)
    {
        var response = await GetAsync($"api/DeviceDatas/{id}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<DeviceDataReadDTO>();
        }
        return null;
    }

    public async Task<bool> CreateDeviceDataAsync(DeviceDataCreateDTO deviceDataCreateDTO)
    {
        var response = await PostAsync("api/DeviceDatas", deviceDataCreateDTO);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateDeviceDataAsync(string id, DeviceDataReadDTO deviceDataDTO)
    {
        var response = await PutAsync($"api/DeviceDatas/{id}", deviceDataDTO);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteDeviceDataAsync(string id)
    {
        var response = await DeleteAsync($"api/DeviceDatas/{id}");
        return response.IsSuccessStatusCode;
    }
}
