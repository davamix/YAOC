using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Yaoc.Core.Data.ChromaDb;

public partial class ChromaDbProvider {
    private async Task InitializeDb() {
        await InitializeTenant();
        await InitializeDatabase();
    }

    private async Task InitializeTenant() {
        if (await IsTenantCreated()) return;

        var requestUrl = GetBaseTenantsUrl();

        var data = new { name = _configuration["AppSettings:ChromaDb:Tenant"] };

        _ = await DoPostAsync(requestUrl, data);
    }

    private async Task<bool> IsTenantCreated() {
        try {
            var response = await _httpClient.GetAsync(GetTenantUrl());

            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            Debug.WriteLine(ex.Message);
        }

        return false;
        
    }

    private async Task InitializeDatabase() {
        if (await IsDatabaseCreated()) return;

        var requestUrl = GetBaseDatabaseUrl();
         var data = new { name = _configuration["AppSettings:ChromaDb:Database"] };

        _ = await DoPostAsync(requestUrl, data);
    }

    private async Task<bool> IsDatabaseCreated() {
        var response = await _httpClient.GetAsync(GetDatabaseUrl());

        return response.IsSuccessStatusCode;
    }
}
