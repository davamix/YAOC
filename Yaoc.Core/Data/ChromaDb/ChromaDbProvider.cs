using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Yaoc.Core.Entities.ChromaDb;
using Yaoc.Core.Requests.ChromaDb;
using Yaoc.Core.Responses.ChromaDb;

namespace Yaoc.Core.Data.ChromaDb;

public interface IChromaDbProvider {
    Task<GetCollectionsResponse> GetCollections();
    Task<CreateCollectionResponse> CreateCollection(Collection request);
    Task UpdateCollection(Collection request);
    Task DeleteCollection(Collection request);
    Task<bool> TestConnection();
}

public partial class ChromaDbProvider : IChromaDbProvider {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ChromaDbProvider(IConfiguration configuration) {
        _configuration = configuration;
        _httpClient = new HttpClient();

        Task.WhenAll(Task.Run(InitializeDb));
    }

    public async Task<GetCollectionsResponse> GetCollections() {
        var requestUrl = $"{GetDatabaseUrl()}/collections";

        var response = await _httpClient.GetAsync(requestUrl);

        response.EnsureSuccessStatusCode();

        var responseData = await response.Content.ReadAsStringAsync();

        var getResponse = new GetCollectionsResponse();
        try {
            var  collections = JsonSerializer.Deserialize<List<Collection>>(responseData);
            getResponse.Collections = collections;
        }catch(Exception ex) {
            Debug.WriteLine(ex.Message);
        }
        

        return getResponse;
    }

    public async Task<CreateCollectionResponse> CreateCollection(Collection collection) {
        var requestUrl = $"{GetDatabaseUrl()}/collections";
        
        var data = new SaveCollectionRequest {
            Name = collection.Name,
            CreateIfNotExists = true,
            Metadata = collection.Metadata,
        };

        var response = await DoPostAsync(requestUrl, data);

        var newCollection = JsonSerializer.Deserialize<Collection>(response);

        return new CreateCollectionResponse {
            Collection = newCollection
        };
    }

    public async Task UpdateCollection(Collection collection) {
        var requestUrl = $"{GetDatabaseUrl()}/collections/{collection.Id}";

        var data = new UpdateCollectionRequest {
            Name = collection.Name,
            Metadata = collection.Metadata,
        };

        await DoPutAsync(requestUrl, data);
    }

    public async Task DeleteCollection(Collection collection) {
        var requestUrl = $"{GetDatabaseUrl()}/collections/{collection.Name}";

        _ = await _httpClient.DeleteAsync(requestUrl);
    }

    public async Task<bool> TestConnection() {
        var requestUrl = _configuration["AppSettings:ChromaDb:Server"] +
            _configuration["AppSettings:ChromaDb:ApiUrl"] +
            "heartbeat";

        try {
            var response = await _httpClient.GetAsync(requestUrl);

            return response.IsSuccessStatusCode;
        } catch (HttpRequestException ex) {
            Debug.WriteLine(ex.Message);
        }

        return false;
    }

    private async Task<string> DoPostAsync(string requestUrl, object data) =>
        await DoRequestAsync(_httpClient.PostAsync, requestUrl, data);

    private async Task DoPutAsync(string requestUrl, object data) =>
        _ = await DoRequestAsync(_httpClient.PutAsync, requestUrl, data);

    private async Task<string> DoRequestAsync(Func<string?, HttpContent?, Task<HttpResponseMessage>> request, string requestUrl, object data) {
        using StringContent sc = new(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

        using HttpResponseMessage response = await request(requestUrl, sc);

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
