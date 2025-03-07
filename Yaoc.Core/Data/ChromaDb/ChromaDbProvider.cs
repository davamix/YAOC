using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Yaoc.Core.Data.ChromaDb;

public interface IChromaDbProvider {
    Task<bool> TestConnection();
}

public partial class ChromaDbProvider : IChromaDbProvider {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ChromaDbProvider(IConfiguration configuration) {
        _configuration = configuration;
        _httpClient = new HttpClient();

        //Task.WaitAll(InitializeDb());
        InitializeDb();
    }

    public async Task<bool> TestConnection() {
        var requestUrl = _configuration["AppSettings:ChromaDb:Server"] +
            _configuration["AppSettings:ChromaDb:ApiUrl"] +
            "heartbeat";

        try {
            var response = await _httpClient.GetAsync(requestUrl);

            return response.IsSuccessStatusCode;
        }catch(HttpRequestException ex) {
            Debug.WriteLine(ex.Message);
        }

        return false;

    }

    private async Task<string> DoPostAsync(string requestUrl, object data) {
        using StringContent sc = new(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, sc);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    private string GetBaseTenantsUrl() {
        return _configuration["AppSettings:ChromaDb:Server"] +
            _configuration["AppSettings:ChromaDb:ApiUrl"] +
            "tenants/";
    }

    private string GetTenantUrl() {
        return GetBaseTenantsUrl() + _configuration["AppSettings:ChromaDb:Tenant"];
    }

    private string GetBaseDatabaseUrl() {
        return GetTenantUrl() + "/databases/";

    }

    private string GetDatabaseUrl() {
        return GetBaseDatabaseUrl() + _configuration["AppSettings:ChromaDb:Database"];
    }
}
